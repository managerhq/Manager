// Pre-defined Slovakia tax codes.
//
// 2025-01-01 rate change: the basic VAT rate moved from 20 % to 23 %, the reduced rate
// from 10 % to 19 %, and a new super-reduced 5 % rate was introduced (zákon č. 366/2024).
//
// The 23 % / 19 % / 5 % codes plus the reverse-charge / exempt codes below are linked
// to the reporting-category Guids declared as `C.R##_*` in reports.js so the DPH
// return produces real figures. The wiring follows the same shape as other countries'
// VAT codes (see wwwroot/extensions/gb/tax-codes.js for the canonical reverse-charge
// pattern).
//
// Mapping summary (form riadky):
//   Sales:        R01/R02 (23 %), R03/R04 (19 %), R05/R06 (5 %)
//                 R15 EU dodanie, R16 vývoz, R17 trojstranný obchod, R18 iné oslobodené
//   Purchases:    R19 odpočet z tuzemských prijatých plnení (all rates)
//   Reverse chg:  R07/R08 + R20 (acq. EÚ 23 %), R09/R10 + R20 (acq. EÚ 19 %),
//                 R11/R12 + R22 (tuzemský prenos / služby z EÚ 23 %),
//                 R13/R14 + R21 (dovoz tovaru 23 %)

const TAX_CODE_TEMPLATES = [
  // --- Current rates (2025+) --------------------------------------------------
  {
    "Name": "DPH 23% Predaj", "Label": "DPH 23%", "TaxRate": 2, "Rate": 23.0,
    "ReportingCategory":         "5ca8164e-c942-4b5b-b061-890d09c624c1", // R01_NET_STD
    "TaxAmountReportingCategory":"cef9db6d-8236-461d-837c-752cad7ff2a9", // R02_TAX_STD
  },
  {
    "Name": "DPH 23% Nákup", "Label": "DPH 23%", "TaxRate": 2, "Rate": 23.0,
    "TaxAmountReportingCategory":"295b0790-efc9-4893-bcbd-95ef2003d104", // R19_DED_DOM
  },
  {
    "Name": "DPH 19% Predaj", "Label": "DPH 19%", "TaxRate": 2, "Rate": 19.0,
    "ReportingCategory":         "a0de535b-05e6-46de-9cd0-6d08035c2789", // R03_NET_RED
    "TaxAmountReportingCategory":"39fd8fc3-0646-456e-8cce-be68263678bd", // R04_TAX_RED
  },
  {
    "Name": "DPH 19% Nákup", "Label": "DPH 19%", "TaxRate": 2, "Rate": 19.0,
    "TaxAmountReportingCategory":"295b0790-efc9-4893-bcbd-95ef2003d104", // R19_DED_DOM
  },
  {
    "Name": "DPH 5% Predaj", "Label": "DPH 5%", "TaxRate": 2, "Rate": 5.0,
    "ReportingCategory":         "7422dfe9-608e-41d9-9f97-d5136b0a252a", // R05_NET_SUP
    "TaxAmountReportingCategory":"228766ed-a0f4-4f5b-b255-e6ac3732e07d", // R06_TAX_SUP
  },
  {
    "Name": "DPH 5% Nákup", "Label": "DPH 5%", "TaxRate": 2, "Rate": 5.0,
    "TaxAmountReportingCategory":"295b0790-efc9-4893-bcbd-95ef2003d104", // R19_DED_DOM
  },

  // --- Reverse-charge (samozdanenie) ------------------------------------------
  // Buyer self-assesses VAT. The same transaction produces both an output entry
  // (R07/R08 etc.) and a deductible input entry (R20/R21/R22) — TaxAmount* points
  // at the output side and TaxAmountReversed* at the input deduction side; the
  // ReverseCharged flag tells Manager to post both legs.
  {
    "Name": "Nadobudnutie tovaru z EÚ 23%", "Label": "EÚ 23%", "TaxRate": 2, "Rate": 23.0,
    "ReportingCategory":                  "f9e60f16-3d61-4a02-b313-0e581aacbb1a", // R07_NET_ACQ_STD
    "TaxAmountReportingCategory":         "4266fac6-4ee5-4627-99b5-56fe4009eb9a", // R08_TAX_ACQ_STD
    "TaxAmountReversedReportingCategory": "fcf95d39-3775-4455-b53d-cc5099ef2ae3", // R20_DED_ACQ
    "ReverseCharged": true,
  },
  {
    "Name": "Nadobudnutie tovaru z EÚ 19%", "Label": "EÚ 19%", "TaxRate": 2, "Rate": 19.0,
    "ReportingCategory":                  "b82e02d3-879d-482c-9844-409960c9bdf2", // R09_NET_ACQ_RED
    "TaxAmountReportingCategory":         "87b64e78-e792-44a0-a7db-ad90a3bbb774", // R10_TAX_ACQ_RED
    "TaxAmountReversedReportingCategory": "fcf95d39-3775-4455-b53d-cc5099ef2ae3", // R20_DED_ACQ
    "ReverseCharged": true,
  },
  {
    "Name": "Tuzemský prenos daňovej povinnosti 23%", "Label": "Prenos 23%", "TaxRate": 2, "Rate": 23.0,
    "ReportingCategory":                  "e0113abf-43b8-41d5-9b26-850cb337a0a3", // R11_NET_RC
    "TaxAmountReportingCategory":         "6e01b47a-e265-4d56-a83f-a679d442ea48", // R12_TAX_RC
    "TaxAmountReversedReportingCategory": "f88543d2-7c8b-4c07-acb4-11fe26c8ea40", // R22_DED_RC
    "ReverseCharged": true,
  },
  {
    "Name": "Prijatie služby z EÚ 23%", "Label": "Služba EÚ 23%", "TaxRate": 2, "Rate": 23.0,
    "ReportingCategory":                  "e0113abf-43b8-41d5-9b26-850cb337a0a3", // R11_NET_RC
    "TaxAmountReportingCategory":         "6e01b47a-e265-4d56-a83f-a679d442ea48", // R12_TAX_RC
    "TaxAmountReversedReportingCategory": "f88543d2-7c8b-4c07-acb4-11fe26c8ea40", // R22_DED_RC
    "ReverseCharged": true,
  },
  {
    "Name": "Dovoz tovaru 23%", "Label": "Dovoz 23%", "TaxRate": 2, "Rate": 23.0,
    "ReportingCategory":                  "12ca390f-036a-4c48-b520-78c9d78b1cff", // R13_NET_IMP
    "TaxAmountReportingCategory":         "df950da9-585e-442b-8994-072bd74e4384", // R14_TAX_IMP
    "TaxAmountReversedReportingCategory": "2dcd683e-3af2-4bdf-90b8-8266fe18a50b", // R21_DED_IMP
    "ReverseCharged": true,
  },

  // --- Oslobodené dodania (0 % codes) -----------------------------------------
  // No tax amount. Net basis only — appears on output side of the form.
  {
    "Name": "Dodanie do EÚ (§ 43)", "Label": "EÚ 0%",
    "ReportingCategory": "054fcea9-e320-442c-b48a-11d1b52e273f", // R15_EU_SUP
  },
  {
    "Name": "Vývoz tovaru (§ 47)", "Label": "Vývoz 0%",
    "ReportingCategory": "66f40101-fd4b-4ad4-bce6-df6f451e3204", // R16_EXPORT
  },
  {
    "Name": "Trojstranný obchod (§ 45)", "Label": "Trojstr.",
    "ReportingCategory": "c6253654-42d7-4940-80aa-290b555eb4fd", // R17_TRIANG
  },
  {
    "Name": "Iné oslobodené dodania", "Label": "Oslob.",
    "ReportingCategory": "7592f668-3ac7-4f0c-ba78-cd4b00fed9ee", // R18_OTHER_EX
  }
];
