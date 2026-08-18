(function () {
    "use strict";

    var shortScale = [100];
    var longScale = [100, 1000];
    var i;
    for (i = 1; i <= 16; i++) shortScale.push(Math.pow(10, i * 3));
    for (i = 1; i <= 15; i++) longScale.push(Math.pow(10, i * 6));

    var languages = {};
    var currencies = {};

    function writtenNumber(n, options) {
        if (n < 0) return "";
        var lang = languages[options.language] || languages["en"];
        if (!lang) return "";
        var result = _writtenNumber(n, lang, options, false, null);
        return appendCurrency(result, n, options);
    }

    function _writtenNumber(n, language, options, noAnd, alternativeBase) {
        if (n < 0) return "";

        var scale, units;
        var baseCardinals = language.base;

        if (language.units && typeof language.units === "object" && !Array.isArray(language.units) && !(language.units instanceof Array)) {
            // Dictionary-style units (keys are power exponents)
            var rawUnits = language.units;
            var keys = [];
            var k;
            for (k in rawUnits) {
                if (rawUnits.hasOwnProperty(k)) keys.push(Number(k));
            }
            keys.sort(function (a, b) { return a - b; });
            scale = [];
            units = [];
            for (var i = 0; i < keys.length; i++) {
                scale.push(Math.pow(10, keys[i]));
                units.push(rawUnits[keys[i]]);
            }
        } else {
            units = language.units || [];
            scale = language.useLongScale ? longScale.slice() : shortScale.slice();
        }

        if (language.unitExceptions && language.unitExceptions[n] !== undefined) return language.unitExceptions[n];
        if (alternativeBase && alternativeBase[n] !== undefined) return alternativeBase[n];
        if (baseCardinals[n] !== undefined && baseCardinals[n] !== "") return baseCardinals[n];
        if (n < 100) return handleSmallerThan100(n, language, baseCardinals, alternativeBase, options);

        var m = n % 100;
        var ret = [];
        if (m !== 0) {
            if (noAnd && !(language.andException === true)) {
                ret.push(_writtenNumber(m, language, options, false, null));
            } else {
                ret.push(language.unitSeparator + _writtenNumber(m, language, options, false, null));
            }
        }

        var firstSignificant = 0;
        var len = units.length;
        for (var i = 0; i < len; i++) {
            var r = Math.floor(n / scale[i]);
            var divideBy;

            if (i === len - 1) divideBy = 1000000;
            else divideBy = scale[i + 1] / scale[i];

            r = r % divideBy;
            if (r === 0) continue;
            firstSignificant = scale[i];

            var unit = units[i];

            // useBaseInstead handling
            if (unit && typeof unit === "object" && unit.useBaseInstead === true) {
                var shouldUseBaseException =
                    unit.useBaseException && unit.useBaseException.indexOf(r) > -1 &&
                    (!unit.useBaseExceptionWhenNoTrailingNumbers ||
                     (i === 0 && ret.length > 0));
                if (!shouldUseBaseException) {
                    var baseVal = (alternativeBase && alternativeBase[r * scale[i]] !== undefined)
                        ? alternativeBase[r * scale[i]]
                        : baseCardinals[r * scale[i]];
                    ret.push(baseVal);
                } else {
                    ret.push(r > 1 && unit.plural ? unit.plural : unit.singular);
                }
                continue;
            }

            var str = "";

            if (typeof unit === "string") {
                str = unit;
            } else if (unit && typeof unit === "object") {
                if ((r === 1 || (unit.useSingularEnding === true && r % 10 === 1
                        && (!unit.avoidEndingRules || unit.avoidEndingRules.indexOf(r) < 0)))
                    && unit.singular) {
                    str = unit.singular;
                } else if (unit.few && ((r > 1 && r < 5) ||
                    (unit.useFewEnding === true && r % 10 > 1 && r % 10 < 5
                        && (!unit.avoidEndingRules || unit.avoidEndingRules.indexOf(r) < 0)))) {
                    str = unit.few;
                } else {
                    str = (unit.plural && (!unit.avoidInNumberPlural || m === 0))
                        ? unit.plural
                        : unit.singular;
                    // Languages with dual
                    if (r === 2 && unit.dual) str = unit.dual;
                    // restrictedPlural
                    if (r > 10 && unit.restrictedPlural === true) str = unit.singular;
                }
            }

            // avoidPrefixException
            if (unit && typeof unit === "object" && unit.avoidPrefixException &&
                unit.avoidPrefixException.length > 0 && unit.avoidPrefixException.indexOf(r) > -1) {
                ret.push(str);
                continue;
            }

            var number;
            if (language.unitExceptions && r < Object.keys(language.unitExceptions).length && language.unitExceptions[r] !== undefined) {
                number = language.unitExceptions[r];
            } else if (typeof unit === "string") {
                number = _writtenNumber(r, language, {language: options.language},
                    !(language.andException === true), null);
            } else if (unit && typeof unit === "object") {
                number = _writtenNumber(r, language, {language: options.language},
                    !(language.andException === true || unit.andException === true),
                    unit.alternativeBase || null);
            } else {
                number = "";
            }
            n -= r * scale[i];
            ret.push(number + " " + str);
        }

        var firstSignificantN = firstSignificant * Math.floor(n / firstSignificant);
        var rest = n - firstSignificantN;

        if (language.andWhenTrailing === true && firstSignificant !== 0 && rest > 0 && ret[0].indexOf(language.unitSeparator) !== 0) {
            var a = [ret[0], language.unitSeparator.replace(/\s+/g, "")];
            ret.splice(0, 1);
            ret = a.concat(ret);
        }

        if (language.allSeparator) {
            for (var j = 0; j < ret.length - 1; j++) {
                ret[j] = language.allSeparator + ret[j];
            }
        }

        ret.reverse();
        return ret.join(" ");
    }

    function handleSmallerThan100(n, language, baseCardinals, alternativeBase, options) {
        var dec = Math.floor(n / 10) * 10;
        var unit = n - dec;

        var baseValue = (alternativeBase && alternativeBase[dec] !== undefined)
            ? alternativeBase[dec]
            : (baseCardinals && baseCardinals[dec] !== undefined)
                ? baseCardinals[dec]
                : "";

        if (unit !== 0) return baseValue + language.baseSeparator + _writtenNumber(unit, language, options, false, null);
        return baseValue;
    }

    function appendCurrency(s, value, options) {
        if (options.currency) {
            var langCode = options.language || "en";
            var currencyData = currencies[langCode] || currencies["en"];
            if (currencyData && currencyData[options.currency]) {
                var c = currencyData[options.currency];
                return s + " " + getCurrencyName(c, value);
            } else {
                return s + " " + options.currency;
            }
        }
        return s;
    }

    function getCurrencyName(c, value) {
        if (value === 1) return c.one || c.other;
        if (value === 2) return c.two || c.other;
        if (value < 5) return c.few || c.other;
        if (value < 10) return c.many || c.other;
        return c.other;
    }

    function spellOutRupees(num) {
        var inputNo = Math.floor(num);
        if (inputNo === 0) return "Zero";
        var numbers = [0, 0, 0, 0];
        var first = 0;
        var u, h, t;
        var sb = "";
        if (inputNo < 0) {
            sb += "Minus ";
            inputNo = -inputNo;
        }
        var words0 = ["", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine "];
        var words1 = ["Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen "];
        var words2 = ["Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety "];
        var words3 = ["Thousand ", "Lakh ", "Crore "];
        numbers[0] = inputNo % 1000;
        numbers[1] = Math.floor(inputNo / 1000);
        numbers[2] = Math.floor(inputNo / 100000);
        numbers[1] = numbers[1] - 100 * numbers[2];
        numbers[3] = Math.floor(inputNo / 10000000);
        numbers[2] = numbers[2] - 100 * numbers[3];
        for (i = 3; i > 0; i--) {
            if (numbers[i] !== 0) { first = i; break; }
        }
        for (i = first; i >= 0; i--) {
            if (numbers[i] === 0) continue;
            u = numbers[i] % 10;
            t = Math.floor(numbers[i] / 10);
            h = Math.floor(numbers[i] / 100);
            t = t - 10 * h;
            if (h > 0) sb += words0[h] + "Hundred ";
            if (u > 0 || t > 0) {
                if (t === 0) sb += words0[u];
                else if (t === 1) sb += words1[u];
                else sb += words2[t - 2] + words0[u];
            }
            if (i !== 0) sb += words3[i - 1];
        }
        return sb.trimEnd ? sb.trimEnd() : sb.replace(/\s+$/, "");
    }

    function registerLanguage(code, config) {
        languages[code] = config;
    }

    function registerCurrencies(langCode, data) {
        currencies[langCode] = data;
    }

    // Expose API
    window.writtenNumber = writtenNumber;
    window.spellOutRupees = spellOutRupees;
    window.WrittenNumber = {
        languages: languages,
        currencies: currencies,
        registerLanguage: registerLanguage,
        registerCurrencies: registerCurrencies
    };
})();
