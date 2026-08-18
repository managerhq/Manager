Vue.component("select2", {
    props: ['value', 'multiple', 'autocompleteFilter', 'autocompleteSubtext', 'width', 'autocompleteUrl', 'autocompleteOnChange', 'noMatchesText', 'searchingText', 'placeholder'],
    data: function () {
        return {
            data: []
        }
    },
    template: "<input type='hidden' />",
    mounted: function () {
        var vm = this;
        this.data = this.value;
        $(this.$el)
            // init select2
            .select2({
                allowClear: true,
                dropdownAutoWidth: true,
                placeholder: this.placeholder,
                multiple: (this.multiple == 'true' ? true : false),
                id: 'Key',
                formatResult: function (item) {
                    var output = item.UniqueName;
                    if (vm.autocompleteSubtext && item[vm.autocompleteSubtext] != null) output = '<div class="flex justify-between items-center gap-3"><div>' + output + '</div><div class="opacity-50">' + item[vm.autocompleteSubtext].UniqueName + '</div></div>';
                    return output;
                },
                formatSelection: function (item) { return item.UniqueName; },
                formatNoMatches: this.noMatchesText,
                formatSearching: this.searchingText,
                ajax:
                {
                    url: function () { return vm.autocompleteUrl },
                    dataType: 'json',
                    width: 'copy',
                    data: function (term, page) { return { Term: term, Page: page, Filter: vm.autocompleteFilter } },
                    results: function (data, page) { return data; }
                }
            })
            .select2('data', this.data)
            .trigger("change")
            // emit event on change.
            .on("change", function () {
                vm.data = $(this).select2('data');
                vm.$emit("input", vm.data);
                vm.autocompleteOnChange(vm.data);
            });        
    },
    watch: {
        value: function (value) {
            // update value
            if (this.data != value) {
                this.data = value;
                $(this.$el).select2('data', value);
            }
        }
    },
    destroyed: function () {
        $(this.$el)
            .off()
            .select2("destroy");
    }
});

Vue.component("input-decimal", {
    props: ['value', 'groupSeparator', 'decimalSeparator', 'minWidth', 'nullable', 'placeholder'],
    data: function () {
        return {
            number: null
        }
    },
    template: "<input type='text' class='form-control field-sizing-content' style='text-align: right; min-inline-size: 10ch' />",
    mounted: function () {
        var vm = this;
        this.number = this.value;
        if (this.nullable == 'true') {
            if (this.number != null) this.$el.value = this.number.toString().replace('.', decimalSeparator);
        }
        else {
            if (this.number != 0) this.$el.value = this.number.toString().replace('.', decimalSeparator);
            this.$el.placeholder = this.placeholder;
        }
        this.$el.style.minWidth = this.minWidth;
        $(this.$el).on('input', function () {
            var text = $(this).val() || '';
            text = text.replaceAll(vm.groupSeparator, '');
            text = text.replaceAll(vm.decimalSeparator, '.');
            text = text.replaceAll(' ', '');
            if (text.length == 0) {
                if (vm.nullable == 'true') {
                    vm.number = null;
                    vm.$emit('input', null);
                }
                else {
                    vm.number = 0;
                    vm.$emit('input', 0);
                }
            }
            else {
                try { var parsedNumber = new Mexp().eval(text); } catch (e) { }
                if (typeof parsedNumber != 'number') {
                    $(this).parent().addClass('has-error');
                }
                else {
                    if (text.includes('*') || text.includes('/') || text.includes('+') || text.includes('-')) {
                        vm.number = (parsedNumber.toPrecision(14) * 1);
                    }
                    else {
                        vm.number = parseFloat(text);
                    }
                    vm.$emit('input', vm.number);
                    $(this).parent().removeClass('has-error');
                }
            }
        });        
    },
    watch: {
        value: function (value) {
            if (this.number != value) {
                // update value
                this.number = value;
                $(this.$el).val((this.number == 0 || this.number == null) ? '' : this.number.toString().replace('.', decimalSeparator))
            }
        }
    }
});

Vue.component("liquid-editor", {
    props: ['value'],
    template: "<div />",
    mounted: function () {
        var vm = this;
        var editor = ace.edit(this.$el);
        editor.getSession().setUseWorker(false);
        editor.getSession().setMode('ace/mode/liquid');
        editor.setOption("displayIndentGuides", false);
        if (this.value) editor.setValue(this.value, -1);
        editor.on('blur', function (e) { vm.$emit('input', editor.getValue()) });
    }
});

Vue.component("html-editor", {
    props: ['value'],
    template: "<div />",
    mounted: function () {
        var vm = this;
        var editor = ace.edit(this.$el);
        editor.getSession().setUseWorker(false);
        editor.getSession().setMode('ace/mode/html');
        editor.setOption("displayIndentGuides", false);
        if (this.value) editor.setValue(this.value, -1);
        editor.on('blur', function (e) { vm.$emit('input', editor.getValue()) });
    }
});

Vue.component("javascript-editor", {
    props: ['value'],
    template: "<div />",
    mounted: function () {
        var vm = this;
        var editor = ace.edit(this.$el);
        editor.getSession().setUseWorker(false);
        editor.getSession().setMode('ace/mode/javascript');
        editor.setOption("displayIndentGuides", false);
        if (this.value) editor.setValue(this.value, -1);
        editor.on('blur', function (e) { vm.$emit('input', editor.getValue()) });
    }
});

function format(html) {
    var tab = '';
    var result = '';
    var indent = '';

    html.split(/>\s*</).forEach(function (element) {
        if (element.match(/^\/\w/)) {
            indent = indent.substring(tab.length);
        }

        result += indent + '<' + element + '>\r\n';

        if (element.match(/^<?\w[^>]*[^\/]$/) && !element.startsWith('input')) {
            indent += tab;
        }
    });

    return result.substring(1, result.length - 3);
}

if (!Number.prototype.getDecimals) {
    Number.prototype.getDecimals = function () {
        var num = parseFloat(this.toFixed(10));
        var match = ('' + num).match(/(?:\.(\d+))?(?:[eE]([+-]?\d+))?$/);
        if (!match)
            return 0;
        return Math.max(0, (match[1] ? match[1].length : 0) - (match[2] ? +match[2] : 0));
    }
}

String.prototype.replaceAll = function (target, replacement) {
    return this.split(target).join(replacement);
};