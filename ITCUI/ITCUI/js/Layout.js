var fixFrame = function() {

    var rightContainerHeight = $("div#container_right").height();
    var leftContainerHeight = $(".center-content").height();

    if (rightContainerHeight >= leftContainerHeight) {
        $(".center-content").css("min-height", rightContainerHeight)
        $("div#container_right").css("min-height", rightContainerHeight);
    }
    else {
        $(".center-content").css("min-height", leftContainerHeight);
        $("div#container_right").css("min-height", leftContainerHeight);
    }
}


var resizeFrame = function() {
    var w = $(window).width();
    if (w < 800) {
        $("#main_wrapper").css("width", "1000px");
    }
    else {
        $("#main_wrapper").css("width", "100%");
    }    
}