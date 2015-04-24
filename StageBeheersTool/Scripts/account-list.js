$(function () {
    "use strict";

    $(".editBtn").click(onEditBtnClick);
    var oldRowHtml = 'undefined';
    var $currentRow = 'undefined';

    $("#grid-tableDiv").on("tableReloaded", function () {
        $(".editBtn").click(onEditBtnClick);
        oldRowHtml = 'undefined';
        $currentRow = 'undefined';
    });

    function onCancelBtnClick() {
        $currentRow.html('');
        $currentRow.html(oldRowHtml);
        $currentRow.find(".editBtn").click(onEditBtnClick);
        $currentRow = 'undefined';
        oldRowHtml = 'undefined';
    }

    function onEditBtnClick() {
        var $button = $(this);
        if ($button.text() === 'Opslaan') {
            saveChanges();
        }
        if ($currentRow !== 'undefined') {
            return false;
        }
        $currentRow = $button.closest("tr");
        oldRowHtml = $currentRow.html();

        //buttons aanpassen
        $button.removeClass("btn-default");
        $button.addClass("btn-success");
        $button.text("Opslaan");
        $currentRow.find(".delBtn").parent().empty().append("<button value='Annuleren' class='btn btn-danger' id='cancelBtn'>Annuleren </button>");
        $("#cancelBtn").click(onCancelBtnClick);

        //input velden editable maken
        var $tds = $currentRow.find("td");
        var $checkboxes = $tds.find("input:checkbox");
        $checkboxes.each(function () {
            $(this).removeAttr("disabled");
        });

        var email = $tds.first("[data-name='Login']").text();
        $tds.first("[data-name='Login']").empty().append("<input type='text' value='" + email + "' name='Email' id='Email' class='form-control'/>");
        $currentRow.find("input:hidden").not("input[name='Id']").remove();

        return false;
    }
    function saveChanges() {
        var values = $currentRow.find("input").serialize();
        $.ajax({ url: "/Account/AjaxEdit", data: values, type: "POST" })
            .success(function (data) {
                if (data.type === "success") {
                    $("#result").html(' <div class="alert alert-success alert-dismissible" role="alert" id="messageId">' +
                        '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span>' +
                        '</button> ' + data.message + '</div>');
                    var elem = $("<tr>" + oldRowHtml + "</tr>");
                    elem.find("td").first("[data-name='Login']").text(data.Email);
                    if (data.Admin) {
                        elem.find("#Admin").attr("checked", "checked");
                    } else {
                        elem.find("#Admin").removeAttr("checked");
                    }
                    if (data.Begeleider) {
                        elem.find("#Begeleider").attr("checked", "checked");
                    } else {
                        elem.find("#Begeleider").removeAttr("checked");
                    }
                    if (data.Student) {
                        elem.find("#Student").attr("checked", "checked");
                    } else {
                        elem.find("#Student").removeAttr("checked");
                    }
                    if (data.Bedrijf) {
                        elem.find("#Bedrijf").attr("checked", "checked");
                    } else {
                        elem.find("#Bedrijf").removeAttr("checked");
                    }
                    $currentRow.html('');
                    $currentRow.html(elem.html());
                    $currentRow.find(".editBtn").click(onEditBtnClick);
                    $currentRow = 'undefined';
                    oldRowHtml = 'undefined';

                } else {
                    $("#result").html(' <div class="alert alert-danger alert-dismissible" role="alert" id="messageId">' +
                       '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span>' +
                       '</button> ' + data.message + '</div>');
                }
            });
    }
});