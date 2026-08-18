// CSV export for the Single Touch Payroll Worksheet (Phase 2) report.
// app.js calls window.EXPORT_CSV after rendering the table, then picks up the
// CSV string from textarea[name="content"] and wires up a Download CSV button.
// getText() / getValue() / defaultValue() / Papa.unparse() are installed by
// app.js#installScriptHelpers before this runs.
window.EXPORT_CSV_FILENAME = "Single Touch Payroll Worksheet (Phase 2).csv";

window.EXPORT_CSV = function () {
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
};
