if ($("#sidebar-wrapper").length === 0) {
    $("#page-content-wrapper").css("padding-left", "0");
}
$("#page-content-wrapper").css("visibility", "visible");

$(function () {
    "use strict";

    window.setTimeout(function () {
        $("#messageId").fadeOut("slow");
    }, 20000);



    //sidebar
    var url = location.href;
    $("#sidebarItems").find("a").each(function (i, item) {
        if (item.href !== "" && url.indexOf(item.href) > -1) {
            $(this).parent().parent().addClass("in");
            $(this).parent().addClass("selectedItem");
            return false;
        }
    });

});