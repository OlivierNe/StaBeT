
$(function () {
    "use strict";
    var $email = $("#Email");
    var $hogentEmailDiv = $("#hogentEmailDiv");

    $email.blur(function () {
        $.ajax({
            url: $hogentEmailDiv.attr("data-url-json") + $email.val(),
            error: function () {
                $hogentEmailDiv.html("");
            },
            success: function (hogentPersoon) {
                if (!hogentPersoon.length) {
                    $hogentEmailDiv.html($hogentEmailDiv.attr("data-class") + ' met HoGent E-mail ' + hogentPersoon.HogentEmail
                        + ' bestaat al. Klik <a href="' + $hogentEmailDiv.attr("data-url-edit") + hogentPersoon.Id +
                        '">hier</a> om de gegevens te wijzigen.');
                } else {
                    $hogentEmailDiv.html("");
                }
            }
        });
    });
});