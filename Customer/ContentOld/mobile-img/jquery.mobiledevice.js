//Mobile device jquery create by Gopal create date 06/11/2015-->
$(window).load(function () {
    //alert("1");
    var width = $(window).width();
    $(window).load(function () {
        {
            AddIndex();
            $("#m-device").remove();
            $("#nav-top-menu").remove();
            $(".support-link .all-shop-mobile").remove();
            $(".dow-app-m").remove();
            //$(".sign-mob").remove();
            $("#content-wrap").remove();
            $("#home-slider").remove();
            $("#load-content").remove();
            $("#showSearch").remove();
            $("#Subscription").remove();
            $("#first-content").remove();
            $(".top-header a").css('margin-left', '5px');
            $(".top-header a").css('padding-right', '5px');
            $(".top-header a").css('border-right', 'none');
            $(".main-header").css('border-bottom', '1.5px solid #757575');
            $(".main-header").css('padding-left', '0');
            $(window).scroll(function () {
                var mh = $(window).scrollTop();
                if (mh > 5) {
                    $(".main-header").css('position', 'fixed');
                    $(".main-header").css('width', '100%');
                    $(".main-header").css('top', '0');
                    $(".main-header").css('left', '0');
                    $(".main-header").css('height', '50px');
                    $(".main-header").css('background', 'rgba(35,35,35,0.8)');
                    $(".main-header").css('z-index', '99');
                    $(".logo").css('left', '0');
                    $(".logo").css('top', '2px');
                } else {
                    $(".main-header").removeAttr('style');
                    $(".logo").removeAttr('style');
                    $(".main-header").css('border-bottom', '1.5px solid #757575');
                    $(".main-header").css('padding-left', '0');
                }

            });
        } else {
            // return false;
        }
    });
    //AddIndex = function () {
    AddIndex = function () {
        var partialHtml;
        var url1 = 'nagpur/Home/MobileIndex';
        var url2 = 'wardha/Home/MobileIndexWardha';
        var url3 = 'varanasi/Home/MobileIndexVaranasi';
        var url4 = 'kanpur/Home/MobileIndexKanpur';
        $.get(url1, function (data) {
            $("#mobile-content").html(data);
        });
        $.get(url2, function (data) {
            $("#mobile-wardha").html(data);
        });
        $.get(url3, function (data) {
            $("#mobile-varanasi").html(data);
        });
        $.get(url4, function (data) {
            $("#mobile-kanpur").html(data);
        });
    };
    // };

});