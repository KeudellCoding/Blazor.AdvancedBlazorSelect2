ParseToBool = (value) => {
    var boolValues = [true, "true", "True", 1, "1"];
    if ($.inArray(value, boolValues) >= 0) {
        return true;
    }
    else {
        return false;
    }
};

window.KeudellCoding_Blazor_AdvancedBlazorSelect2Component = {
    init: function (id, options, dotnetHelper) {
        var jsonOptions = JSON.parse(options);
        if (ParseToBool(jsonOptions.noMarkupEscape) === true) { jsonOptions.escapeMarkup = (markup) => { return markup; }; }
        jsonOptions.templateSelection = (data) => data.text;
        jsonOptions.templateResult = (data) => data.formatedResult || data.text;

        jsonOptions.ajax = {
            transport: function (params, success, failure) {
                var searchString = params.data && params.data.q ? params.data.q : "";
                var page = params.data.page || 1;
                dotnetHelper.invokeMethodAsync("Select2OnSearch", searchString, page).then(success).catch(failure);
            }
        };

        $('#' + id).select2(jsonOptions);

        $('#' + id).closest("form").on("reset", function (ev) {
            var targetJQForm = $(ev.target);
            setTimeout((function () {
                $('#' + id).trigger("change");
            }).bind(targetJQForm), 0);
        });
    },
    onChange: function (id, dotnetHelper) {
        $('#' + id).on('change.select2', function (e) {
            var valueToInvoke = Array.isArray($('#' + id).val()) ? $('#' + id).val() : [$('#' + id).val()];
            dotnetHelper.invokeMethodAsync("Select2OnChange", valueToInvoke);
        });
    },
    updateSelected: function (id, newData, triggerChange) {
        $('#' + id).val(null);
        $('#' + id).find('option').remove();
        $.each(newData, function (i, data) {
            var option = new Option(data.text, data.id, false, true);
            $('#' + id).append(option);
        });
        if (ParseToBool(triggerChange) === true) {
            $('#' + id).trigger('change');
        }
    },
    getSelectedIds: function (id) {
        return $.makeArray($('#' + id).find(':selected').map(function () { return this.value }));
    }
}