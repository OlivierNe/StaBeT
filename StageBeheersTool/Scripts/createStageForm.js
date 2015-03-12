"use strict";

$(function () {
    $("#stagementorBtn").click(function (e) {
        getFormStagementorForm(e.target);
        return false;
    });

    $("#controndBtn").click(function (e) {
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
    if ($("#ContractondertekenaarId option[value='-1']").length == 0) {
        var voornaam = $("#Stagementor_Voornaam").val();
        var naam = $("#Stagementor_Familienaam").val();
        if ($("#Stagementor_IsContractondertekenaar").is(":checked") && naam && voornaam) {
            var o = new Option(naam + " " + voornaam, "-1");
            if ($("#ContractondertekenaarIdHidden").val() == "-1") {
                o.selected = true;
            }
            $("#ContractondertekenaarId").append(o);
        }
    }
    if ($("#stagementorForm").children().length > 0) {
        $("#stagementorDropdown").hide();
    }

    $("#Stagementor_IsContractondertekenaar").change(function () {
        if ($(this).is(":checked")) {
            var o = new Option($("#Stagementor_Familienaam").val() + " " + $("#Stagementor_Voornaam").val(), "-1");
            $("#ContractondertekenaarId").append(o);
        }
        else {
            $("#ContractondertekenaarId option[value='-1']").remove();
        }
    });
    $("#Stagementor_Voornaam, #Stagementor_Familienaam").keyup(function () {
        if ($("#Stagementor_IsContractondertekenaar").is(":checked")) {
            $("#ContractondertekenaarId option[value='-1']").text($("#Stagementor_Familienaam").val() + " " + $("#Stagementor_Voornaam").val());
        }
    });
    $("#cancelBtnStM").click(function () {
        $("#stagementorForm").fadeOut("slow", function () {
            $("#stagementorForm").empty();
            $("#ContractondertekenaarId option[value='-1']").remove();
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
        $("#contractondertekenaarForm input[type='text'],#contractondertekenaarForm input[type='email'],#contractondertekenaarForm textarea").each(showRequired);
        $("#ControndDropdown").hide();
        initContrOndForm();
        initClientsideValidation();
    });
}
function initContrOndForm() {
    if ($("#StagementorId option[value='-1']").length == 0) {
        var naam = $("#Contractondertekenaar_Familienaam").val();
        var voornaam = $("#Contractondertekenaar_Voornaam").val();
        if ($("#Contractondertekenaar_IsStagementor").is(":checked") && naam && voornaam) {
            var o = new Option(naam + " " + voornaam, "-1");
            if ($("#StagementorIdHidden").val() == "-1") {
                o.selected = true;
            }
            $("#StagementorId").append(o);
        }
    }
    if ($("#contractondertekenaarForm").children().length > 0) {
        $("#ControndDropdown").hide();
    }
    $("#Contractondertekenaar_IsStagementor").change(function () {
        if ($(this).is(":checked")) {
            var o = new Option($("#Contractondertekenaar_Familienaam").val() + " " + $("#Contractondertekenaar_Voornaam").val(), "-1");
            $("#StagementorId").append(o);
        }
        else {
            $("#StagementorId option[value='-1']").remove();
        }
    });
    $("#Contractondertekenaar_Voornaam, #Contractondertekenaar_Familienaam").keyup(function () {
        if ($("#Contractondertekenaar_IsStagementor").is(":checked")) {
            $("#StagementorId option[value='-1']").text($("#Contractondertekenaar_Familienaam").val() + " " + $("#Contractondertekenaar_Voornaam").val());
        }
    });
    $("#cancelBtnCO").click(function () {
        $("#contractondertekenaarForm").fadeOut("slow", function () {
            $("#contractondertekenaarForm").empty();
            $("#StagementorId option[value='-1']").remove();
            $("#ControndDropdown").show();
        });
    });
}
