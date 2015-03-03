"use strict";

var showRequired = function () {
    var $input = $(this);
    if ($input.attr("data-val-required")) {
        var label = $('label[for="' + $input.attr('id') + '"]');
        if (label.text().length > 0) {
            label.append("<span style='color:red'> *</span>");
        }
    }
};

$(function () {

    $("input[type='text'], input[type='email'], textarea").each(showRequired);

});

function showpreview(input) {
    if (input.files && input.files[0]) {
        //if (input.files[0].size < 512000) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#foto').attr('src', e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
        // }
    }
};