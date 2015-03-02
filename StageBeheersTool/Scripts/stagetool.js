"use strict";

$(function () {
    var $inputs = $("#searchForm input, #searchForm select");
    var $searchA = $("#searchA");
    var $searchForm = $("#searchForm");
    var emptyInputs = $inputs.filter(function () {
        return !$(this).val();
    }).length;
    if (emptyInputs == $inputs.length) {
        $searchForm.css("display", "none");
    }
    else {
        $searchA.remove();
    }
    $searchA.on("click", function () {
        $searchForm.css("display", "block");
        $searchA.remove();
    });

    var ajaxDone = function (data) {
        $("#grid-tableDiv").html(data);
        history.pushState({}, "", this.url);
        registerListener();
    };

    $searchForm.submit(function () {
        $.ajax({
            method: "get", url: $(this).attr("action"),
            headers: { "X-Requested-Width": "XMLHttpRequest" },
            data: $(this).serialize()
        }).done(ajaxDone);

        return false;
    });
    var registerListener = function () {
        var $aList = $(".grid-header >> a, .pagination>>a");
        $aList.each(function (i, a) {
            var $a = $(a);
            $a.on("click", function () {
                $.ajax({
                    method: "get", url: a.href,
                    headers: { "X-Requested-With": "XMLHttpRequest" }
                }).success(ajaxDone);
                return false;
            });
        });
    }
    registerListener();
    $(window).on("popstate", function (e) {
        if (e.originalEvent.state !== null) {
            location.reload();
        }
    });

});