// Slovak DPH (VAT) report transformation — daňové priznanie k dani z pridanej hodnoty.
//
// The reporting-category Guids declared below in `C = { ... }` are real UUIDs that the
// SK tax codes (tax-codes.js) point at via ReportingCategory / ReportingCategoryReversed /
// TaxAmountReportingCategory / TaxAmountReversedReportingCategory. Once a tax code is
// installed in a business (via the "Daňové kódy" tab on the country index) and used on
// transactions, the corresponding form rows below will sum those amounts.
//
// XML export is a structural stub. The real eDane FDF schema published by Finančná
// správa SR uses a specific XML namespace and element names; the customScript below
// emits a plain <DanovePriznanie> document with one <Riadok cislo="NN"> per form row.
// The exact eDane mapping will need a sample submission file or the XSD to be exact.

const C = {
  // II. časť — Vystavené daňové doklady (output side)
  R01_NET_STD:  "5ca8164e-c942-4b5b-b061-890d09c624c1", // Dodanie tovaru/služby v tuzemsku, základná sadzba — základ
  R02_TAX_STD:  "cef9db6d-8236-461d-837c-752cad7ff2a9", // ...daň
  R03_NET_RED:  "a0de535b-05e6-46de-9cd0-6d08035c2789", // Dodanie tovaru/služby v tuzemsku, znížená sadzba — základ
  R04_TAX_RED:  "39fd8fc3-0646-456e-8cce-be68263678bd", // ...daň
  R05_NET_SUP:  "7422dfe9-608e-41d9-9f97-d5136b0a252a", // Dodanie tovaru/služby, super-znížená sadzba — základ
  R06_TAX_SUP:  "228766ed-a0f4-4f5b-b255-e6ac3732e07d", // ...daň
  R07_NET_ACQ_STD: "f9e60f16-3d61-4a02-b313-0e581aacbb1a", // Nadobudnutie tovaru z EÚ, základná — základ
  R08_TAX_ACQ_STD: "4266fac6-4ee5-4627-99b5-56fe4009eb9a", // ...daň
  R09_NET_ACQ_RED: "b82e02d3-879d-482c-9844-409960c9bdf2", // Nadobudnutie tovaru z EÚ, znížená — základ
  R10_TAX_ACQ_RED: "87b64e78-e792-44a0-a7db-ad90a3bbb774", // ...daň
  R11_NET_RC:   "e0113abf-43b8-41d5-9b26-850cb337a0a3", // Prenos daňovej povinnosti (samozdanenie) — základ
  R12_TAX_RC:   "6e01b47a-e265-4d56-a83f-a679d442ea48", // ...daň
  R13_NET_IMP:  "12ca390f-036a-4c48-b520-78c9d78b1cff", // Dovoz tovaru — základ
  R14_TAX_IMP:  "df950da9-585e-442b-8994-072bd74e4384", // ...daň
  R15_EU_SUP:   "054fcea9-e320-442c-b48a-11d1b52e273f", // Dodanie tovaru do EÚ oslobodené (§ 43)
  R16_EXPORT:   "66f40101-fd4b-4ad4-bce6-df6f451e3204", // Vývoz tovaru (§ 47)
  R17_TRIANG:   "c6253654-42d7-4940-80aa-290b555eb4fd", // Trojstranný obchod — dodávky druhému odberateľovi
  R18_OTHER_EX: "7592f668-3ac7-4f0c-ba78-cd4b00fed9ee", // Iné oslobodené dodania

  // III. časť — Odpočet dane (input side)
  R19_DED_DOM:  "295b0790-efc9-4893-bcbd-95ef2003d104", // Odpočet dane z prijatých plnení od platiteľa
  R20_DED_ACQ:  "fcf95d39-3775-4455-b53d-cc5099ef2ae3", // Odpočet dane pri nadobudnutí tovaru z EÚ
  R21_DED_IMP:  "2dcd683e-3af2-4bdf-90b8-8266fe18a50b", // Odpočet dane pri dovoze
  R22_DED_RC:   "f88543d2-7c8b-4c07-acb4-11fe26c8ea40", // Odpočet dane pri samozdanení (riadok 12)
  R23_DED_PROP: "b6b898d9-b555-4d42-acad-e3bb57775595", // Pomerné odpočítanie dane
};

// Special markers used by report-transformation-figures. Subtracts the previous figure(s).
const REVERSE_SIGN = "0b3fe333-755b-42c0-b921-2835e39e50f0";

const REPORTS = [{
  id: "b16afdac-1570-4f6d-9bd1-09feaeb966ed",
  name: "Priznanie k DPH",
  columns: 1,
  items: [
    { Name: "II. časť — Daň z dodania tovarov a služieb\n----" },
    { Name: "01 — Dodanie tovaru a služby v tuzemsku, základná sadzba — základ dane", Column1: [C.R01_NET_STD] },
    { Name: "02 — Dodanie tovaru a služby v tuzemsku, základná sadzba — daň",         Column1: [C.R02_TAX_STD] },
    { Name: "03 — Dodanie tovaru a služby v tuzemsku, znížená sadzba — základ dane",  Column1: [C.R03_NET_RED] },
    { Name: "04 — Dodanie tovaru a služby v tuzemsku, znížená sadzba — daň",          Column1: [C.R04_TAX_RED] },
    { Name: "05 — Dodanie tovaru a služby, super-znížená sadzba — základ dane",       Column1: [C.R05_NET_SUP] },
    { Name: "06 — Dodanie tovaru a služby, super-znížená sadzba — daň",               Column1: [C.R06_TAX_SUP] },
    { Name: "07 — Nadobudnutie tovaru v tuzemsku z iného členského štátu — základ (základná sadzba)", Column1: [C.R07_NET_ACQ_STD] },
    { Name: "08 — Nadobudnutie tovaru v tuzemsku z iného členského štátu — daň (základná sadzba)",    Column1: [C.R08_TAX_ACQ_STD] },
    { Name: "09 — Nadobudnutie tovaru v tuzemsku z iného členského štátu — základ (znížená sadzba)",  Column1: [C.R09_NET_ACQ_RED] },
    { Name: "10 — Nadobudnutie tovaru v tuzemsku z iného členského štátu — daň (znížená sadzba)",     Column1: [C.R10_TAX_ACQ_RED] },
    { Name: "11 — Tovar a služby, pri ktorých je povinný platiť daň príjemca — základ", Column1: [C.R11_NET_RC] },
    { Name: "12 — Tovar a služby, pri ktorých je povinný platiť daň príjemca — daň",    Column1: [C.R12_TAX_RC] },
    { Name: "13 — Dovoz tovaru — základ dane", Column1: [C.R13_NET_IMP] },
    { Name: "14 — Dovoz tovaru — daň",         Column1: [C.R14_TAX_IMP] },
    { Name: "15 — Dodanie tovaru do iného členského štátu oslobodené (§ 43)", Column1: [C.R15_EU_SUP] },
    { Name: "16 — Vývoz tovaru (§ 47)",                                       Column1: [C.R16_EXPORT] },
    { Name: "17 — Trojstranný obchod — dodávky druhému odberateľovi (§ 45)",  Column1: [C.R17_TRIANG] },
    { Name: "18 — Iné oslobodené dodania",                                    Column1: [C.R18_OTHER_EX] },

    { Name: "III. časť — Odpočet dane\n----" },
    { Name: "19 — Odpočet dane z prijatých plnení od platiteľa",       Column1: [C.R19_DED_DOM] },
    { Name: "20 — Odpočet dane pri nadobudnutí tovaru z EÚ",           Column1: [C.R20_DED_ACQ] },
    { Name: "21 — Odpočet dane pri dovoze tovaru",                     Column1: [C.R21_DED_IMP] },
    { Name: "22 — Odpočet dane pri samozdanení (riadok 12)",           Column1: [C.R22_DED_RC] },
    { Name: "23 — Pomerné odpočítanie dane",                           Column1: [C.R23_DED_PROP] },
    { Name: "24 — Odpočet dane spolu (súčet 19–23)",
      Column1: [C.R19_DED_DOM, C.R20_DED_ACQ, C.R21_DED_IMP, C.R22_DED_RC, C.R23_DED_PROP] },

    { Name: "IV. časť — Výsledok\n----" },
    { Name: "25 — Daňová povinnosť (súčet R02+R04+R06+R08+R10+R12+R14)",
      Column1: [C.R02_TAX_STD, C.R04_TAX_RED, C.R06_TAX_SUP, C.R08_TAX_ACQ_STD, C.R10_TAX_ACQ_RED, C.R12_TAX_RC, C.R14_TAX_IMP] },
    { Name: "26 — Vlastná daňová povinnosť alebo nadmerný odpočet (R25 − R24)",
      Column1: [C.R02_TAX_STD, C.R04_TAX_RED, C.R06_TAX_SUP, C.R08_TAX_ACQ_STD, C.R10_TAX_ACQ_RED, C.R12_TAX_RC, C.R14_TAX_IMP,
                C.R19_DED_DOM, C.R20_DED_ACQ, C.R21_DED_IMP, C.R22_DED_RC, C.R23_DED_PROP, REVERSE_SIGN] },
  ],
  // XML export — conforms to the eDane DPH 2025 schema published by Finančná správa SR
  // at https://ekr.financnasprava.sk/Formulare/XSD/dph2025.xsd
  //
  // Structure: <dokument><hlavicka>...</hlavicka><telo>...</telo></dokument>
  // - Body rows are typed `optDec` (decimal, totalDigits=12, fractionDigits=2, period
  //   as decimal separator). Empty rows are simply omitted.
  // - splneniePodmienok is required (minOccurs="1") — we emit "0".
  // - Header identification fields (DIC, IČ DPH, daňový úrad, adresa, meno…) are emitted
  //   as empty placeholders. eDane will surface them on import so the user can fill them
  //   in there, or a future iteration can add HTML inputs to this report page that feed
  //   into the export.
  // - Period (mesiac/štvrťrok + rok) is pulled from the form selector at the top of the
  //   report page so the export already carries the right zdaňovacie obdobie.
  // - Form rows r01–r23 map 1:1 to the report items in this same order; r24/r25/r26 are
  //   the computed totals shown at the bottom of the report. Sub-rows (r01a, r05a,
  //   r09a/b, r11a–e, r12a–e, etc.) are not produced because the underlying tax codes
  //   don't break out those sub-categories yet — they can be added later by introducing
  //   more granular reporting categories without changing the export structure.
  customScript: `
var rows = document.querySelectorAll('#reportOutput tbody tr');
var values = [];
for (var i = 0; i < rows.length; i++) {
  if (rows[i].querySelector('th')) continue;
  var node = rows[i].querySelector('span[data-value], a[data-value]');
  var raw = node ? parseFloat(node.dataset.value) : 0;
  values.push(isNaN(raw) ? 0 : raw);
}

// Order must match the items[] above (data rows only, skipping section headers).
var FORM_ROWS = [
  'r01','r02','r03','r04','r05','r06',          // 23% / 19% / 5% domestic supplies
  'r07','r08','r09','r10',                      // EU acquisitions (std + reduced)
  'r11','r12',                                  // reverse-charge inputs
  'r13','r14',                                  // import
  'r15','r16','r17','r18',                      // exempt supplies, export, triangulation, other
  'r19','r20','r21','r22','r23',                // input VAT deductions
  'r24','r25','r26'                             // computed totals
];

// Period from the form selectors (set in priznanie-k-dph.html).
var pt = (document.getElementById('periodType') || {}).value || '';
var year = (document.getElementById('year') || {}).value || '';
var mesiac = '';
var stvrtrok = '';
if (pt === 'month') {
  var m = parseInt((document.getElementById('month') || {}).value, 10);
  if (!isNaN(m)) mesiac = (m + 1 < 10 ? '0' : '') + (m + 1);
} else if (pt === 'quarter') {
  var q = parseInt((document.getElementById('quarter') || {}).value, 10);
  if (!isNaN(q)) stvrtrok = String(q + 1);
}

var bodyRows = [];
for (var j = 0; j < values.length && j < FORM_ROWS.length; j++) {
  if (values[j] !== 0) {
    bodyRows.push('    <' + FORM_ROWS[j] + '>' + values[j].toFixed(2) + '</' + FORM_ROWS[j] + '>');
  }
}

var xml = [
  '<?xml version="1.0" encoding="UTF-8"?>',
  '<dokument>',
  '  <hlavicka>',
  '    <identifikacneCislo>',
  '      <kodStatu>SK</kodStatu>',
  '      <cislo></cislo>',
  '    </identifikacneCislo>',
  '    <dic></dic>',
  '    <danovyUrad></danovyUrad>',
  '    <nevzniklaPov>0</nevzniklaPov>',
  '    <typDP>',
  '      <rdp>1</rdp>',
  '      <odp>0</odp>',
  '      <ddp>0</ddp>',
  '      <datumZisteniaDdp></datumZisteniaDdp>',
  '    </typDP>',
  '    <osoba>',
  '      <platitel>1</platitel>',
  '      <registrovana>0</registrovana>',
  '      <inaPovinna>0</inaPovinna>',
  '      <zdanitelna>0</zdanitelna>',
  '      <zastupca>0</zastupca>',
  '      <zastupca69aa>0</zastupca69aa>',
  '    </osoba>',
  '    <zdanObd>',
  '      <mesiac>' + mesiac + '</mesiac>',
  '      <stvrtrok>' + stvrtrok + '</stvrtrok>',
  '      <rok>' + year + '</rok>',
  '    </zdanObd>',
  '    <meno><riadok></riadok></meno>',
  '    <adresa>',
  '      <ulica></ulica>',
  '      <cislo></cislo>',
  '      <psc></psc>',
  '      <obec></obec>',
  '      <telefon></telefon>',
  '      <email></email>',
  '    </adresa>',
  '    <opravnenaOsoba>',
  '      <menoPriezvisko></menoPriezvisko>',
  '      <telefon></telefon>',
  '      <email></email>',
  '    </opravnenaOsoba>',
  '    <datumVyhlasenia></datumVyhlasenia>',
  '  </hlavicka>',
  '  <telo>'
].concat(bodyRows).concat([
  '    <splneniePodmienok>0</splneniePodmienok>',
  '  </telo>',
  '</dokument>',
  ''
]).join('\\n');

document.getElementsByName('content')[0].value = xml;

// After app.js finishes adding the Download CSV button to .report-actions on this
// same synchronous tick, rename it to "Download XML" and lift the action bar out of
// #reportOutput so the Print/Download buttons sit above the green tlačivo panel,
// not inside it. setTimeout(..., 0) waits one tick — by then app.js has appended
// the download button.
//
// Before relocating the new bar, remove any previously-relocated bars from earlier
// "Vygenerovať" clicks — app.js regenerates only the inside of #reportOutput, so
// without this cleanup each click would leave its old action bar floating above
// the form, accumulating copies.
setTimeout(function () {
  var actions = document.querySelector('#reportOutput .report-actions');
  if (!actions) return;
  var btns = actions.querySelectorAll('button');
  for (var k = 0; k < btns.length; k++) {
    if (btns[k].textContent.indexOf('CSV') !== -1) btns[k].textContent = 'Download XML';
  }
  var stale = document.querySelectorAll('.report-actions');
  for (var k = 0; k < stale.length; k++) {
    if (stale[k] !== actions && stale[k].parentNode) stale[k].parentNode.removeChild(stale[k]);
  }
  var output = document.getElementById('reportOutput');
  if (output && output.parentNode) output.parentNode.insertBefore(actions, output);
}, 0);
  `,
  csvFilename: "priznanie-k-dph.xml",
}];
