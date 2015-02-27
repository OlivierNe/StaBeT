﻿"use strict";

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