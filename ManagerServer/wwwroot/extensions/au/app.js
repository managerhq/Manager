// Depends on globals REPORTS and TAX_CODE_TEMPLATES (loaded by reports.js / tax-codes.js).
//
// This script runs on two kinds of pages and self-detects which one via DOM elements:
//
//   • Per-report page  — has <form id="reportControls"> and a window.REPORT_ID set to a report id.
//     Renders the form, runs the figures API, displays the report.
//
//   • Country index    — has <div id="business-list">. Lists the businesses, then runs the
//     per-business workspace: running the country's reports and the tax-code setup flow.

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

// ---- Selected business (used by both page types) ---------------------------
//
// #business is a single-option <select> naming the business every API call on the page is
// scoped to. On report pages Manager supplies the name; on the country index the user picks
// it from the business list and the picker writes it here. Everything downstream — this file's
// report/tax-code tables and custom-fields.js — reads businessSelect.value and listens for
// its "change" event, so neither needs to know where the name came from.

const businessSelect = document.getElementById("business");

function setSelectedBusiness(name) {
  if (!businessSelect) return;
  businessSelect.innerHTML = "";
  if (name) {
    const opt = document.createElement("option");
    opt.value = name; opt.textContent = name; opt.selected = true;
    businessSelect.appendChild(opt);
  }
  businessSelect.dispatchEvent(new Event("change"));
}

if (businessSelect && isReportPage) {
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
    if (business) setSelectedBusiness(business);
  })();
}

// ---- Country index: shell (business list → business workspace) -------------
//
// The index page has two screens. It opens on #business-picker, a list of every business.
// Picking one enters #business-workspace, whose tabs all act on that business. The only way
// out is "Close business", which returns to the list — there is deliberately no business
// switcher inside the workspace, so a tab can never be showing one business's data while
// another is selected.

// Tab strip: each <button class="tab" data-view="X"> pairs with a <div id="X-view">.
// (Report pages get their own strip from custom-fields.js keyed on data-cf-view, which this
// selector deliberately does not match.)
const indexTabs = document.querySelectorAll(".tab[data-view]");
const indexViews = document.querySelectorAll('[id$="-view"]');
let setupLoaded = false; // has the Tax codes tab fetched for the current business yet?

function activateTab(name) {
  for (const tab of indexTabs) tab.classList.toggle("active", tab.dataset.view === name);
  for (const view of indexViews) view.hidden = view.id !== `${name}-view`;
  // Tax codes needs a round-trip to diagnose each template against the business — now that a
  // business is always selected here, do it on first open instead of making the user ask.
  if (name === "setup" && !setupLoaded && businessSelect && businessSelect.value) {
    setupLoaded = true;
    loadSetup();
  }
}

for (const tab of indexTabs) {
  tab.addEventListener("click", () => activateTab(tab.dataset.view));
}

const businessList = document.getElementById("business-list");
if (businessList && businessSelect) {
  const picker = document.getElementById("business-picker");
  const workspace = document.getElementById("business-workspace");
  const searchInput = document.getElementById("business-search");
  const currentBusinessLabel = document.getElementById("current-business");
  const closeButton = document.getElementById("closeBusiness");
  let names = [];

  function renderBusinessList() {
    const query = (searchInput.value || "").trim().toLowerCase();
    const matches = query ? names.filter(n => n.toLowerCase().includes(query)) : names;
    if (matches.length === 0) {
      businessList.innerHTML = `<div class="status">${query ? "No business matches that name." : "No businesses found. Create one in Manager first."}</div>`;
      return;
    }
    businessList.innerHTML = `<div class="business-items">` + matches.map(n =>
      `<button type="button" class="business-item" data-name="${escapeAttr(n)}">
        <span class="business-item-name">${escapeText(n)}</span>
        <span class="business-item-go" aria-hidden="true">&rarr;</span>
      </button>`).join("") + `</div>`;
  }

  function enterBusiness(name) {
    currentBusinessLabel.textContent = name;
    picker.hidden = true;
    workspace.hidden = false;
    setSelectedBusiness(name);
    activateTab("reports");
  }

  function closeBusiness() {
    workspace.hidden = true;
    picker.hidden = false;
    currentBusinessLabel.textContent = "";
    // Clearing the selection lets every tab drop the business's data instead of keeping it
    // around behind the list screen.
    setSelectedBusiness("");
    activateTab("reports");
  }

  businessList.addEventListener("click", (e) => {
    const btn = e.target.closest(".business-item");
    if (btn) enterBusiness(btn.dataset.name);
  });
  searchInput.addEventListener("input", renderBusinessList);
  closeButton.addEventListener("click", closeBusiness);

  (async function loadBusinesses() {
    let data;
    try {
      const res = await fetch("/api4/businesses", { credentials: "include" });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      data = await res.json();
    } catch (err) {
      businessList.innerHTML = `<div class="error">Could not load the list of businesses (${escapeText(err.message)}).</div>`;
      return;
    }
    names = (data.businesses || []).map(b => b.name).sort((a, b) => a.localeCompare(b));
    // A search box only earns its space once the list stops fitting on screen.
    searchInput.hidden = names.length < 9;
    renderBusinessList();
  })();
}

// ---- Report runner ---------------------------------------------------------
//
// Drives one report: reads its form controls, posts the report's definition from
// reports.js to /api4/report-transformation-figures, renders what comes back.
//
// Nothing about a report is stored in the business — the whole definition travels in
// the request body — so a runner works against any business the user can open.
//
// The form it drives is always the one in the report's own page. On that page it is
// simply the markup already there; the country index borrows the same markup (see
// loadReportForm) rather than rebuilding it, so a report's controls, their labels and
// their defaults cannot drift between the two places — which matters because several
// countries localize that form and one of them defaults to a different period type.
//
// Every lookup therefore happens when the runner starts rather than when this file
// loads, and listeners on the shared #business select carry an abort signal so the
// index can swap reports without leaving the previous runner's handlers behind.

function reportView(report) {
  if (typeof REPORT_VIEWS === "undefined") return {};
  return REPORT_VIEWS[report.id] || {};
}

// Wires up the form inside `root` — a whole report page, or the index's runner host.
// Returns a teardown function that detaches every listener it added.
function initReportRunner(report, root) {
  const view = reportView(report);
  const controller = new AbortController();
  const signal = controller.signal;
  const byId = (id) => root.querySelector("#" + id);

  const reportControls = byId("reportControls");
  const output = byId("reportOutput");
  const toDateInput = byId("toDate");
  const fromDateInput = byId("fromDate");
  const periodTypeSelect = byId("periodType");
  const quarterSelect = byId("quarter");
  const monthSelect = byId("month");
  const yearSelect = byId("year");
  // Alternative period mode: a single financial-year picker (1 Jul → 30 Jun).
  const financialYearSelect = byId("financialYear");
  // Optional per-employee scoping: when present, the user must pick an employee and the
  // employee's key is forwarded to the figures endpoint to filter transactions.
  const employeeSelect = byId("employee");

  const today = new Date();

  if (financialYearSelect) {
    // A 1 July - 30 June financial year. If we are already past 1 July in
    // this calendar year the current FY started this year; otherwise last year.
    const currentFYStart = today.getMonth() >= 6 ? today.getFullYear() : today.getFullYear() - 1;
    for (let y = currentFYStart + 1; y >= currentFYStart - 6; y--) {
      const opt = document.createElement("option");
      opt.value = String(y);
      opt.textContent = `${y}-${String((y + 1) % 100).padStart(2, "0")}`;
      financialYearSelect.appendChild(opt);
    }
    financialYearSelect.value = String(currentFYStart);
  } else if (periodTypeSelect) {
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
      for (const el of reportControls.querySelectorAll(".period-control")) {
        el.hidden = !el.dataset.types.split(" ").includes(type);
      }
    };
    periodTypeSelect.addEventListener("change", applyPeriodType, { signal });
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
    businessSelect.addEventListener("change", loadEmployees, { signal });
    // On a report page the business arrives asynchronously from the parent frame and
    // fires "change"; in the index it is already chosen, so prime the list now.
    loadEmployees();
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
    output.innerHTML = '<div class="status">Loading…</div>';

    const business = businessSelect.value;
    if (!business) { output.innerHTML = `<div class="error">Please select a business.</div>`; return; }
    const period = computePeriod();
    if (!period.from || !period.to) { output.innerHTML = `<div class="error">Please choose a valid period.</div>`; return; }
    if (employeeSelect && !employeeSelect.value) {
      output.innerHTML = `<div class="error">Please select an employee.</div>`;
      return;
    }

    const accountingEl = byId("accounting");
    const body = {
      Business: business,
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

    // Business travels in the body, so this works the same whether the page is nested
    // in Manager (which would otherwise inject it as a header) or opened on its own.
    let result;
    try {
      const res = await fetch("/api4/report-transformation-figures", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body),
        credentials: "include",
      });
      const text = await res.text();
      if (!res.ok) {
        output.innerHTML = `<div class="error">Request failed (${res.status}):\n${escapeText(text)}</div>`;
        return;
      }
      result = JSON.parse(text);
    } catch (err) {
      output.innerHTML = `<div class="error">${escapeText(err.message)}</div>`;
      return;
    }
    renderReport(report, result, business, period, { root, view });
  }, { signal });

  return () => controller.abort();
}

// Per-report page bootstrap: the form is already in the page's markup, and REPORT_ID
// names the report it belongs to.
if (isReportPage && typeof REPORTS !== "undefined") {
  const pageReport = (typeof REPORT_ID !== "undefined") ? REPORTS.find(r => r.id === REPORT_ID) : null;
  if (pageReport) {
    initReportRunner(pageReport, document);
  } else {
    document.getElementById("reportOutput").innerHTML =
      `<div class="error">Report not found. The page may be linking to a removed report.</div>`;
  }
}

// Renders a figures response into `ctx.root`. ctx carries the runner's root element and
// the report's view (see report-views.js), which between them decide where the markup
// lands and how it should look.
function renderReport(report, data, business, period, ctx) {
  const root = ctx.root;
  const view = ctx.view || {};

  // Per-report custom renderer — lets a view replace the generic table layout with a
  // form-style one (used by the PAYG payment summary). The override owns the entire
  // #reportOutput payload and the print-button binding. window.RENDER_REPORT is still
  // honoured for any page that sets it inline.
  const render = view.render || window.RENDER_REPORT;
  if (typeof render === "function") {
    render(report, data, business, period);
    return;
  }
  const totalCols = report.columns + 1;
  const accountingEl = root.querySelector("#accounting");
  const decimals = data.decimals;
  const hideZero = view.hideZeroRows === true || window.HIDE_ZERO_ROWS === true;

  // The container id "printable-content" is what legacy customScript bodies (from Assets/Localizations)
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
  html += renderRowsTbody(data.rows || [], report.columns, totalCols, decimals, hideZero);
  for (const group of data.supplierGroups || []) html += renderGroupTbody(group, report.columns, totalCols, "supplier", decimals, hideZero);
  for (const group of data.employeeGroups || []) html += renderGroupTbody(group, report.columns, totalCols, "employee", decimals, hideZero);
  html += `</table>
    </div>`;
  html += `<textarea name="content" hidden></textarea>`;

  const output = root.querySelector("#reportOutput");
  output.innerHTML = html;
  output.querySelector("#printReport").addEventListener("click", () => window.print());

  const exportCsv = view.exportCsv || window.EXPORT_CSV;
  const hasExportHook = typeof exportCsv === "function";
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
        exportCsv(report, data);
      } catch (err) {
        console.error("export csv hook failed", err);
      }
    }
    const contentEl = output.querySelector('textarea[name="content"]');
    if (contentEl && contentEl.value) {
      const filename = view.csvFilename || report.csvFilename || window.EXPORT_CSV_FILENAME || (report.name.replace(/\s+/g, "_") + ".csv");
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
// reports whose view sets hideZeroRows — markup is still emitted so the CSV
// export hook can read cell data-values, just with [hidden] applied to the row.
function isZeroRow(row) {
  if (row.isHeader) return false;
  for (const cell of row.cells || []) {
    if (!cell) continue;
    if (cell.text && String(cell.text).trim().length > 0) return false;
    if (cell.number != null && cell.number !== 0) return false;
  }
  return true;
}

function renderRowsTbody(rows, columns, totalCols, decimals, hideZero) {
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

function renderGroupTbody(group, columns, totalCols, kind, decimals, hideZero) {
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

// ---- Country index: Reports tab --------------------------------------------
//
// Two screens inside the tab. The list names every report this country ships and
// "View" runs one against the selected business there and then. The report's whole
// definition is posted with the request, so nothing about it has to be set up in the
// business first — every report this country ships is runnable against every business.

const reportList = document.getElementById("report-list");
if (reportList && typeof REPORTS !== "undefined") {
  const countrySlug = reportList.dataset.countrySlug;
  const listScreen = document.getElementById("report-list-screen");
  const runnerScreen = document.getElementById("report-runner-screen");
  const runnerHost = document.getElementById("report-runner");
  const runnerTitle = document.getElementById("runner-title");
  const backButton = document.getElementById("back-to-reports");
  // The runner screen is markup this file does not own; without it the tab still lists
  // the country's reports, and the sections below this one still load.
  const canRun = !!(listScreen && runnerScreen && runnerHost && runnerTitle);
  let activeRunner = null; // teardown for the runner currently on screen

  function reportSlug(report) {
    let s = report.name.toLowerCase().replace(/[^a-z0-9]+/g, "-").replace(/^-+|-+$/g, "");
    if (!s) s = "report-" + report.id.replace(/-/g, "").substring(0, 8);
    return s;
  }
  function reportEndpoint(report) {
    return `/extensions/${countrySlug}/${reportSlug(report)}.html`;
  }

  function renderReportTable() {
    const business = businessSelect.value;
    let html = `<table>
      <thead><tr><th>Report</th><th>Action</th></tr></thead><tbody>`;
    for (let i = 0; i < REPORTS.length; i++) {
      const r = REPORTS[i];
      const viewAction = canRun
        ? `<button class="primary" data-action="view" data-idx="${i}"${business ? "" : " disabled"}>View</button>`
        : "";
      html += `<tr><td><strong>${escapeText(r.name)}</strong></td>`
            + `<td>${viewAction}</td></tr>`;
    }
    html += `</tbody></table>`;
    reportList.innerHTML = html;

    for (const btn of reportList.querySelectorAll('button[data-action="view"]')) {
      btn.addEventListener("click", () => openRunner(REPORTS[parseInt(btn.dataset.idx, 10)]));
    }
  }

  // Borrow the report page's own <form id="reportControls"> instead of rebuilding it.
  // The page is same-origin and is the single source of truth for which controls the
  // report takes, what they are labelled and which period type it opens on — all of
  // which some countries localize. DOMParser does not run scripts, so only the markup
  // comes across; the output container is ours.
  async function loadReportForm(report) {
    const res = await fetch(reportEndpoint(report), { credentials: "include" });
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    const doc = new DOMParser().parseFromString(await res.text(), "text/html");
    const form = doc.getElementById("reportControls");
    if (!form) throw new Error("that page has no report form");
    return document.importNode(form, true);
  }

  // Only one report is on screen at a time, so the borrowed form's ids stay unique.
  // openToken guards against a slow fetch landing after the user has moved on.
  let openToken = 0;
  async function openRunner(report) {
    const token = ++openToken;
    discardRunner();
    runnerTitle.textContent = report.name;
    listScreen.hidden = true;
    runnerScreen.hidden = false;
    runnerHost.innerHTML = `<div class="status">Loading…</div>`;

    let form;
    try {
      form = await loadReportForm(report);
    } catch (err) {
      if (token !== openToken) return;
      runnerHost.innerHTML = `<div class="error">Could not load this report's form (${escapeText(err.message)}).</div>`;
      return;
    }
    if (token !== openToken) return;

    runnerHost.innerHTML = "";
    runnerHost.appendChild(form);
    const output = document.createElement("div");
    output.id = "reportOutput";
    runnerHost.appendChild(output);
    activeRunner = initReportRunner(report, runnerHost);
  }

  function discardRunner() {
    if (activeRunner) { activeRunner(); activeRunner = null; }
    runnerHost.innerHTML = "";
  }

  function closeRunner() {
    if (!canRun) return;
    openToken++; // abandon any form still loading
    discardRunner();
    runnerScreen.hidden = true;
    listScreen.hidden = false;
  }

  if (backButton) backButton.addEventListener("click", closeRunner);

  businessSelect.addEventListener("change", () => {
    // Figures on screen belong to the business they were run for, so drop them
    // rather than leave them sitting under a different business's name.
    closeRunner();
    renderReportTable();
  });
  renderReportTable();
}

// ---- Country index: Setup tax codes tab ------------------------------------

const setupView = document.getElementById("setup-view");
if (setupView) {
  const refreshBtn = document.getElementById("refreshSetup");
  if (refreshBtn) refreshBtn.addEventListener("click", loadSetup);

  // A different business is a different diagnosis: drop the table built for the old one and
  // make the next open of the tab re-fetch.
  if (businessSelect) businessSelect.addEventListener("change", () => {
    setupLoaded = false;
    document.getElementById("setupOutput").innerHTML = "";
  });

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
    // have nothing to "update" — a name match means the business already has the code.
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
