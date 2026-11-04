// Per-report presentation for the AU extension, keyed by the report ids in reports.js.
//
// reports.js says what a report *computes* (its rows, and the reporting categories behind
// each one). A report's controls come from its own page's form, which app.js reuses in
// both places it runs the report. What is left for a view here is how the result is
// displayed, and only for the reports where a plain table is not it:
//
//   hideZeroRows - drop all-zero rows from the table. Markup is still emitted (just
//                  with [hidden]) so exportCsv can still read the cells' data-values.
//   render       - replaces the generic table renderer; owns the whole #reportOutput.
//   exportCsv    - fills textarea[name="content"] with CSV. app.js then adds a
//                  Download CSV button named by csvFilename.
//
// A report with nothing to say here needs no entry: BAS and TPAR render as the generic
// table, on their own page and in the country index alike.

const REPORT_VIEWS = {

  // ---- PAYG payment summary — individual non-business ---------------------
  "92b38154-38fc-479a-a296-2019f656d1e2": {
    // Form-style renderer mirroring the layout of the official ATO paper form
    // (NAT 0046) rather than the generic table the shared renderer produces.
    render: function (report, data, business, period) {
      // The API returns rows in source order; rebuild sections by walking until we
      // hit a header. "Name" appears once in each section, so a flat map by row name
      // would clobber the payee name with the payer name.
      const sections = {};
      let currentSection = null;
      for (const row of data.rows || []) {
        if (row.isHeader) {
          currentSection = (row.name || "").trim();
          sections[currentSection] = {};
        } else if (currentSection) {
          sections[currentSection][row.name] = (row.cells && row.cells[0]) || null;
        }
      }
      const payee = sections["Payee details"] || {};
      const payer = sections["Payer details"] || {};
      const decimals = data.decimals != null ? data.decimals : 2;

      const cellText = (cell) => (cell && cell.text) ? cell.text : "";
      const cellAmount = (cell, dp) => {
        if (!cell || cell.number == null) return "";
        const d = dp != null ? dp : decimals;
        return Number(cell.number).toLocaleString(undefined, { minimumFractionDigits: d, maximumFractionDigits: d });
      };

      // ATO form formats dates as DD/MM/YYYY in three boxes; we use a single value.
      const fmtFormDate = (iso) => {
        if (!iso) return "";
        const parts = iso.split("-");
        return parts.length === 3 ? `${parts[2]} / ${parts[1]} / ${parts[0]}` : iso;
      };
      // "Payment summary for year ending 30 June YYYY" uses the FY end year.
      const endYear = (period.to || "").substring(0, 4);

      const textField = (label, value) => `
        <div class="payg-field">
          <label>${escapeText(label)}</label>
          <div class="payg-value${value ? "" : " empty"}">${value ? escapeText(value) : "&nbsp;"}</div>
        </div>`;
      const amountField = (label, amount) => `
        <div class="payg-field">
          <label>${escapeText(label)}</label>
          <div class="payg-value num${amount ? "" : " empty"}">${amount ? "$ " + escapeText(amount) : "&nbsp;"}</div>
        </div>`;

      const totalTax = cellAmount(payee["Total tax withheld"], 0);

      const html = `
        <div class="report-actions"><button type="button" class="primary" id="printReport">Print</button></div>
        <div id="printable-content" class="payg-form">

          <div class="payg-banner">
            <div class="payg-title">PAYG payment summary &ndash; individual non-business</div>
            <div class="payg-subtitle">Payment summary for year ending 30 June ${escapeText(endYear)}</div>
          </div>
          <div class="payg-notice">
            Section 16-170 of Schedule 1 to the <em>Taxation Administration Act 1953</em>.
            This summary shows payments and tax withheld for the period stated. Keep this for your records.
          </div>

          <div class="payg-section">
            <div class="payg-section-title">Section A &mdash; Payee details</div>
            <div class="payg-section-body">
              <div class="payg-row cols-2">
                ${textField("Payee's tax file number", cellText(payee["TFN"]))}
                ${textField("Payee's name", cellText(payee["Name"]))}
              </div>
              <div class="payg-row cols-2">
                ${textField("Period of payment &mdash; day / month / year (from)", fmtFormDate(period.from))}
                ${textField("Period of payment &mdash; day / month / year (to)", fmtFormDate(period.to))}
              </div>
            </div>
          </div>

          <div class="payg-section">
            <div class="payg-section-title">Section B &mdash; Payment details</div>
            <div class="payg-section-body">
              <div class="payg-totalbox">
                <div class="label">Total tax withheld &mdash; do not show cents</div>
                <div class="amount">$ ${totalTax || "0"}</div>
              </div>
              <div class="payg-row cols-2">
                ${amountField("Gross payments", cellAmount(payee["Gross Payments"]))}
                ${amountField("CDEP payments", cellAmount(payee["CDEP payments"]))}
              </div>
              <div class="payg-row cols-2">
                ${amountField("Reportable employer superannuation contributions", cellAmount(payee["Reportable employer superannuation contributions"]))}
                ${amountField("Total allowances", cellAmount(payee["Total allowances"]))}
              </div>
              <div class="payg-subhead">Lump sum payments</div>
              <div class="payg-row cols-4">
                ${amountField("A", cellAmount(payee["Lump Sum Payment A"]))}
                ${amountField("B", cellAmount(payee["Lump Sum Payment B"]))}
                ${amountField("D", cellAmount(payee["Lump Sum Payment D"]))}
                ${amountField("E", cellAmount(payee["Lump Sum Payment E"]))}
              </div>
              <div class="payg-row cols-2">
                ${amountField("W &mdash; return to work payment", cellAmount(payee["Lump Sum Payment W"]))}
              </div>
            </div>
          </div>

          <div class="payg-section">
            <div class="payg-section-title">Section C &mdash; Payer details</div>
            <div class="payg-section-body">
              <div class="payg-row cols-2">
                ${textField("Payer's ABN or withholding payer number", cellText(payer["ABN"]))}
                ${textField("Payer's name", cellText(payer["Name"]))}
              </div>
              <div class="payg-row cols-2">
                ${textField("Signature of authorised person", "")}
                ${textField("Date", "")}
              </div>
            </div>
          </div>

        </div>`;

      const output = document.getElementById("reportOutput");
      output.innerHTML = html;
      output.querySelector("#printReport").addEventListener("click", () => window.print());
    },
  },

  // ---- Single Touch Payroll Worksheet (Phase 2) ---------------------------
  "07332ba3-3e82-4dc1-9451-1350f5d84e24": {
    hideZeroRows: true,
    csvFilename: "Single Touch Payroll Worksheet (Phase 2).csv",

    // Scrapes the rendered table into the column layout the ATO's STP Phase 2
    // worksheet expects. getText() / getValue() / defaultValue() / Papa.unparse()
    // are installed by app.js#installScriptHelpers before this runs.
    exportCsv: function () {
      const data = [];
      const employees = document.getElementsByClassName('employee');
      for (let i = 0; i < employees.length; i++) {
        const employee = employees[i];
        data.push({
          "Entity ABN": document.getElementsByClassName('0fba87ee0386401a8d1f302313b663f4').getText().split(' ').join(''),
          "Period W1 value": document.getElementsByClassName('a850ea3d0ee80695311cd5bf38c6c919').getValue(),
          "Period W2 value": document.getElementsByClassName('48e36eda2b0205de0b0139297433f16e').getValue(),
          "Period CS garnishee Total": 0,
          "Period CS deduction Total": 0,
          "Payroll number": employee.getElementsByClassName('b8c661cdff564862853b75d0f2920776').getText(),
          "Employee TFN": employee.getElementsByClassName('8067ce2d6a4845f1a5a95e2d923e9cc7').getText().split(' ').join(''),
          "Family name": employee.getElementsByClassName('11acbfb39557487a8b8b2528a2c43c53').getText(),
          "Given name": employee.getElementsByClassName('0713b751ee074d228b17ad0131029ca0').getText(),
          "Middle name": employee.getElementsByClassName('91a2b72299cd41e1b638532793aae782').getText(),
          "Date of birth": employee.getElementsByClassName('f6859bbb17364e6081f22703a9ea4686').getText().split('-').reverse().join('/'),
          "Address 1": employee.getElementsByClassName('57a258c732964b4abc6c82ba5755c222').getText(),
          "Address 2": employee.getElementsByClassName('ce735064e907478f83b0f319b2a9a7fb').getText(),
          "Suburb": employee.getElementsByClassName('f6326e3861c941c68bda7a9f2d594150').getText(),
          "State/territory": employee.getElementsByClassName('050a38288ba04db9b560f24a3c5e413b').getText(),
          "Postcode": employee.getElementsByClassName('dc15193e98834e89a78b7e6751db5240').getText(),
          "Country": employee.getElementsByClassName('343a86335d1046ca9d200beed32ebab8').getText().split(' ')[0],
          "Email": employee.getElementsByClassName('f66ab672c1c642809439bdb0a72b7619').getText(),
          "Hired date": employee.getElementsByClassName('03d6a7cc2fc847f7a0018d714c55cd48').getText().split('-').reverse().join('/'),
          "Basis of employment code": employee.getElementsByClassName('6b83766610394cfc948b5c36b30682c2').getText().split(' ')[0],
          "Tax treatment code":
            employee.getElementsByClassName('8980d8c4ed0645349b224e5d4fca2595').getText().split(' ')[0].defaultValue('R') +
            employee.getElementsByClassName('89005ebb48984f7981f3471b61143d77').getText().split(' ')[0].defaultValue('T') +
            employee.getElementsByClassName('b159d29eb71f4f45aacfc61ac53fb576').getText().split(' ')[0].defaultValue('X') +
            employee.getElementsByClassName('570b929c64cc478cb26f332841fd9632').getText().split(' ')[0].defaultValue('X') +
            employee.getElementsByClassName('8bbfc7164cf74ad99cc557fa4bcd8176').getText().split(' ')[0].defaultValue('X') +
            employee.getElementsByClassName('8100d24730af4996a6b46a68a71b2098').getText().split(' ')[0].defaultValue('X'),
          "Pay period start date": document.getElementsByClassName('cef33379d1b34172b0900fc24cf978da').getValue().split('-').reverse().join('/'),
          "Pay period end date": document.getElementsByClassName('8ba7e5e78f74443ab7eed8539b12e7e2').getValue().split('-').reverse().join('/'),
          "Final EOY pay indicator": document.getElementsByClassName('8ba7e5e78f74443ab7eed8539b12e7e2').getValue().endsWith('-06-30').toString(),
          "Income stream code": employee.getElementsByClassName('af894e259a1e438495362f816d86c2e5').getText().split(' ')[0].defaultValue('SAW'),
          "Income stream country code": employee.getElementsByClassName('2ee7f0d083f34b438ff92d34291ad7b1').getText(),
          "Employee gross pay": employee.getElementsByClassName('9188c2ce35e8496db1227f72ba2e6edb').getValue(),
          "Employee tax": employee.getElementsByClassName('35deb25162f345ba927b7a2a13d6b13b').getValue(),
          "Exempt foreign income": employee.getElementsByClassName('84ddd187709c0bc003caab12732df1a0').getValue(),
          "Overtime": employee.getElementsByClassName('cd00edeaf36901742dd3f159bf16654d').getValue(),
          "Bonus commission": employee.getElementsByClassName('6679f062fdb60549392be1d747bf3b50').getValue(),
          "Directors fees": employee.getElementsByClassName('3982712ed4740d9a1ef95d12784af2b8').getValue(),
          "Employee CDEP": employee.getElementsByClassName('4884dc1d8ff50f66298baecd3c08d592').getValue(),
          "Cashout leave": employee.getElementsByClassName('ad0fc512e2790cdd11814b3e943ff661').getValue(),
          "Term unused leave": employee.getElementsByClassName('75e0fdc7d2c80c5e1563181178c6222a').getValue(),
          "Parental leave": employee.getElementsByClassName('c2d83ab2beb8022b2d7f505eed85c81a').getValue(),
          "Workers comp leave": employee.getElementsByClassName('3d726ef282b100fc3089308a79ca92a6').getValue(),
          "Defence leave": employee.getElementsByClassName('f5f964e10b76041709d27a82bc9e475b').getValue(),
          "Other leave": employee.getElementsByClassName('f7fca28ecdd90b840b0b93d681e51f53').getValue(),
          "Kilometer allowance": employee.getElementsByClassName('6618deccac7e032116c91180c33898b1').getValue(),
          "Transport allowance": employee.getElementsByClassName('e00ab1488c9d089e0cb7d46144303a33').getValue(),
          "Laundry allowance": employee.getElementsByClassName('e1e8c096b40801af10875c6680c853e7').getValue(),
          "Meal allowance": employee.getElementsByClassName('794a0272b0f5091b124d281425c29511').getValue(),
          "Travel allowance": employee.getElementsByClassName('e00ab1488c9d089e0cb7d46144303a33').getValue(),
          "Tool allowance": employee.getElementsByClassName('c52ff56882cd0d7509b560b5838fa112').getValue(),
          "Tasks allowance": employee.getElementsByClassName('0deb298ec0e00a600dc228d02eb235d3').getValue(),
          "Qualifications allowance": employee.getElementsByClassName('c5bfe840391b00b61331886df7f3b69b').getValue(),
          "Other allowance 1 description": "Other",
          "Other allowance 1 value": employee.getElementsByClassName('d379e617b2bf057c3153b0f7dc1ebb45').getValue(),
          "Lump sum A type T amount": employee.getElementsByClassName('51fc497926c000513013df9bafe9a167').getValue(),
          "Lump sum A type R amount": employee.getElementsByClassName('64feba644453068712328399ce608ae2').getValue(),
          "Lump sum B amount": employee.getElementsByClassName('487d70c8f50304fd1c249235754ed87c').getValue(),
          "Lump sum D amount": employee.getElementsByClassName('b59011d0e6c80abb151d9600ed43805d').getValue(),
          "Lump sum E financial year 1": employee.getElementsByClassName('61b44922f99d0ff734ae25d8185037e2').getValue(),
          "Lump sum W amount": employee.getElementsByClassName('e4d0ec46312e0556356e0f3ffc064050').getValue(),
          "Union fees": employee.getElementsByClassName('7c0a7407a79344ae8a6d3bb36962d939').getValue(),
          "Workplace giving": employee.getElementsByClassName('87beb0025e81461ba967c60507376c2d').getValue(),
          "Super guarantee amount": employee.getElementsByClassName('a981e618f8fb0aec323a9750fc1a451b').getValue(),
          "SG earnings amount": employee.getElementsByClassName('63d31999e68e0ca122f95132a5996ec4').getValue(),
          "RESC": employee.getElementsByClassName('991da4602b2b09f61d214b49f08b0018').getValue(),
          "Termination date": employee.getElementsByClassName('2d6b0b420dc742429f9dc37a9eba5c48').getText().split('-').reverse().join('/'),
          "Termination type": employee.getElementsByClassName('cc5736e62ecb45b489204b9e0cef15c9').getText().split(' ')[0],
        });
      }

      document.getElementsByName('content')[0].value = Papa.unparse(data);
    },
  },

};
