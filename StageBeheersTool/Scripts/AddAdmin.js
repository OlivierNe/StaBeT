$(function () {
    $("input[type='checkbox'][name$='.IsAdmin']").each(function () {
        var $self = $(this);
        $self.change(function () {
            var name = $self.attr("name");
            var index = name.substring(1, name.indexOf("]"));
            var hasChangedInput = $("input[type='hidden'][name='[" + index + "].HasChanged']");
            if (hasChangedInput.val() == "true") {
                hasChangedInput.val(false);
            } else {
                hasChangedInput.val(true);
            }
        });
    });

    $("input[type='hidden'][name$='.HasChanged']").each(function () {
        $(this).val(false);
    });

    $("#addButton").click(function () {
        var index = $("#AdminTable tbody tr").length - 2;
        $('#AdminTable tr:last').before(
            '<tr><td class="col-md-1">' +
            '<input type="email" name="[' + index + '].Email" class="text-box single-line" ' +
            'data-val="true" data-val-email="Het veld E-mail is geen geldig e-mailadres."/></td>' +
            '<td class="col-md-3"><input type="checkbox" value="true" checked name="[' + index + '].IsAdmin"/></td>' +
            '<td> <input name="[' + index + '].HasChanged" type="hidden" value="True" /></td></tr>');
        return false;
    });
});