// Chart of Accounts Builder — country-agnostic extension.
//
// Lets the user pick a business, choose a preset profit & loss structure,
// preview it against what already exists, and create the missing groups,
// subtotals and accounts in one click. Depends on global CHART_PRESETS.
//
// Endpoints used (the browser session carries auth; Business is passed
// explicitly so the page also works when opened directly):
//   GET  /api4/businesses
//   GET  /api4/profit-and-loss-statement-group-batch
//   GET  /api4/profit-and-loss-statement-account-batch
//   GET  /api4/subtotal-batch
//   POST /api4/profit-and-loss-statement-group
//   POST /api4/profit-and-loss-statement-account
//   POST /api4/subtotal

function escapeText(s) { return String(s).replace(/&/g, "&amp;").replace(/</g, "&lt;"); }
function escapeAttr(s) { return String(s).replace(/&/g, "&amp;").replace(/"/g, "&quot;"); }
function normName(s) { return String(s || "").trim().toLowerCase(); }

const businessSelect = document.getElementById("business");
const presetSelect = document.getElementById("preset");
const previewEl = document.getElementById("preview");
const summaryEl = document.getElementById("summary");
const buildBtn = document.getElementById("buildBtn");

// Current chart of accounts in the selected business: { groups, accounts,
// subtotals }, each a list of { key, name }. null until a business is loaded.
let state = null;

// ---- Tab switching ---------------------------------------------------------
// Each <button class="tab" data-view="X"> pairs with a <div id="X-view">.
const allViews = document.querySelectorAll('[id$="-view"]');
for (const tab of document.querySelectorAll(".tab")) {
  tab.addEventListener("click", () => {
    for (const t of document.querySelectorAll(".tab")) t.classList.remove("active");
    tab.classList.add("active");
    for (const view of allViews) view.hidden = view.id !== `${tab.dataset.view}-view`;
  });
}

// ---- Business dropdown -----------------------------------------------------
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
  } catch {
    businessSelect.innerHTML = `<option value="">(error)</option>`;
  }
})();

businessSelect.addEventListener("change", loadState);
presetSelect.addEventListener("change", render);
buildBtn.addEventListener("click", build);

// ---- Load current chart of accounts ----------------------------------------

async function fetchBatch(route, business) {
  const res = await fetch(`/api4/${route}?business=${encodeURIComponent(business)}&pageSize=500`, { credentials: "include" });
  if (!res.ok) throw new Error(`HTTP ${res.status}: ${await res.text()}`);
  const data = await res.json();
  // GET batch serializes each entity under the JSON key "item".
  return (data.items || []).map(it => ({ key: it.key, name: (it.item || {}).name || "" }));
}

async function loadState() {
  const business = businessSelect.value;
  if (!business) { state = null; render(); return; }
  previewEl.innerHTML = `<div class="status">Loading current chart of accounts…</div>`;
  summaryEl.textContent = "";
  buildBtn.disabled = true;
  try {
    const [groups, accounts, subtotals] = await Promise.all([
      fetchBatch("profit-and-loss-statement-group-batch", business),
      fetchBatch("profit-and-loss-statement-account-batch", business),
      fetchBatch("subtotal-batch", business),
    ]);
    state = { groups, accounts, subtotals };
  } catch (err) {
    state = null;
    previewEl.innerHTML = `<div class="error">Failed to load: ${escapeText(err.message)}</div>`;
    return;
  }
  render();
}

// ---- Lookups & positions ---------------------------------------------------

function findGroup(name) { return state && state.groups.find(g => normName(g.name) === normName(name)); }
function findAccount(name) { return state && state.accounts.find(a => normName(a.name) === normName(name)); }
function findSubtotal(name) { return state && state.subtotals.find(s => normName(s.name) === normName(name)); }
function groupById(preset, id) { return preset.groups.find(g => g.id === id); }

// A top-level group / subtotal takes its Position from its slot in `sequence`;
// a subgroup takes its Position from its order among siblings of the same parent.
function groupPosition(g, preset) {
  if (g.type === 2) {
    return preset.groups.filter(x => x.type === 2 && x.parent === g.parent).indexOf(g);
  }
  return preset.sequence.findIndex(s => s.group === g.id);
}
function accountPosition(a, preset) {
  return preset.accounts.filter(x => x.group === a.group).indexOf(a);
}

// ---- Preview ---------------------------------------------------------------

function statusBadge(exists) {
  return exists
    ? `<span class="badge exists">Exists</span>`
    : `<span class="badge new">New</span>`;
}

function nodeRow(name, level, isGroup, exists) {
  const cls = `coa-row level-${level} ${isGroup ? "coa-group" : "coa-account"}`;
  return `<div class="${cls}"><span class="coa-name">${escapeText(name)}</span>${statusBadge(exists)}</div>`;
}

function subtotalRow(name, exists) {
  return `<div class="coa-row coa-subtotal"><span class="coa-name">${escapeText(name)}</span>${statusBadge(exists)}</div>`;
}

function render() {
  const preset = CHART_PRESETS[presetSelect.value];
  document.getElementById("presetDescription").textContent = preset.description;

  if (!state) {
    previewEl.innerHTML = `<p class="muted">Select a business to preview the chart of accounts.</p>`;
    summaryEl.textContent = "";
    buildBtn.disabled = true;
    return;
  }

  let html = "";
  let newGroups = 0, newAccounts = 0, newSubtotals = 0;

  const accountsHtml = (groupId, level) => {
    let out = "";
    for (const acc of preset.accounts.filter(a => a.group === groupId)) {
      const exists = !!findAccount(acc.name);
      if (!exists) newAccounts++;
      out += nodeRow(acc.name, level, false, exists);
    }
    return out;
  };

  for (const item of preset.sequence) {
    if (item.subtotal) {
      const exists = !!findSubtotal(item.subtotal);
      if (!exists) newSubtotals++;
      html += subtotalRow(item.subtotal, exists);
      continue;
    }
    const top = groupById(preset, item.group);
    const topExists = !!findGroup(top.name);
    if (!topExists) newGroups++;
    html += nodeRow(top.name, 0, true, topExists);

    for (const sub of preset.groups.filter(g => g.type === 2 && g.parent === top.id)) {
      const subExists = !!findGroup(sub.name);
      if (!subExists) newGroups++;
      html += nodeRow(sub.name, 1, true, subExists);
      html += accountsHtml(sub.id, 2);
    }
    // Accounts filed directly under a top-level group (no subgroup).
    html += accountsHtml(top.id, 1);
  }

  previewEl.innerHTML = `<div class="coa-tree">${html}</div>`;

  if (newGroups + newSubtotals + newAccounts === 0) {
    summaryEl.textContent = "Everything in this structure already exists in the business.";
    buildBtn.disabled = true;
  } else {
    const parts = [];
    if (newGroups) parts.push(`${newGroups} group${newGroups === 1 ? "" : "s"}`);
    if (newSubtotals) parts.push(`${newSubtotals} subtotal${newSubtotals === 1 ? "" : "s"}`);
    if (newAccounts) parts.push(`${newAccounts} account${newAccounts === 1 ? "" : "s"}`);
    summaryEl.textContent = `Will create ${parts.join(", ")}. Existing items are left untouched.`;
    buildBtn.disabled = false;
  }
}

// ---- Build -----------------------------------------------------------------

async function postObject(route, business, value) {
  const res = await fetch(`/api4/${route}`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ Business: business, Value: value }),
    credentials: "include",
  });
  if (!res.ok) throw new Error(`HTTP ${res.status}: ${await res.text()}`);
  return await res.json(); // the created object's key (Guid)
}

async function build() {
  const business = businessSelect.value;
  const preset = CHART_PRESETS[presetSelect.value];
  if (!business) return;

  buildBtn.disabled = true;
  buildBtn.textContent = "Building…";
  try {
    // Resolve each preset group id to a real key — reusing an existing group
    // when one matches by name, otherwise creating it. Top-level groups are
    // created before subgroups, which reference their parent's key.
    const keyByPresetId = {};
    const ordered = [
      ...preset.groups.filter(g => g.type !== 2),
      ...preset.groups.filter(g => g.type === 2),
    ];
    for (const g of ordered) {
      const existing = findGroup(g.name);
      if (existing) { keyByPresetId[g.id] = existing.key; continue; }
      const value = { Name: g.name, Type: g.type, Position: groupPosition(g, preset) };
      if (g.type === 2) value.Group = keyByPresetId[g.parent];
      keyByPresetId[g.id] = await postObject("profit-and-loss-statement-group", business, value);
    }

    // Subtotals share the top-level Position space with the groups.
    for (let i = 0; i < preset.sequence.length; i++) {
      const item = preset.sequence[i];
      if (!item.subtotal || findSubtotal(item.subtotal)) continue;
      await postObject("subtotal", business, { Name: item.subtotal, Position: i });
    }

    for (const a of preset.accounts) {
      if (findAccount(a.name)) continue;
      const value = { Name: a.name, Group: keyByPresetId[a.group], Position: accountPosition(a, preset) };
      await postObject("profit-and-loss-statement-account", business, value);
    }

    await loadState(); // refresh badges — created items now show as "Exists"
  } catch (err) {
    previewEl.insertAdjacentHTML("beforebegin", `<div class="error">Build failed: ${escapeText(err.message)}</div>`);
  } finally {
    buildBtn.textContent = "Build chart of accounts";
  }
}

render();
