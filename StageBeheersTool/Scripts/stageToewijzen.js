$(function () {
    "use strict";

    var $begindatum = $("#Begindatum");
    var $einddatum = $("#Einddatum");
    var $semester = $("#Semester");

    var $aangepasteStageperiode = $("#AangepasteStageperiode");
    $aangepasteStageperiode.change(toggleStageperiodes);

    function toggleStageperiodes() {
        if ($aangepasteStageperiode.is(":checked")) {
            $begindatum.removeAttr("disabled","disabled");
            $einddatum.removeAttr("disabled", "disabled");
        } else {
            $begindatum.attr("disabled", "disabled");
            $einddatum.attr("disabled", "disabled");
        }
    }

    toggleStageperiodes();

    $('.datepicker').datepicker({ dateFormat: "dd/mm/yy" });

    var semester1Begin = $("#Semester1Begin").html();
    var semester1Einde = $("#Semester1Einde").html();
    var semester2Begin = $("#Semester2Begin").html();
    var semester2Einde = $("#Semester2Einde").html();

    function setDefaultDates() {
        if ($semester.find("option:selected").text() == "1") {
            $begindatum.datepicker("option", "defaultDate", semester1Begin);
            $einddatum.datepicker("option", "defaultDate", semester1Einde);
            $begindatum.val(semester1Begin);
            $einddatum.val(semester1Einde);
        } else {
            $begindatum.datepicker("option", "defaultDate", semester2Begin);
            $einddatum.datepicker("option", "defaultDate", semester2Einde);
            $begindatum.val(semester2Begin);
            $einddatum.val(semester2Einde);
        }
    }

    if (!$begindatum.val() && !$einddatum.val()) {
        setDefaultDates();
    }

    $semester.change(function () {
        setDefaultDates();
    });

});