$(function () {
    "use strict";
    var $specialisatie = $("#Specialisatie");
    var andere = $specialisatie.val();
    var opties = [];

    $("#SpecialisatieId option").each(function () {
        var $item = $(this);
        if ($item.val() != "") {
            opties.push({ index: $item.val(), text: $item.text().toLowerCase() });
        }
    });

    var indexOf = function (text) {
        for (var i = 0; i < opties.length; i++) {
            if (text.toLowerCase() === opties[i].text) {
                return opties[i].index;
            }
        }
        return -1;
    };

    if ($specialisatie.val() === "") {
        $specialisatie.css("display", "none");
    } else {
        var selectedIndex = indexOf($specialisatie.val());
        if (selectedIndex != -1) {
            $("#SpecialisatieId").val(selectedIndex).change();
            $specialisatie.css("display", "none");
        } else {
            $("#SpecialisatieId").val(indexOf("Andere")).change();
            $specialisatie.css("display", "inline");
        }
    }

    $("#SpecialisatieId").change(function () {
        var $selected = $("#SpecialisatieId option:selected");
        var specialisatie = $selected.text();
        if ($selected.val() === "") {
            specialisatie = "";
        }
        if (specialisatie === "Andere") {
            $specialisatie.css("display", "inline");
            if (indexOf(andere) === -1) {
                $specialisatie.val(andere);
            } else {
                $specialisatie.val("");
            }
        } else {
            $specialisatie.css("display", "none");
            $specialisatie.val(specialisatie);
        }
    });

    $specialisatie.keyup(function () {
        andere = $specialisatie.val();
    });

});
