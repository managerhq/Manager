// Custom-field setup UI for the AU extension. Drives both the index page
// tabs (Business / Suppliers / Employees) and a per-report disclosure
// panel injected above the report output on each report page.
//
// Field GUIDs match the entries under section dcb382dc-… in
// Assets/Localizations.json (the "en-AU" CustomField bag, auto-seeded into
// every business that activates the AU localization — no extension-side
// metadata POST required).
//
// All values are stored in the legacy CustomFields bag — a flat string
// dictionary (Dictionary<Guid, string>) — not the newer typed CustomFields2.
// Wire format: GET → item.customFields[guid], PUT body → { Business, Key, Value }.

(function () {
  const BUSINESS_FIELDS = [
    { id: "0fba87ee-0386-401a-8d1f-302313b663f4", label: "ABN", type: "text",
      help: "Australian Business Number — used as Entity ABN / Payer ABN on BAS, STP and TPAR." },
  ];

  const SUPPLIER_FIELDS = [
    { id: "e3f5b9dd-ee9a-4fd0-9ca9-750c9aad9b1f", label: "Include in TPAR", type: "select", options: ["", "Yes", "No"] },
    { id: "9483841a-8d21-4c77-8031-614f721878fa", label: "ABN",             type: "text" },
  ];

  // STP Phase 2 fields, grouped to mirror how a payroll officer thinks about
  // them. Order within a group also drives form layout.
  const EMPLOYEE_FIELD_GROUPS = [
    { heading: "Identification", fields: [
      { id: "8067ce2d-6a48-45f1-a5a9-5e2d923e9cc7", label: "Tax File Number (TFN)", type: "text" },
      { id: "b8c661cd-ff56-4862-853b-75d0f2920776", label: "Payroll number",        type: "text" },
      { id: "0713b751-ee07-4d22-8b17-ad0131029ca0", label: "Given name",            type: "text" },
      { id: "91a2b722-99cd-41e1-b638-532793aae782", label: "Middle name",           type: "text" },
      { id: "11acbfb3-9557-487a-8b8b-2528a2c43c53", label: "Family name",           type: "text" },
      { id: "f6859bbb-1736-4e60-81f2-2703a9ea4686", label: "Date of birth",         type: "date" },
    ]},
    { heading: "Address", fields: [
      { id: "57a258c7-3296-4b4a-bc6c-82ba5755c222", label: "Address 1",       type: "text" },
      { id: "ce735064-e907-478f-83b0-f319b2a9a7fb", label: "Address 2",       type: "text" },
      { id: "f6326e38-61c9-41c6-8bda-7a9f2d594150", label: "Suburb",          type: "text" },
      { id: "050a3828-8ba0-4db9-b560-f24a3c5e413b", label: "State/territory", type: "text" },
      { id: "dc15193e-9883-4e89-a78b-7e6751db5240", label: "Postcode",        type: "text" },
      { id: "343a8633-5d10-46ca-9d20-0beed32ebab8", label: "Country",         type: "select",
        options: ["", "au — Australia"] },
    ]},
    { heading: "Employment", fields: [
      { id: "03d6a7cc-2fc8-47f7-a001-8d714c55cd48", label: "Hired date", type: "date" },
      { id: "6b837666-1039-4cfc-948b-5c36b30682c2", label: "Payment basis (basis of employment code)", type: "select",
        options: ["", "F — Full time", "P — Part time", "C — Casual", "L — Labour hire", "V — Voluntary Agreement", "D — Death Beneficiary", "N — Non-Employee"] },
      { id: "af894e25-9a1e-4384-9536-2f816d86c2e5", label: "Income stream code", type: "select",
        options: ["", "SAW - Salary and Wages", "CHP - Closely Held Payees", "IAA - Inbound Assignees to Australia",
                  "WHM - Working Holiday Makers", "SWP - Seasonal Worker Programme", "FEI - Foreign Employment",
                  "JPD - Joint Petroleum Development Area", "VOL - Voluntary Agreement", "LAB - Labour-Hire",
                  "OSP - Other Specified Payments"] },
      { id: "2ee7f0d0-83f3-4b43-8ff9-2d34291ad7b1", label: "Income stream country code", type: "text",
        help: "Required only for Working Holiday Makers (WHM). ISO 3166-1 alpha-2, not AU/CC/CX/HM/NF." },
    ]},
    { heading: "Tax treatment", fields: [
      { id: "8980d8c4-ed06-4534-9b22-4e5d4fca2595", label: "Tax treatment code", type: "select",
        options: ["", "R - Regular", "A - Actors", "C - Horticulturists and Shearers", "S – Seniors and Pensioners",
                  "H – Working Holiday Maker", "W – Seasonal Worker Programme", "F – Foreign Resident",
                  "N – No TFN", "D – ATO Defined", "V – Voluntary Agreement"] },
      { id: "89005ebb-4898-4f79-81f3-471b61143d77", label: "Tax treatment option", type: "select",
        options: ["", "T – With tax-free threshold", "D – Daily casuals", "N – With no tax-free threshold",
                  "P – Promotional (Actors)", "F – Foreign resident", "S – Single (Seniors and Pensioners)",
                  "M – Couple (Seniors and Pensioners)", "I – Illness separated couple (Seniors and Pensioners)",
                  "R – Registered employer (Working Holiday Maker)", "U – Unregistered employer (Working Holiday Maker)",
                  "A – Australian resident (No TFN)", "B – Death Beneficiary", "V – Downward Variation",
                  "Z – Non-Employee", "C – With Commissioner's instalment rate (CIR) (Voluntary Agreement)",
                  "O – Without Commissioner's instalment rate (CIR) (Voluntary Agreement)"] },
      { id: "b159d29e-b71f-4f45-aacf-c61ac53fb576", label: "Study and Training Support Loan", type: "select",
        options: ["", "S – Has STSL", "X – No STSL"] },
      { id: "570b929c-64cc-478c-b26f-332841fd9632", label: "Medicare levy surcharge", type: "select",
        options: ["", "1 – Tier 1", "2 – Tier 2", "3 – Tier 3", "X – No surcharge"] },
      { id: "8bbfc716-4cf7-4ad9-9cc5-57fa4bcd8176", label: "Medicare levy exemption", type: "select",
        options: ["", "H - Half", "F - Full", "X - No exemption"] },
    ]},
    { heading: "Termination", fields: [
      { id: "2d6b0b42-0dc7-4242-9f9d-c37a9eba5c48", label: "Termination date", type: "date" },
      { id: "cc5736e6-2ecb-45b4-8920-4b9e0cef15c9", label: "Termination type", type: "select",
        options: ["", "V - Voluntary Cessation", "I - Ill Health", "D - Deceased", "R - Redundancy",
                  "F - Dismissal", "C - Contract Cessation", "T - Transfer"] },
    ]},
  ];

  // Which sections each report wants in its setup panel. "follow:#elementId"
  // means "track the employee dropdown that already exists in the report
  // form" rather than rendering an own picker — used for PAYG where the
  // report itself drives employee selection.
  const REPORT_SETUP = {
    // Business Activity Statement
    "11acbfe1-0d24-4161-b366-fe905f2bcfd9": { business: true },
    // PAYG payment summary — individual non-business
    "92b38154-38fc-479a-a296-2019f656d1e2": { business: true, employee: "follow:#employee", payslipItems: true },
    // Single Touch Payroll Worksheet (Phase 2)
    "07332ba3-3e82-4dc1-9451-1350f5d84e24": { business: true, employee: "own", payslipItems: true },
    // Taxable Payments Annual Report (TPAR)
    "c4a0ccf7-9171-4e8e-b390-97f7052b1479": { business: true, suppliers: true },
  };

  // Payslip-item reporting categories defined by the AU localization
  // (Localizations.json sections 3de1fae6-… / 1ccb2c74-… / ad4c002b-…). The
  // server has no public endpoint to list these, but the GUIDs are seeded
  // into every business that activates the AU localization, so we can hard
  // code them here — exactly the same approach we take for custom fields.
  //
  // `endpointBase` is the kebab-cased item type used by the API
  // (`/api4/payslip-earnings-item-batch` for GET batch, `/api4/payslip-earnings-item` for PUT).
  const PAYSLIP_ITEM_TYPES = [
    {
      key: "earnings",
      label: "Earnings",
      endpointBase: "payslip-earnings-item",
      categories: [
        { id: "1b442ce9-b447-452d-a051-a2d4205a7b05", name: "Bonus commission" },
        { id: "d0321999-ab88-4cb9-88fb-083df3dab634", name: "Cashout leave" },
        { id: "35b90096-c604-4f02-b0f1-edce5bed95c7", name: "CDEP payments" },
        { id: "88c4b86a-4287-4473-90a8-3981db7b070e", name: "Defence leave" },
        { id: "44bfada5-9d85-4dfe-8783-1e111fafb2ed", name: "Directors fees" },
        { id: "f9e00d0c-396d-4ba4-9ab0-e81114c8b1f5", name: "Exempt foreign employment income" },
        { id: "f25bdb57-d366-45cc-93db-2e401fb7001f", name: "Gross payments" },
        { id: "1eeec512-af7f-4cc5-bb83-1231c27c2e91", name: "Gross payments - OTE" },
        { id: "1b250247-e58f-4345-8fb3-5283a4ddd8e4", name: "Kilometer allowance" },
        { id: "9cd51c1d-fdf9-41cb-89fd-1f65e72d13b2", name: "Laundry allowance" },
        { id: "19c366ef-0da2-46e3-8b48-c09aa985cab7", name: "Lump sum A (type R)" },
        { id: "2cc195f2-6f31-4035-a969-9c98c80ce132", name: "Lump sum A (type T)" },
        { id: "3540ac43-bcf2-4499-855e-d13612ab9829", name: "Lump sum B" },
        { id: "c8adcd5b-af39-4adf-8c67-d5038aa6c008", name: "Lump sum D" },
        { id: "1c8995a9-b06c-4f93-add4-66db7fb577b7", name: "Lump sum E" },
        { id: "99ed30cd-78df-4532-ac14-4c3c9be30005", name: "Lump sum W" },
        { id: "0477def9-f904-497f-8b37-6b174227d544", name: "Meal allowance" },
        { id: "ae443a9c-fb4e-4518-a829-f3f4bbfbfb10", name: "Other allowance" },
        { id: "8ac17e05-8428-4be0-9271-d0d5e6005f06", name: "Other leave" },
        { id: "b03d3161-ba98-4110-b4a9-b25ad8f32518", name: "Overtime" },
        { id: "bfe5e639-f749-424f-b405-135d8a60884f", name: "Parental leave" },
        { id: "b88234cb-70ea-40d2-8a4b-cb6e9016f6ce", name: "Qualifications allowance" },
        { id: "70d6f505-8911-4a04-94b8-6bd349577586", name: "Tasks allowance" },
        { id: "08dd214c-9b39-4c3a-8c19-5b121f23627f", name: "Term unused leave" },
        { id: "b81229e3-cb3c-4d11-90cf-23b6e46ae147", name: "Tool allowance" },
        { id: "cf03ab85-80a9-4cdb-afca-8b9db1e300d7", name: "Transport allowance" },
        { id: "9d376dc3-c56c-48fa-95cd-976223d57a66", name: "Travel allowance" },
        { id: "404fb279-cb40-4098-a9f3-73891e2fd2f3", name: "Workers comp leave" },
      ],
    },
    {
      key: "deductions",
      label: "Deductions",
      endpointBase: "payslip-deduction-item",
      categories: [
        { id: "43dc8de9-5e59-471e-b220-111c97ada19e", name: "PAYG" },
        { id: "0a084bbf-9b39-460a-aa36-5085ed19c99c", name: "Union / association fees" },
        { id: "f1bc8fba-622b-44bf-893c-ad33834c7c88", name: "Workplace giving" },
      ],
    },
    {
      key: "contributions",
      label: "Contributions",
      endpointBase: "payslip-contribution-item",
      categories: [
        { id: "e42078eb-62da-4992-845b-084a976e404d", name: "Reportable employer super contribution" },
        { id: "d4bc3a93-b10a-4a88-ab40-d4539bff054e", name: "Superannuation guarantee" },
      ],
    },
  ];

  // ---- Small helpers --------------------------------------------------------

  function escapeAttr(s) { return String(s).replace(/&/g, "&amp;").replace(/"/g, "&quot;"); }
  function escapeText(s) { return String(s).replace(/&/g, "&amp;").replace(/</g, "&lt;"); }

  function readField(model, field) {
    const cf = model && model.customFields;
    if (!cf) return "";
    const v = cf[field.id];
    return v == null ? "" : String(v);
  }

  function patchCustomFields(existing, updates) {
    const out = Object.assign({}, existing || {});
    for (const { field, value } of updates) {
      if (value === "" || value == null) delete out[field.id];
      else out[field.id] = String(value);
    }
    return out;
  }

  function renderControl(field, value, idPrefix) {
    const inputId = `${idPrefix}-${field.id}`;
    if (field.type === "select") {
      const opts = field.options.map(o => {
        const sel = o === value ? " selected" : "";
        return `<option value="${escapeAttr(o)}"${sel}>${escapeText(o || "—")}</option>`;
      }).join("");
      return `<select id="${inputId}" data-field-id="${field.id}" data-field-type="select">${opts}</select>`;
    }
    if (field.type === "date") {
      return `<input id="${inputId}" type="date" value="${escapeAttr(value)}" data-field-id="${field.id}" data-field-type="date">`;
    }
    return `<input id="${inputId}" type="text" value="${escapeAttr(value)}" data-field-id="${field.id}" data-field-type="text">`;
  }

  function renderField(field, value, idPrefix) {
    const control = renderControl(field, value, idPrefix);
    const help = field.help ? `<small class="muted">${escapeText(field.help)}</small>` : "";
    return `<label class="cf-field"><span>${escapeText(field.label)}</span>${control}${help}</label>`;
  }

  function collectFieldValues(container, fields) {
    const updates = [];
    for (const f of fields) {
      const el = container.querySelector(`[data-field-id="${f.id}"]`);
      if (!el) continue;
      updates.push({ field: f, value: el.value });
    }
    return updates;
  }

  async function getJson(url) {
    const res = await fetch(url, { credentials: "include" });
    if (!res.ok) throw new Error(`GET ${url} → HTTP ${res.status}: ${await res.text()}`);
    return res.json();
  }

  async function putJson(url, body) {
    const res = await fetch(url, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(body),
      credentials: "include",
    });
    if (!res.ok) throw new Error(`PUT ${url} → HTTP ${res.status}: ${await res.text()}`);
    return res;
  }

  function flash(btn, kind, text) {
    const old = btn.dataset.originalText || btn.textContent;
    btn.dataset.originalText = old;
    btn.disabled = true;
    btn.textContent = text;
    btn.classList.add(kind === "ok" ? "save-ok" : "save-err");
    setTimeout(() => {
      btn.textContent = old;
      btn.classList.remove("save-ok", "save-err");
      btn.disabled = false;
    }, kind === "ok" ? 1200 : 3500);
  }

  const businessSelect = document.getElementById("business");
  function currentBusiness() { return businessSelect ? businessSelect.value : ""; }
  function noBusinessMessage() { return `<div class="status">Please select a business above.</div>`; }

  // ---- Section: Business ABN ------------------------------------------------

  // Mounts the Business ABN editor into `container`. Returns a controller with
  // refresh() so callers can re-trigger fetches on business change / tab open.
  function mountBusinessSection(container, opts) {
    opts = opts || {};
    let currentModel = {};

    async function refresh() {
      const business = currentBusiness();
      if (!business) { container.innerHTML = noBusinessMessage(); return; }
      container.innerHTML = `<div class="status">Loading…</div>`;
      try {
        currentModel = (await getJson(`/api4/business-details?business=${encodeURIComponent(business)}`)) || {};
      } catch (err) {
        container.innerHTML = `<div class="error">${escapeText(err.message)}</div>`;
        return;
      }
      render();
    }

    function render() {
      const intro = opts.intro != null
        ? opts.intro
        : `<p class="muted">These values are stored as custom fields on the business and used by BAS, STP and TPAR reports.</p>`;
      const fieldsHtml = BUSINESS_FIELDS.map(f => renderField(f, readField(currentModel, f), "biz")).join("");
      container.innerHTML = `
        ${intro}
        <form class="cf-form cf-business-form">
          ${fieldsHtml}
          <div class="cf-actions"><button type="submit" class="primary">Save</button></div>
        </form>`;
      container.querySelector("form").addEventListener("submit", onSave);
    }

    async function onSave(e) {
      e.preventDefault();
      const business = currentBusiness();
      if (!business) return;
      const form = e.currentTarget;
      const btn = form.querySelector("button[type=submit]");
      const updates = collectFieldValues(form, BUSINESS_FIELDS);
      const updated = Object.assign({}, currentModel, {
        customFields: patchCustomFields(currentModel.customFields, updates),
      });
      try {
        await putJson("/api4/business-details", { Business: business, Value: updated });
        currentModel = updated;
        flash(btn, "ok", "Saved");
      } catch (err) {
        console.error(err);
        flash(btn, "err", "Failed — retry");
        alert(err.message);
      }
    }

    return { refresh };
  }

  // ---- Section: Suppliers (table) -------------------------------------------

  function mountSuppliersSection(container, opts) {
    opts = opts || {};
    let cache = []; // [{ key, value }]

    async function refresh() {
      const business = currentBusiness();
      if (!business) { container.innerHTML = noBusinessMessage(); return; }
      container.innerHTML = `<div class="status">Loading suppliers…</div>`;
      let data;
      try {
        data = await getJson(`/api4/supplier-batch?business=${encodeURIComponent(business)}&pageSize=2000`);
      } catch (err) {
        container.innerHTML = `<div class="error">${escapeText(err.message)}</div>`;
        return;
      }
      cache = (data.items || [])
        .map(it => ({ key: it.key, value: it.item || {} }))
        .sort((a, b) => (a.value.name || "").localeCompare(b.value.name || ""));
      render();
    }

    function render() {
      if (cache.length === 0) {
        container.innerHTML = `<div class="status">No suppliers found for this business.</div>`;
        return;
      }
      const rows = cache.map((s, idx) => {
        const fieldCells = SUPPLIER_FIELDS.map(f =>
          `<td>${renderControl(f, readField(s.value, f), `sup-${idx}`)}</td>`
        ).join("");
        return `<tr data-key="${escapeAttr(s.key)}" data-idx="${idx}">
          <td><strong>${escapeText(s.value.name || "(unnamed)")}</strong>${s.value.code ? `<div class="muted">${escapeText(s.value.code)}</div>` : ""}</td>
          ${fieldCells}
          <td><button class="secondary" data-action="save-supplier">Save</button></td>
        </tr>`;
      }).join("");
      const headers = SUPPLIER_FIELDS.map(f => `<th>${escapeText(f.label)}</th>`).join("");
      const intro = opts.intro != null
        ? opts.intro
        : `<p class="muted">Mark each supplier that should appear on the TPAR (Yes/No) and fill in their ABN. Saves one row at a time.</p>`;
      container.innerHTML = `
        ${intro}
        <div class="cf-table-wrap">
          <table class="cf-table">
            <thead><tr><th>Supplier</th>${headers}<th></th></tr></thead>
            <tbody>${rows}</tbody>
          </table>
        </div>`;
      for (const btn of container.querySelectorAll('button[data-action="save-supplier"]')) {
        btn.addEventListener("click", onSave);
      }
    }

    async function onSave(e) {
      const btn = e.currentTarget;
      const row = btn.closest("tr");
      const idx = parseInt(row.dataset.idx, 10);
      const key = row.dataset.key;
      const business = currentBusiness();
      if (!business) return;
      const current = cache[idx];
      if (!current) return;
      const updates = collectFieldValues(row, SUPPLIER_FIELDS);
      const updated = Object.assign({}, current.value, {
        customFields: patchCustomFields(current.value.customFields, updates),
      });
      try {
        await putJson("/api4/supplier", { Business: business, Key: key, Value: updated });
        current.value = updated;
        flash(btn, "ok", "Saved");
      } catch (err) {
        console.error(err);
        flash(btn, "err", "Failed");
        alert(err.message);
      }
    }

    return { refresh };
  }

  // ---- Section: Employee (picker + form) ------------------------------------

  // opts.followSelectId: when set, the section reflects the employee selected
  // in an external <select> (e.g. PAYG's #employee). Otherwise it shows its
  // own picker. opts.intro: optional override for the leading paragraph.
  function mountEmployeeSection(container, opts) {
    opts = opts || {};
    let cache = [];
    let currentKey = "";

    async function refresh() {
      const business = currentBusiness();
      if (!business) { container.innerHTML = noBusinessMessage(); return; }
      container.innerHTML = `<div class="status">Loading employees…</div>`;
      let data;
      try {
        data = await getJson(`/api4/employee-batch?business=${encodeURIComponent(business)}&pageSize=2000`);
      } catch (err) {
        container.innerHTML = `<div class="error">${escapeText(err.message)}</div>`;
        return;
      }
      cache = (data.items || [])
        .map(it => ({ key: it.key, value: it.item || {} }))
        .sort((a, b) => (a.value.name || "").localeCompare(b.value.name || ""));
      renderShell();
      // Re-apply the externally-driven selection (PAYG) or reset (own picker).
      if (opts.followSelectId) syncFromExternalPicker();
      else currentKey = "";
    }

    function renderShell() {
      const intro = opts.intro != null
        ? opts.intro
        : `<p class="muted">STP Phase 2 custom fields. Choose an employee to view and edit their values; each save updates that employee only.</p>`;
      if (opts.followSelectId) {
        container.innerHTML = `${intro}<div class="cf-employee-host"></div>`;
      } else {
        const opts2 = [`<option value="">— select an employee —</option>`].concat(
          cache.map(e => `<option value="${escapeAttr(e.key)}">${escapeText(e.value.name || "(unnamed)")}</option>`)
        ).join("");
        container.innerHTML = `
          ${intro}
          <div class="cf-picker">
            <label>Employee
              <select class="cf-employee-picker">${opts2}</select>
            </label>
          </div>
          <div class="cf-employee-host"></div>`;
        container.querySelector(".cf-employee-picker").addEventListener("change", e => {
          currentKey = e.target.value;
          renderForm();
        });
      }
    }

    function syncFromExternalPicker() {
      const ext = document.querySelector(opts.followSelectId);
      currentKey = ext ? ext.value : "";
      renderForm();
    }

    function renderForm() {
      const host = container.querySelector(".cf-employee-host");
      if (!host) return;
      if (!currentKey) {
        host.innerHTML = opts.followSelectId
          ? `<div class="status">Select an employee in the form above to edit their STP fields here.</div>`
          : "";
        return;
      }
      const employee = cache.find(emp => emp.key === currentKey);
      if (!employee) {
        host.innerHTML = `<div class="status">Employee not loaded — click the panel header to reload.</div>`;
        return;
      }
      const employeeName = employee.value.name || "(unnamed)";
      const heading = opts.followSelectId
        ? `<div class="cf-employee-heading">Editing: <strong>${escapeText(employeeName)}</strong></div>`
        : "";
      const groups = EMPLOYEE_FIELD_GROUPS.map(group => {
        const fields = group.fields.map(f => renderField(f, readField(employee.value, f), `emp-${employee.key}`)).join("");
        return `<fieldset class="cf-group">
          <legend>${escapeText(group.heading)}</legend>
          <div class="cf-grid">${fields}</div>
        </fieldset>`;
      }).join("");
      host.innerHTML = `
        ${heading}
        <form class="cf-form cf-employee-form">
          ${groups}
          <div class="cf-actions"><button type="submit" class="primary">Save employee</button></div>
        </form>`;
      host.querySelector("form").addEventListener("submit", e => onSave(e, employee));
    }

    async function onSave(e, employee) {
      e.preventDefault();
      const business = currentBusiness();
      if (!business) return;
      const form = e.currentTarget;
      const btn = form.querySelector("button[type=submit]");
      const allFields = EMPLOYEE_FIELD_GROUPS.flatMap(g => g.fields);
      const updates = collectFieldValues(form, allFields);
      const updated = Object.assign({}, employee.value, {
        customFields: patchCustomFields(employee.value.customFields, updates),
      });
      try {
        await putJson("/api4/employee", { Business: business, Key: employee.key, Value: updated });
        employee.value = updated;
        flash(btn, "ok", "Saved");
      } catch (err) {
        console.error(err);
        flash(btn, "err", "Failed — retry");
        alert(err.message);
      }
    }

    // Hook into an external picker (PAYG #employee) — re-render the form when
    // it changes. The element exists statically in the report HTML; only the
    // <option> list is populated async by app.js.
    if (opts.followSelectId) {
      const ext = document.querySelector(opts.followSelectId);
      if (ext) {
        ext.addEventListener("change", () => {
          if (cache.length === 0) return; // wait for the section's first refresh
          syncFromExternalPicker();
        });
      }
    }

    return { refresh };
  }

  // ---- Section: Payslip items (three stacked tables) -----------------------

  // For each payslip-item type (earnings / deductions / contributions) lists
  // every item in the business and lets the user assign a reporting category
  // from the AU localization's catalog. ReportingCategory is a top-level
  // `Guid?` on the model — not a custom field — so this section does NOT use
  // the CustomFields machinery. Per-row save like Suppliers.
  function mountPayslipItemsSection(container, opts) {
    opts = opts || {};
    // caches[typeKey] = [{ key, value }]
    const caches = {};

    async function refresh() {
      const business = currentBusiness();
      if (!business) { container.innerHTML = noBusinessMessage(); return; }
      container.innerHTML = `<div class="status">Loading payslip items…</div>`;
      try {
        const results = await Promise.all(PAYSLIP_ITEM_TYPES.map(t =>
          getJson(`/api4/${t.endpointBase}-batch?business=${encodeURIComponent(business)}&pageSize=2000`)
        ));
        for (let i = 0; i < PAYSLIP_ITEM_TYPES.length; i++) {
          const t = PAYSLIP_ITEM_TYPES[i];
          caches[t.key] = (results[i].items || [])
            .map(it => ({ key: it.key, value: it.item || {} }))
            .sort((a, b) => (a.value.name || "").localeCompare(b.value.name || ""));
        }
      } catch (err) {
        container.innerHTML = `<div class="error">${escapeText(err.message)}</div>`;
        return;
      }
      render();
    }

    function render() {
      const intro = opts.intro != null
        ? opts.intro
        : `<p class="muted">Assign each payslip item to a reporting category so its values flow into the right rows of STP and PAYG reports.</p>`;
      const sections = PAYSLIP_ITEM_TYPES.map(renderTable).join("");
      container.innerHTML = intro + sections;
      for (const btn of container.querySelectorAll('button[data-action="save-payslip-item"]')) {
        btn.addEventListener("click", onSave);
      }
    }

    function renderTable(type) {
      const items = caches[type.key] || [];
      const heading = `<h3 class="cf-payslip-heading">${escapeText(type.label)}</h3>`;
      if (items.length === 0) {
        return `${heading}<div class="status">No ${type.label.toLowerCase()} items in this business.</div>`;
      }
      const optionsHtml = ["", ...type.categories.map(c => c.id)].map(id => {
        if (!id) return `<option value="">— none —</option>`;
        const c = type.categories.find(cat => cat.id === id);
        return `<option value="${escapeAttr(id)}">${escapeText(c.name)}</option>`;
      }).join("");
      const rows = items.map((it, idx) => {
        const current = (it.value.reportingCategory || "").toLowerCase();
        // Mark the option matching the current value as selected. Build the
        // string fresh per row because we need to embed `selected` on the
        // matching <option>.
        const opts2 = ["", ...type.categories.map(c => c.id)].map(id => {
          const sel = (id || "").toLowerCase() === current ? " selected" : "";
          if (!id) return `<option value=""${sel}>— none —</option>`;
          const c = type.categories.find(cat => cat.id === id);
          return `<option value="${escapeAttr(id)}"${sel}>${escapeText(c.name)}</option>`;
        }).join("");
        return `<tr data-type="${type.key}" data-key="${escapeAttr(it.key)}" data-idx="${idx}">
          <td><strong>${escapeText(it.value.name || "(unnamed)")}</strong>${it.value.inactive ? ` <span class="muted">(inactive)</span>` : ""}</td>
          <td><select data-role="category">${opts2}</select></td>
          <td><button class="secondary" data-action="save-payslip-item">Save</button></td>
        </tr>`;
      }).join("");
      return `${heading}
        <div class="cf-table-wrap">
          <table class="cf-table">
            <thead><tr><th>Item</th><th>Reporting category</th><th></th></tr></thead>
            <tbody>${rows}</tbody>
          </table>
        </div>`;
    }

    async function onSave(e) {
      const btn = e.currentTarget;
      const row = btn.closest("tr");
      const typeKey = row.dataset.type;
      const type = PAYSLIP_ITEM_TYPES.find(t => t.key === typeKey);
      const idx = parseInt(row.dataset.idx, 10);
      const key = row.dataset.key;
      const business = currentBusiness();
      if (!business || !type) return;
      const current = caches[typeKey][idx];
      if (!current) return;
      const newCategory = row.querySelector('select[data-role="category"]').value || null;
      const updated = Object.assign({}, current.value, {
        reportingCategory: newCategory,
      });
      try {
        await putJson(`/api4/${type.endpointBase}`, { Business: business, Key: key, Value: updated });
        current.value = updated;
        flash(btn, "ok", "Saved");
      } catch (err) {
        console.error(err);
        flash(btn, "err", "Failed");
        alert(err.message);
      }
    }

    return { refresh };
  }

  // ==========================================================================
  // Bootstrap A: Index page (Welcome / Reports / Tax codes / Business /
  // Suppliers / Employees / Payslip items). Tab-driven, with lazy first-load
  // on tab activate.
  // ==========================================================================

  const businessView     = document.getElementById("business-view");
  const suppliersView    = document.getElementById("suppliers-view");
  const employeesView    = document.getElementById("employees-view");
  const payslipItemsView = document.getElementById("payslip-items-view");

  if (businessView || suppliersView || employeesView || payslipItemsView) {
    const sections = {
      business:      businessView     ? mountBusinessSection(businessView)         : null,
      suppliers:     suppliersView    ? mountSuppliersSection(suppliersView)       : null,
      employees:     employeesView    ? mountEmployeeSection(employeesView)        : null,
      payslipItems:  payslipItemsView ? mountPayslipItemsSection(payslipItemsView) : null,
    };

    function refreshActive() {
      if (sections.business     && businessView     && !businessView.hidden)     sections.business.refresh();
      if (sections.suppliers    && suppliersView    && !suppliersView.hidden)    sections.suppliers.refresh();
      if (sections.employees    && employeesView    && !employeesView.hidden)    sections.employees.refresh();
      if (sections.payslipItems && payslipItemsView && !payslipItemsView.hidden) sections.payslipItems.refresh();
    }

    if (businessSelect) businessSelect.addEventListener("change", refreshActive);

    for (const tab of document.querySelectorAll('.tab[data-view="business"], .tab[data-view="suppliers"], .tab[data-view="employees"], .tab[data-view="payslip-items"]')) {
      // app.js toggles `hidden` on the click; defer one tick so we see the
      // post-click visibility state.
      tab.addEventListener("click", () => setTimeout(refreshActive, 0));
    }
  }

  // ==========================================================================
  // Bootstrap B: Per-report tab strip. Each report page declares
  // `const REPORT_ID = "..."` in an inline script and has a #reportOutput +
  // #reportControls. We inject a `.tabs` bar above the form with a default
  // "Report" tab plus one tab per applicable setup section (Business /
  // Employee / Suppliers). Clicking a setup tab hides the report form +
  // output and reveals an injected pane with that section's editor.
  //
  // NB: REPORT_ID is a top-level `const` in the report HTML, so it does NOT
  // attach to `window` — we have to read it via the bare identifier with a
  // `typeof` guard (same pattern app.js uses).
  // ==========================================================================

  const reportId = (typeof REPORT_ID !== "undefined") ? REPORT_ID : null;
  const reportControls = document.getElementById("reportControls");
  const reportOutput = document.getElementById("reportOutput");
  const setupSpec = reportId && REPORT_SETUP[reportId];
  if (reportControls && reportOutput && setupSpec) {
    injectReportSetupTabs(setupSpec);
  }

  function injectReportSetupTabs(spec) {
    const TAB_DEFS = [
      { key: "report",       label: "Report",        enabled: true },
      { key: "business",     label: "Business",      enabled: !!spec.business },
      { key: "employee",     label: "Employee",      enabled: !!spec.employee },
      { key: "suppliers",    label: "Suppliers",     enabled: !!spec.suppliers },
      { key: "payslipItems", label: "Payslip items", enabled: !!spec.payslipItems },
    ].filter(t => t.enabled);

    // Tab strip — uses a custom data attribute so app.js's index-page tab
    // logic (which keys off `data-view` and is gated on #setup-view existing)
    // can't accidentally fight us.
    const tabs = document.createElement("div");
    tabs.className = "tabs cf-report-tabs";
    tabs.innerHTML = TAB_DEFS.map((t, i) =>
      `<button type="button" class="tab${i === 0 ? " active" : ""}" data-cf-view="${t.key}">${escapeText(t.label)}</button>`
    ).join("");
    reportControls.parentNode.insertBefore(tabs, reportControls);

    // Setup panes go just after #reportOutput. Each is hidden by default.
    const panes = {};
    for (const t of TAB_DEFS) {
      if (t.key === "report") continue;
      const pane = document.createElement("div");
      pane.className = "cf-tab-pane";
      pane.dataset.cfView = t.key;
      pane.hidden = true;
      reportOutput.parentNode.insertBefore(pane, reportOutput.nextSibling);
      panes[t.key] = pane;
    }

    // Mount each section into its pane (controllers are lazy — refresh runs
    // the first time the tab activates, and on every business change).
    const controllers = {};
    if (spec.business) {
      controllers.business = mountBusinessSection(panes.business, {
        intro: `<p class="muted">Edit business-level fields used by this report. Changes apply on next Generate.</p>`,
      });
    }
    if (spec.employee) {
      const followId = spec.employee.startsWith("follow:") ? spec.employee.slice("follow:".length) : null;
      controllers.employee = mountEmployeeSection(panes.employee, {
        followSelectId: followId,
        intro: followId
          ? `<p class="muted">Edit the STP fields of the employee selected on the Report tab.</p>`
          : `<p class="muted">Pick an employee to edit their STP fields.</p>`,
      });
    }
    if (spec.suppliers) {
      controllers.suppliers = mountSuppliersSection(panes.suppliers, {
        intro: `<p class="muted">Toggle which suppliers are included in TPAR and edit their ABN. Saves one row at a time.</p>`,
      });
    }
    if (spec.payslipItems) {
      controllers.payslipItems = mountPayslipItemsSection(panes.payslipItems, {
        intro: `<p class="muted">Assign each payslip item to a reporting category so its values flow into the right rows of this report.</p>`,
      });
    }

    const loaded = {}; // tab key → bool, tracks first-time fetch

    function activate(view) {
      for (const btn of tabs.querySelectorAll(".tab")) {
        btn.classList.toggle("active", btn.dataset.cfView === view);
      }
      const onReport = view === "report";
      reportControls.hidden = !onReport;
      reportOutput.hidden = !onReport;
      for (const [name, pane] of Object.entries(panes)) {
        pane.hidden = name !== view;
      }
      if (!onReport && controllers[view] && !loaded[view]) {
        loaded[view] = true;
        controllers[view].refresh();
      }
    }

    for (const btn of tabs.querySelectorAll(".tab")) {
      btn.addEventListener("click", () => activate(btn.dataset.cfView));
    }

    // When the business changes (rare on a report page — bar is hidden — but
    // can fire async from app.js's parent-frame lookup), drop the "loaded"
    // marks so the next activation re-fetches. If a setup tab is currently
    // showing, refresh it immediately.
    if (businessSelect) {
      businessSelect.addEventListener("change", () => {
        for (const name of Object.keys(controllers)) {
          loaded[name] = false;
          if (!panes[name].hidden) {
            loaded[name] = true;
            controllers[name].refresh();
          }
        }
      });
    }
  }
})();
