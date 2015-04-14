
$(function () {
    "use strict";
    var $emailDiv = $("#emailDiv");
    var $Email = $($emailDiv.attr("data-email-veld"));

    $Email.blur(function () {
        $.ajax({
            url: $emailDiv.attr("data-url-json") + $Email.val(),
            error: function () {
                $emailDiv.html("");
            },
            success: function (gebruiker) {
                if (!gebruiker.length) {
                    console.log(gebruiker);
                    $emailDiv.html($emailDiv.attr("data-class") + ' met E-mail ' + $Email.val()
                        + ' bestaat al. Klik <a href="' + $emailDiv.attr("data-url-edit") + gebruiker.Id +
                        '">hier</a> om de gegevens te wijzigen.');
                } else {
                    $emailDiv.html("");
                }
            }
        });
    });
});