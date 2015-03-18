$(function () {
    "use strict";

    $('.datepicker').datepicker({ dateFormat: "dd/mm/yy" });

    var $Semester1Begin = $("#Semester1Begin");
    var $Semester1Einde = $("#Semester1Einde");
    var $Semester2Begin = $("#Semester2Begin");
    var $Semester2Einde = $("#Semester2Einde");
    var $Academiejaar = $("#Academiejaar");

    function setDefaultDates() {
        var academiejaar = $Academiejaar.val();
        var beginJaar = academiejaar.substring(0, 4);
        var eindJaar = academiejaar.substr(5, academiejaar.length);
        if (!isNaN(beginJaar) && !isNaN(eindJaar)) {
            $Semester1Begin.datepicker("option", "defaultDate", "01/09/" + beginJaar);
            $Semester1Einde.datepicker("option", "defaultDate", "01/01/" + eindJaar);
            $Semester2Begin.datepicker("option", "defaultDate", "01/02/" + eindJaar);
            $Semester2Einde.datepicker("option", "defaultDate", "01/05/" + eindJaar);
        }
    }
    setDefaultDates();

    $Academiejaar.focusout(setDefaultDates);
});