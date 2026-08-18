// Depends on globals REPORTS and TAX_CODE_TEMPLATES (loaded by reports.js / tax-codes.js).
//
// This script runs on two kinds of pages and self-detects which one via DOM elements:
//
//   • Per-report page  — has <form id="reportControls"> and a window.REPORT_ID set to a report id.
//     Renders the form, runs the figures API, displays the report.
//
//   • Country index    — has <div id="setup-view">. Drives the Setup-tax-codes flow.
//     Reports are linked statically as <a> on this page (no JS needed for the list).

const REPORTING_FIELDS = ["reportingCategory", "reportingCategoryReversed", "taxAmountReportingCategory", "taxAmountReversedReportingCategory"];

function escapeAttr(s) { return String(s).replace(/&/g, "&amp;").replace(/"/g, "&quot;"); }
function escapeText(s) { return String(s).replace(/&/g, "&amp;").replace(/</g, "&lt;"); }
function normGuid(g) { return g ? String(g).toLowerCase() : ""; }
function capitalize(s) { return s.charAt(0).toUpperCase() + s.slice(1); }
function isoDate(d) { return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`; }
function guidClass(g) { return String(g).replace(/-/g, ""); }
function formatNumber(n, decimals) {
  const d = (decimals == null) ? 2 : decimals;
  return Number(n).toLocaleString(undefined, { minimumFractionDigits: d, maximumFractionDigits: d });
}
function formatDisplayDate(s) {
  if (!s) return "";
  const d = new Date(s + "T00:00:00");
  if (isNaN(d)) return s;
  return d.toLocaleDateString(undefined, { day: "numeric", month: "long", year: "numeric" });
}

// Report pages always run within a business context (parent iframe sets window.MANAGER_BUSINESS,
// or the URL carries ?business=NAME for bookmarked/direct access). The Business dropdown and
// the back-to-country link are noise in that mode, so they are hidden on any page that has the
// report form.
const isReportPage = !!document.getElementById("reportControls");
if (isReportPage) {
  for (const el of document.querySelectorAll(".page-nav, .business-bar")) el.hidden = true;
}

// Bridge to Manager's api-proxy.js (loaded by HttpHandlers/Template.cs on the parent frame).
// The proxy injects Manager-Business automatically so the extension never needs to know which
// business it's running against. Returns { status, body } on success or rejects on missing parent.
async function postMessageWithResponse(message) {
  return new Promise((resolve, reject) => {
    if (window.parent === window) { reject(new Error("Not running inside Manager (no parent frame).")); return; }
    const requestId = crypto.randomUUID();
    function onMessage(event) {
      if (event.data?.type?.endsWith("-response") && event.data?.requestId === requestId) {
        window.removeEventListener("message", onMessage);
        resolve(event.data);
      }
    }
    window.addEventListener("message", onMessage);
    window.parent.postMessage({ ...message, requestId }, "*");
  });
}

// ---- Business dropdown (used by both page types) ---------------------------

const businessSelect = document.getElementById("business");
if (businessSelect) {
  if (isReportPage) {
    // Report pages run inside Manager — ask the parent for the page's business via page-request.
    // Fallback to ?business= query param for direct/bookmarked access outside Manager.
    (async () => {
      let business = null;
      try {
        const result = await postMessageWithResponse({ type: "page-request" });
        business = result?.body?.query?.business || null;
      } catch { /* not running inside Manager */ }
      if (!business) {
        business = new URLSearchParams(window.location.search).get("business");
      }
      if (business) {
        const opt = document.createElement("option");
        opt.value = business; opt.textContent = business; opt.selected = true;
        businessSelect.innerHTML = "";
        businessSelect.appendChild(opt);
        businessSelect.dispatchEvent(new Event("change"));
      }
    })();
  } else {
    // Country index — list every business so the user can pick where to install reports / tax codes.
    (async function loadBusinesses() {
      try {
        const res = await fetch("/api4/businesses", { credentials: "include" });
        if (!res.ok) { businessSelect.innerHTML = `<option value="">(failed to load)</option>`; return; }
        const data = await res.json();
        const names = (data.businesses || []).map(b => b.name).sort((a, b) => a.localeCompare(b));
        if (names.length === 0) { businessSelect.innerHTML = `<option value="">(no businesses)</option>`; return; }
        // Empty placeholder first so the browser doesn't auto-select a business the user didn't pick.
        businessSelect.innerHTML = `<option value="">— select a business —</option>`
          + names.map(n => `<option value="${escapeAttr(n)}">${escapeText(n)}</option>`).join("");
        businessSelect.value = "";
        businessSelect.dispatchEvent(new Event("change"));
      } catch (err) {
        businessSelect.innerHTML = `<option value="">(error)</option>`;
      }
    })();
  }
}

// ---- Per-report page -------------------------------------------------------

const reportControls = document.getElementById("reportControls");
if (reportControls) {
  const toDateInput = document.getElementById("toDate");
  const fromDateInput = document.getElementById("fromDate");
  const periodTypeSelect = document.getElementById("periodType");
  const quarterSelect = document.getElementById("quarter");
  const monthSelect = document.getElementById("month");
  const yearSelect = document.getElementById("year");
  // Optional alternative period mode: a single financial-year picker (1 Jul → 30 Jun).
  // Pages opt in by replacing the standard period controls with <select id="financialYear">.
  const financialYearSelect = document.getElementById("financialYear");
  // Optional per-employee scoping: when present, the user must pick an employee and the
  // employee's key is forwarded to the figures endpoint to filter transactions.
  const employeeSelect = document.getElementById("employee");

  const report = (typeof REPORT_ID !== "undefined") ? REPORTS.find(r => r.id === REPORT_ID) : null;
  if (!report) {
    document.getElementById("reportOutput").innerHTML =
      `<div class="error">Report not found. The page may be linking to a removed report.</div>`;
  }

  const today = new Date();

  if (financialYearSelect) {
    // Australian FY runs 1 July YYYY to 30 June YYYY+1. If we're already past 1 July
    // this calendar year, the current FY started this year; otherwise last year.
    const currentFYStart = today.getMonth() >= 6 ? today.getFullYear() : today.getFullYear() - 1;
    for (let y = currentFYStart + 1; y >= currentFYStart - 6; y--) {
      const opt = document.createElement("option");
      opt.value = String(y);
      opt.textContent = `${y}-${String((y + 1) % 100).padStart(2, "0")}`;
      financialYearSelect.appendChild(opt);
    }
    financialYearSelect.value = String(currentFYStart);
  } else {
    // Period controls: defaults are previous quarter / previous month.
    let prevQuarter = Math.floor(today.getMonth() / 3) - 1;
    let prevQuarterYear = today.getFullYear();
    if (prevQuarter < 0) { prevQuarter = 3; prevQuarterYear -= 1; }
    const prevMonthDate = new Date(today.getFullYear(), today.getMonth() - 1, 1);

    const currentYear = today.getFullYear();
    for (let y = currentYear + 1; y >= currentYear - 6; y--) {
      const opt = document.createElement("option");
      opt.value = String(y); opt.textContent = String(y);
      yearSelect.appendChild(opt);
    }
    yearSelect.value = String(prevQuarterYear);
    quarterSelect.value = String(prevQuarter);
    monthSelect.value = String(prevMonthDate.getMonth());

    const toDefault = new Date(today.getFullYear(), today.getMonth(), 0);
    const fromDefault = new Date(toDefault.getFullYear(), toDefault.getMonth() - 2, 1);
    toDateInput.value = isoDate(toDefault);
    fromDateInput.value = isoDate(fromDefault);

    const applyPeriodType = () => {
      const type = periodTypeSelect.value;
      for (const el of document.querySelectorAll(".period-control")) {
        el.hidden = !el.dataset.types.split(" ").includes(type);
      }
    };
    periodTypeSelect.addEventListener("change", applyPeriodType);
    applyPeriodType();
  }

  if (employeeSelect) {
    const loadEmployees = async () => {
      const business = businessSelect.value;
      if (!business) {
        employeeSelect.innerHTML = `<option value="">— select a business first —</option>`;
        return;
      }
      employeeSelect.innerHTML = `<option value="">Loading…</option>`;
      try {
        const res = await fetch(`/api4/employee-batch?business=${encodeURIComponent(business)}&pageSize=500`, { credentials: "include" });
        if (!res.ok) { employeeSelect.innerHTML = `<option value="">(failed to load)</option>`; return; }
        const data = await res.json();
        const employees = (data.items || [])
          .map(it => ({ key: it.key, name: (it.item || {}).name || "(unnamed)" }))
          .sort((a, b) => a.name.localeCompare(b.name));
        if (employees.length === 0) {
          employeeSelect.innerHTML = `<option value="">(no employees)</option>`;
          return;
        }
        employeeSelect.innerHTML = `<option value="">— select an employee —</option>`
          + employees.map(e => `<option value="${escapeAttr(e.key)}">${escapeText(e.name)}</option>`).join("");
      } catch {
        employeeSelect.innerHTML = `<option value="">(error)</option>`;
      }
    };
    businessSelect.addEventListener("change", loadEmployees);
  }

  function computePeriod() {
    if (financialYearSelect) {
      const startYear = parseInt(financialYearSelect.value, 10);
      return { from: isoDate(new Date(startYear, 6, 1)), to: isoDate(new Date(startYear + 1, 5, 30)) };
    }
    const type = periodTypeSelect.value;
    if (type === "custom") return { from: fromDateInput.value, to: toDateInput.value };
    const year = parseInt(yearSelect.value, 10);
    if (type === "month") {
      const month = parseInt(monthSelect.value, 10);
      return { from: isoDate(new Date(year, month, 1)), to: isoDate(new Date(year, month + 1, 0)) };
    }
    const quarter = parseInt(quarterSelect.value, 10);
    return { from: isoDate(new Date(year, quarter * 3, 1)), to: isoDate(new Date(year, quarter * 3 + 3, 0)) };
  }

  reportControls.addEventListener("submit", async (e) => {
    e.preventDefault();
    if (!report) return;
    const output = document.getElementById("reportOutput");
    output.innerHTML = '<div class="status">Loading…</div>';

    const business = businessSelect.value;
    if (!business) { output.innerHTML = `<div class="error">Please select a business.</div>`; return; }
    const period = computePeriod();
    if (!period.from || !period.to) { output.innerHTML = `<div class="error">Please choose a valid period.</div>`; return; }
    if (employeeSelect && !employeeSelect.value) {
      output.innerHTML = `<div class="error">Please select an employee.</div>`;
      return;
    }

    const accountingEl = document.getElementById("accounting");
    const body = {
      FromDate: period.from, ToDate: period.to,
      AccountingMethod: accountingEl ? parseInt(accountingEl.value, 10) : 0,
      Columns: report.columns, Items: report.items,
    };
    if (report.forEachSupplier) {
      body.ForEachSupplier = report.forEachSupplier;
      if (report.supplierCustomField) body.SupplierCustomField = report.supplierCustomField;
      if (report.supplierCustomFieldValue != null) body.SupplierCustomFieldValue = report.supplierCustomFieldValue;
    }
    if (report.forEachEmployee) body.ForEachEmployee = report.forEachEmployee;
    if (employeeSelect && employeeSelect.value) body.Employee = employeeSelect.value;

    let result;
    try {
      result = await postMessageWithResponse({
        type: "api-request",
        method: "POST",
        path: "/api4/report-transformation-figures",
        body,
      });
    } catch (err) {
      output.innerHTML = `<div class="error">${err.message}</div>`;
      return;
    }
    if (result.status < 200 || result.status >= 300) {
      const detail = typeof result.body === "string" ? result.body : JSON.stringify(result.body);
      output.innerHTML = `<div class="error">Request failed (${result.status}):\n${detail}</div>`;
      return;
    }
    renderReport(report, result.body, business, computePeriod());
  });
}

function renderReport(report, data, business, period) {
  // Per-page custom renderer hook — lets a report HTML page replace the generic
  // table layout with a form-style one (used by PAYG payment summary). The override
  // owns the entire #reportOutput payload and the print-button binding.
  if (typeof window.RENDER_REPORT === "function") {
    window.RENDER_REPORT(report, data, business, period);
    return;
  }
  const totalCols = report.columns + 1;
  const accountingEl = document.getElementById("accounting");
  const decimals = data.decimals;

  // The container id "printable-content" is what legacy customScript bodies (from Localizations.json)
  // probe via document.getElementById to scrape numeric cells for CSV export.
  let html = `
    <div class="report-actions"><button type="button" class="primary" id="printReport">Print</button></div>
    <div id="printable-content">
      <div class="report-header">
        <div class="business-name">${escapeText(business)}</div>
        <div class="report-title">${escapeText(report.name)}</div>
        <div class="report-period">For the period from ${escapeText(formatDisplayDate(period.from))} to ${escapeText(formatDisplayDate(period.to))}</div>
        ${accountingEl ? `<div class="report-basis">${accountingEl.value === "0" ? "Accrual basis" : "Cash basis"}</div>` : ""}
      </div>
      <table>`;
  html += renderRowsTbody(data.rows || [], report.columns, totalCols, decimals);
  for (const group of data.supplierGroups || []) html += renderGroupTbody(group, report.columns, totalCols, "supplier", decimals);
  for (const group of data.employeeGroups || []) html += renderGroupTbody(group, report.columns, totalCols, "employee", decimals);
  html += `</table>
    </div>`;
  html += `<textarea name="content" hidden></textarea>`;

  const output = document.getElementById("reportOutput");
  output.innerHTML = html;
  output.querySelector("#printReport").addEventListener("click", () => window.print());

  const hasExportHook = typeof window.EXPORT_CSV === "function";
  if (report.customScript || hasExportHook) {
    installScriptHelpers();
    if (report.customScript) {
      try {
        new Function(report.customScript)();
      } catch (err) {
        console.error("custom script failed", err);
      }
    }
    if (hasExportHook) {
      try {
        window.EXPORT_CSV(report, data);
      } catch (err) {
        console.error("export csv hook failed", err);
      }
    }
    const contentEl = output.querySelector('textarea[name="content"]');
    if (contentEl && contentEl.value) {
      const filename = report.csvFilename || window.EXPORT_CSV_FILENAME || (report.name.replace(/\s+/g, "_") + ".csv");
      const downloadBtn = document.createElement("button");
      downloadBtn.type = "button";
      downloadBtn.className = "primary";
      downloadBtn.style.marginLeft = "8px";
      downloadBtn.textContent = "Download CSV";
      downloadBtn.addEventListener("click", () => downloadText(filename, contentEl.value));
      output.querySelector(".report-actions").appendChild(downloadBtn);
    }
  }
}

// True when no cell in the row carries a non-zero number or non-empty text.
// Header rows return false (we never hide those). Hidden cells (cell.hidden,
// set by the API when HideIfEmpty zeroes a value) are still inspected — a row
// whose only cell is a "hidden zero" is exactly what we want to drop. Used by
// pages that set window.HIDE_ZERO_ROWS — markup is still emitted so the CSV
// customScript can read cell data-values, just with [hidden] applied to the row.
function isZeroRow(row) {
  if (row.isHeader) return false;
  for (const cell of row.cells || []) {
    if (!cell) continue;
    if (cell.text && String(cell.text).trim().length > 0) return false;
    if (cell.number != null && cell.number !== 0) return false;
  }
  return true;
}

function renderRowsTbody(rows, columns, totalCols, decimals) {
  const hideZero = window.HIDE_ZERO_ROWS === true;
  let html = `<tbody>`;
  for (const row of rows) {
    if (row.isHeader) { html += `<tr><th colspan="${totalCols}">${row.name || ""}</th></tr>`; continue; }
    const attrs = hideZero && isZeroRow(row) ? " hidden" : "";
    html += `<tr${attrs}><td>${row.name || ""}</td>`;
    const cells = row.cells || [];
    for (let i = 0; i < columns; i++) html += renderCell(cells[i], decimals);
    html += `</tr>`;
  }
  return html + `</tbody>`;
}

function renderGroupTbody(group, columns, totalCols, kind, decimals) {
  const hideZero = window.HIDE_ZERO_ROWS === true;
  const cls = kind ? `group ${kind}` : "group";
  const id = group.key ? ` id="${guidClass(group.key)}"` : "";
  let html = `<tbody class="${cls}"${id}><tr><th colspan="${totalCols}">${escapeText(group.name || "")}</th></tr>`;
  for (const row of group.rows || []) {
    if (row.isHeader) { html += `<tr><th colspan="${totalCols}">${row.name || ""}</th></tr>`; continue; }
    const attrs = hideZero && isZeroRow(row) ? " hidden" : "";
    html += `<tr${attrs}><td>${row.name || ""}</td>`;
    const cells = row.cells || [];
    for (let i = 0; i < columns; i++) html += renderCell(cells[i], decimals);
    html += `</tr>`;
  }
  return html + `</tbody>`;
}

function renderCell(cell, decimals) {
  if (!cell || cell.hidden) return `<td></td>`;
  const cls = cell.key ? guidClass(cell.key) : "";
  if (cell.number != null) {
    const formatted = formatNumber(cell.number, decimals);
    const attrs = `class="${cls}" data-value="${cell.number}"`;
    if (cell.drillDownUrl) {
      return `<td class="num"><a href="${cell.drillDownUrl}" target="_blank" rel="noopener" ${attrs}>${formatted}</a></td>`;
    }
    return `<td class="num"><span ${attrs}>${formatted}</span></td>`;
  }
  if (cell.text) return `<td><span class="${cls}" data-value="${escapeAttr(cell.text)}">${cell.text}</span></td>`;
  return `<td><span class="${cls}" data-value=""></span></td>`;
}

function downloadText(filename, text) {
  const blob = new Blob([text], { type: "text/csv;charset=utf-8" });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url; a.download = filename;
  document.body.appendChild(a); a.click(); a.remove();
  setTimeout(() => URL.revokeObjectURL(url), 0);
}

let scriptHelpersInstalled = false;
function installScriptHelpers() {
  if (scriptHelpersInstalled) return;
  scriptHelpersInstalled = true;
  HTMLCollection.prototype.getText = function () {
    return this.length === 0 ? "" : (this[0].innerText || "");
  };
  HTMLCollection.prototype.getValue = function () {
    return this.length === 0 ? 0 : (this[0].dataset.value || 0);
  };
  // eslint-disable-next-line no-extend-native
  String.prototype.defaultValue = function (value) {
    return this == "" ? value : this.toString();
  };
  window.Papa = window.Papa || {
    unparse: function (data) {
      if (!Array.isArray(data) || data.length === 0) return "";
      const headers = Object.keys(data[0]);
      const esc = (v) => {
        if (v == null) return "";
        const s = String(v);
        return /[",\n\r]/.test(s) ? `"${s.replace(/"/g, '""')}"` : s;
      };
      const lines = [headers.join(",")];
      for (const row of data) lines.push(headers.map(h => esc(row[h])).join(","));
      return lines.join("\r\n");
    },
  };
}

// ---- Country index: Install reports tab ------------------------------------

const installList = document.getElementById("report-install-list");
if (installList && typeof REPORTS !== "undefined") {
  const countrySlug = installList.dataset.countrySlug;
  let installedExtensions = [];

  function reportSlug(report) {
    let s = report.name.toLowerCase().replace(/[^a-z0-9]+/g, "-").replace(/^-+|-+$/g, "");
    if (!s) s = "report-" + report.id.replace(/-/g, "").substring(0, 8);
    return s;
  }
  function reportEndpoint(report) {
    return `/extensions/${countrySlug}/${reportSlug(report)}.html`;
  }

  async function refreshInstalled() {
    const business = businessSelect.value;
    if (!business) { installedExtensions = []; renderInstallTable(); return; }
    let res;
    try {
      res = await fetch(`/api4/extension-batch?business=${encodeURIComponent(business)}&pageSize=500`, { credentials: "include" });
    } catch {
      installedExtensions = []; renderInstallTable(); return;
    }
    if (!res.ok) { installedExtensions = []; renderInstallTable(); return; }
    const data = await res.json();
    installedExtensions = (data.items || []).map(it => ({ key: it.key, value: it.item || {} }));
    renderInstallTable();
  }

  function renderInstallTable() {
    const business = businessSelect.value;
    let html = `<table>
      <thead><tr><th>Report</th><th>Status</th><th>Action</th></tr></thead><tbody>`;
    for (let i = 0; i < REPORTS.length; i++) {
      const r = REPORTS[i];
      const endpoint = reportEndpoint(r);
      const installed = installedExtensions.find(e =>
        e.value.endpoint === endpoint && e.value.placement === "reports");
      let badge, action;
      if (!business) {
        badge = `<span class="muted">Select a business</span>`;
        action = `<button class="secondary" disabled>Install</button>`;
      } else if (installed) {
        badge = `<span class="badge ok">Installed</span>`;
        action = `<button class="secondary" data-action="uninstall" data-key="${escapeAttr(installed.key)}">Uninstall</button>`;
      } else {
        badge = `<span class="badge missing">Not installed</span>`;
        action = `<button class="secondary" data-action="install" data-idx="${i}">Install</button>`;
      }
      html += `<tr><td><strong>${escapeText(r.name)}</strong></td><td>${badge}</td><td>${action}</td></tr>`;
    }
    html += `</tbody></table>`;
    installList.innerHTML = html;

    for (const btn of installList.querySelectorAll("button[data-action]")) {
      btn.addEventListener("click", () => onInstallAction(btn));
    }
  }

  async function onInstallAction(btn) {
    const business = businessSelect.value;
    if (!business) return;
    const action = btn.dataset.action;
    btn.disabled = true;
    btn.textContent = action === "install" ? "Installing…" : "Uninstalling…";
    try {
      if (action === "install") {
        const r = REPORTS[parseInt(btn.dataset.idx, 10)];
        const value = {
          Name: r.name,
          Source: 0,
          Endpoint: reportEndpoint(r),
          Placement: "reports",
        };
        const res = await fetch("/api4/extension", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ Business: business, Value: value }),
          credentials: "include",
        });
        if (!res.ok) throw new Error(`HTTP ${res.status}: ${await res.text()}`);
      } else {
        const key = btn.dataset.key;
        const res = await fetch(`/api4/extension?business=${encodeURIComponent(business)}&key=${encodeURIComponent(key)}`, {
          method: "DELETE",
          credentials: "include",
        });
        if (!res.ok) throw new Error(`HTTP ${res.status}: ${await res.text()}`);
      }
      await refreshInstalled();
    } catch (err) {
      btn.disabled = false;
      btn.textContent = action === "install" ? "Install" : "Uninstall";
      alert(`Failed: ${err.message}`);
    }
  }

  businessSelect.addEventListener("change", refreshInstalled);
  renderInstallTable();
}

// ---- Country index: Setup tax codes tab ------------------------------------

const setupView = document.getElementById("setup-view");
if (setupView) {
  // Tab switching on the country index page. Each <button class="tab" data-view="X"> pairs with
  // a <div id="X-view"> — clicking the tab activates the tab and shows its matching view.
  const allViews = document.querySelectorAll('[id$="-view"]');
  for (const tab of document.querySelectorAll(".tab")) {
    tab.addEventListener("click", () => {
      for (const t of document.querySelectorAll(".tab")) t.classList.remove("active");
      tab.classList.add("active");
      const active = tab.dataset.view;
      for (const view of allViews) view.hidden = view.id !== `${active}-view`;
    });
  }

  const refreshBtn = document.getElementById("refreshSetup");
  if (refreshBtn) refreshBtn.addEventListener("click", loadSetup);

  // If this country has no tax-code templates, hide the Tax codes tab.
  if (typeof TAX_CODE_TEMPLATES !== "undefined" && TAX_CODE_TEMPLATES.length === 0) {
    const setupTab = document.querySelector('.tab[data-view="setup"]');
    if (setupTab) setupTab.hidden = true;
  }
}

let existingTaxCodes = [];

async function loadSetup() {
  const business = businessSelect.value;
  const output = document.getElementById("setupOutput");
  if (!business) { output.innerHTML = `<div class="error">Please select a business.</div>`; return; }
  output.innerHTML = `<div class="status">Loading tax codes…</div>`;
  let response;
  try {
    response = await fetch(`/api4/tax-code-batch?business=${encodeURIComponent(business)}&pageSize=500`, {
      credentials: "include",
    });
  } catch (err) {
    output.innerHTML = `<div class="error">Network error: ${err.message}</div>`;
    return;
  }
  if (!response.ok) {
    const text = await response.text();
    output.innerHTML = `<div class="error">Request failed (${response.status}):\n${text}</div>`;
    return;
  }
  const data = await response.json();
  // GET batch serializes the entity under JSON key "item" (see Api/Item.cs).
  existingTaxCodes = (data.items || []).map(it => ({ key: it.key, value: it.item || {} }));
  renderSetup();
}

function diagnose(template) {
  const templateName = (template.Name || "").trim().toLowerCase();
  const templateHasCategories = REPORTING_FIELDS.some(f => template[capitalize(f)]);

  if (templateHasCategories) {
    // Strong identity: all four reporting-category fields equal the template's.
    const matchConfigured = existingTaxCodes.find(tc =>
      REPORTING_FIELDS.every(f => normGuid(tc.value[f]) === normGuid(template[capitalize(f)])));
    if (matchConfigured) return { state: "ok", existing: matchConfigured };
  }

  // Name match as a fallback.
  const matchByName = existingTaxCodes.find(tc =>
    (tc.value.name || "").trim().toLowerCase() === templateName);
  if (matchByName) {
    // Templates that carry no reporting-category bindings (e.g. Slovakia's "DPH 10% Nákup")
    // have nothing to "update" — a name match means the user already has it installed.
    return { state: templateHasCategories ? "update" : "ok", existing: matchByName };
  }

  return { state: "missing", existing: null };
}

function renderSetup() {
  const output = document.getElementById("setupOutput");
  const rows = TAX_CODE_TEMPLATES.map((tpl, idx) => {
    const { state, existing } = diagnose(tpl);
    const rateText = tpl.TaxRate === 1 ? "100%" : (tpl.Rate ? `${tpl.Rate}%` : (tpl.TaxRate === 0 ? "0%" : "—"));
    const rcText = tpl.ReverseCharged ? "Yes" : "";

    let badge, action;
    if (state === "ok") {
      badge = `<span class="badge ok">Configured</span> <span class="muted">→ ${escapeText(existing.value.name || "(unnamed)")}</span>`;
      action = `<button class="secondary" disabled>OK</button>`;
    } else if (state === "update") {
      badge = `<span class="badge update">Reporting categories not linked</span> <span class="muted">→ ${escapeText(existing.value.name || "(unnamed)")}</span>`;
      action = `<button class="secondary" data-action="update" data-idx="${idx}" data-key="${escapeAttr(existing.key)}">Update existing</button>`;
    } else {
      badge = `<span class="badge missing">Not present</span>`;
      action = `<button class="secondary" data-action="create" data-idx="${idx}">Create</button>`;
    }
    return `<tr>
      <td><strong>${escapeText(tpl.Name)}</strong>${tpl.Label ? `<div class="muted">${escapeText(tpl.Label)}</div>` : ""}</td>
      <td>${rateText}</td>
      <td>${rcText}</td>
      <td>${badge}</td>
      <td>${action}</td>
    </tr>`;
  }).join("");

  output.innerHTML = `<table>
    <thead><tr><th>Tax code</th><th>Rate</th><th>Reverse charged</th><th>Status</th><th>Action</th></tr></thead>
    <tbody>${rows}</tbody>
  </table>`;

  for (const btn of output.querySelectorAll("button[data-action]")) {
    btn.addEventListener("click", () => onSetupAction(btn));
  }
}

async function onSetupAction(btn) {
  const business = businessSelect.value;
  const idx = parseInt(btn.dataset.idx, 10);
  const action = btn.dataset.action;
  const template = TAX_CODE_TEMPLATES[idx];
  btn.disabled = true;
  btn.textContent = action === "create" ? "Creating…" : "Updating…";
  try {
    if (action === "create") {
      const res = await fetch("/api4/tax-code", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ Business: business, Value: template }),
        credentials: "include",
      });
      if (!res.ok) throw new Error(`HTTP ${res.status}: ${await res.text()}`);
    } else {
      const key = btn.dataset.key;
      const existing = existingTaxCodes.find(tc => tc.key === key);
      if (!existing) throw new Error("Existing tax code not found");
      const updated = { ...existing.value };
      updated.reportingCategory = template.ReportingCategory || null;
      updated.reportingCategoryReversed = template.ReportingCategoryReversed || null;
      updated.taxAmountReportingCategory = template.TaxAmountReportingCategory || null;
      updated.taxAmountReversedReportingCategory = template.TaxAmountReversedReportingCategory || null;
      const res = await fetch("/api4/tax-code", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ Business: business, Key: key, Value: updated }),
        credentials: "include",
      });
      if (!res.ok) throw new Error(`HTTP ${res.status}: ${await res.text()}`);
    }
    await loadSetup();
  } catch (err) {
    btn.disabled = false;
    btn.textContent = action === "create" ? "Create" : "Update existing";
    alert(`Failed: ${err.message}`);
  }
}
