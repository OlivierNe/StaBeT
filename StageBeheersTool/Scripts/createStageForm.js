"use strict";

$(function () {
    $("#stagementorBtn").click(function (e) {
        getFormStagementorForm(e.target);
        return false;
    });

    $("#contrOndBtn").click(function (e) {
        getContrOndForm(e.target);
        return false;
    });
    initContrOndForm();
    initStagementorForm();
});
function initClientsideValidation() {
    var form = $("#stageopdrachtForm");
    form.removeData("validator");
    form.removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(form);
}
/*
    * Stagementor
    */
function getFormStagementorForm(a) {
    var $a = $(a);
    $.ajax({
        url: $a.attr("href"),
        method: $a.attr("data-ajax-method"),
        headers: { "X-Requested-With": "XMLHttpRequest" }
    }).success(function (data) {
        $($a.attr("data-ajax-update")).html(data).hide().fadeIn(1000);
        $("#stagementorForm input[type='text'],#stagementorForm input[type='email'],#stagementorForm textarea").each(showRequired);
        $("#stagementorDropdown").hide();
        initStagementorForm();
        initClientsideValidation();
    });
}
function initStagementorForm() {
    /*
     * Stagementor
     */
    if ($("#ContractOndertekenaarId option[value='-1']").length == 0) {
        var voornaam = $("#Stagementor_Voornaam").val();
        var naam = $("#Stagementor_Familienaam").val();
        if ($("#Stagementor_IsContractOndertekenaar").is(":checked") && naam && voornaam) {
            var o = new Option(naam + " " + voornaam, "-1");
            if ($("#ContractOndertekenaarIdHidden").val() == "-1") {
                o.selected = true;
            }
            $("#ContractOndertekenaarId").append(o);
        }
    }
    if ($("#stagementorForm").children().length > 0) {
        $("#stagementorDropdown").hide();
    }

    $("#Stagementor_IsContractOndertekenaar").change(function () {
        if ($(this).is(":checked")) {
            var o = new Option($("#Stagementor_Familienaam").val() + " " + $("#Stagementor_Voornaam").val(), "-1");
            $("#ContractOndertekenaarId").append(o);
        }
        else {
            $("#ContractOndertekenaarId option[value='-1']").remove();
        }
    });
    $("#Stagementor_Voornaam, #Stagementor_Familienaam").keyup(function () {
        if ($("#Stagementor_IsContractOndertekenaar").is(":checked")) {
            $("#ContractOndertekenaarId option[value='-1']").text($("#Stagementor_Familienaam").val() + " " + $("#Stagementor_Voornaam").val());
        }
    });
    $("#cancelBtnStM").click(function () {
        $("#stagementorForm").fadeOut("slow", function () {
            $("#stagementorForm").empty();
            $("#ContractOndertekenaarId option[value='-1']").remove();
            $("#stagementorDropdown").show();
        })
    });
}

/*
 * Contractondertekenaar
 */
function getContrOndForm(a) {
    var $a = $(a);
    $.ajax({
        url: $a.attr("href"),
        method: $a.attr("data-ajax-method"),
        headers: { "X-Requested-With": "XMLHttpRequest" }
    }).success(function (data) {
        $($a.attr("data-ajax-update")).html(data).hide().fadeIn(1000);
        $("#contractOndertekenaarForm input[type='text'],#contractOndertekenaarForm input[type='email'],#contractOndertekenaarForm textarea").each(showRequired);
        $("#ContrOndDropdown").hide();
        initContrOndForm();
        initClientsideValidation();
    });
}
function initContrOndForm() {
    if ($("#StagementorId option[value='-1']").length == 0) {
        var naam = $("#ContractOndertekenaar_Familienaam").val();
        var voornaam = $("#ContractOndertekenaar_Voornaam").val();
        if ($("#ContractOndertekenaar_IsStagementor").is(":checked") && naam && voornaam) {
            var o = new Option(naam + " " + voornaam, "-1");
            if ($("#StagementorIdHidden").val() == "-1") {
                o.selected = true;
            }
            $("#StagementorId").append(o);
        }
    }
    if ($("#contractOndertekenaarForm").children().length > 0) {
        $("#ContrOndDropdown").hide();
    }
    $("#ContractOndertekenaar_IsStagementor").change(function () {
        if ($(this).is(":checked")) {
            var o = new Option($("#ContractOndertekenaar_Familienaam").val() + " " + $("#ContractOndertekenaar_Voornaam").val(), "-1");
            $("#StagementorId").append(o);
        }
        else {
            $("#StagementorId option[value='-1']").remove();
        }
    });
    $("#ContractOndertekenaar_Voornaam, #ContractOndertekenaar_Familienaam").keyup(function () {
        if ($("#ContractOndertekenaar_IsStagementor").is(":checked")) {
            $("#StagementorId option[value='-1']").text($("#ContractOndertekenaar_Familienaam").val() + " " + $("#ContractOndertekenaar_Voornaam").val());
        }
    });
    $("#cancelBtnCO").click(function () {
        $("#contractOndertekenaarForm").fadeOut("slow", function () {
            $("#contractOndertekenaarForm").empty();
            $("#StagementorId option[value='-1']").remove();
            $("#ContrOndDropdown").show();
        });
    });
}
