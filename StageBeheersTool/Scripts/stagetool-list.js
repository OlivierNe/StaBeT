"use strict";

$(function () {
    var $inputs = $("#searchForm input, #searchForm select");
    var $searchA = $("#searchA");
    var $cancelBtn = $("#CancelBtn");
    var $searchForm = $("#searchForm");
    var emptyInputs = $inputs.filter(function () {
        return !$(this).val();
    }).length;
    if (emptyInputs == $inputs.length) {
        $searchForm.css("display", "none");
    }
    else {
        $searchA.css("display", "none");
    }
    $searchA.on("click", function () {
        $searchForm.css("display", "block");
        $searchA.css("display", "none");
        $("#searchForm input:text").first().focus();
    });

    $cancelBtn.on("click", function () {
        $searchForm.trigger("reset");
        $searchForm.submit();
        $searchA.css("display", "inline");
        $searchForm.css("display", "none");

    });

    var ajaxDone = function (data) {
        $("#grid-tableDiv").html(data);
        var ajaxUrl = this.url + "&a=1";
        history.pushState({ url: ajaxUrl }, "", ajaxUrl);
        registerListener();
    };

    $searchForm.submit(function () {
        $.ajax({
            method: "get", url: $(this).attr("action"),
            headers: {
                "X-Requested-Width": "XMLHttpRequest",
                "Cache-Control": "no-store"
            },
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
                    headers: {
                        "X-Requested-With": "XMLHttpRequest",
                        "Cache-Control": "no-store"
                    }
                }).success(ajaxDone);
                return false;
            });
        });
    }
    registerListener();

    $(window).on("popstate", function (e) {
        if (e.originalEvent.state !== null) {
            $.ajax({
                method: "get", url: history.state.url,
                headers: {
                    "X-Requested-With": "XMLHttpRequest",
                    "Cache-Control": "no-store"
                },
                cache: false
            }).done(function (data) {
                $("#grid-tableDiv").html(data);
                registerListener();
            });
        }
    });

});