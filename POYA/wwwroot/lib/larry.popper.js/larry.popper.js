
function MakePopper(MainElementString, PopperElementString) {
    var MainElement = $("#" + MainElementString);
    var PopperElement = $("#" + PopperElementString);
    MainElement.on("click", function () {
        if (PopperElement.is(":hidden")) {
            PopperElement.offset().top = MainElement.offset().top - PopperElement.height;
            PopperElement.offset().left = MainElement.offset().left + PopperElement.width;
            if ((MainElement.offset().top - PopperElement.height) < 0) {
                PopperElement.offset().top = MainElement.offset().top + MainElement.height;
            }
            if ((MainElement.offset().left + PopperElement.width) > $(document).width) {
                PopperElement.offset().left = MainElement.offset().left - PopperElement.width;
            }

            PopperElement.show();
        } else {
            PopperElement.hide();
        }
    });

}
 