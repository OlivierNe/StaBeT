$(function () {
    "use strict";
    var $bedrijfList = $("#BedrijfId");
    var $gemeente = $("#Gemeente");
    var $postcode = $("#Postcode");
    var $straat = $("#Straat");
    var $straatnummer = $("#Straatnummer");
    var $stagementors = $("#StagementorId");
    var $contractorondertekenaars = $("#ContractondertekenaarId");

    $bedrijfList.change(function () {
        $.ajax({ url: "/Bedrijf/BedrijfJson/" + $bedrijfList.val() })
            .success(function (bedrijf) {
                $gemeente.val(bedrijf.Gemeente);
                $postcode.val(bedrijf.Postcode);
                $straat.val(bedrijf.Straat);
                $straatnummer.val(bedrijf.Straatnummer);

                $stagementors.children('option:not(:first)').remove();
                for (var i = 0; i < bedrijf.Stagementors.length; i++) {
                    var option = new Option(bedrijf.Stagementors[i].Naam, bedrijf.Stagementors[i].Id);
                    $stagementors.append(option);
                }
                $contractorondertekenaars.children('option:not(:first)').remove();
                for (var i = 0; i < bedrijf.Contractondertekenaars.length; i++) {
                    var option = new Option(bedrijf.Contractondertekenaars[i].Naam, bedrijf.Contractondertekenaars[i].Id);
                    $contractorondertekenaars.append(option);
                }
            });
    });
});