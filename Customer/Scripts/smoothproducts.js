/*
 * Smoothproducts version 2.0.2
 * #
 *
 * Copyright 2013, Kevin Thornbloom
 * Free to use and abuse under the MIT license.
 * #
 */
(function ($) {
    $.fn.extend({
        smoothproducts: function () {

            // Add some markup & set some CSS
            var pName = '';
            $('.sp-loading').hide();
            $('[id*=zoom]').each(function () {
                $(this).addClass('sp-touch');
                var thumbQty = $('a', this).length;

                // If more than one image
                if ($('#zoom a').length > 1) {
                    $(this).append('<div class="sp-large"></div><div class="sp-thumbs sp-tb-active"><span class="next"></span> <span class="prev"></span><ul></ul></div>');
                    $('a', this).each(function () {

                        // var temp = this.replace(" ", "");

                        var thumb = $('img', this).attr('src');
                        pName=$('img', this).attr('alt');
                        var large = $(this).attr('href');
                        $(this).parents('.sp-wrap').find('.sp-thumbs ul').append('<li><a href="' + large + '" style="background-image:url(' + this + ')"></a></li>');
                        $(this).remove();
                    });
                    $('.sp-thumbs a:first', this).addClass('sp-current');
                    var firstLarge = $('.sp-thumbs a:first', this).attr('href'),
                        firstThumb = get_url_from_background($('.sp-thumbs a:first', this).css('backgroundImage'));
                    //alert(firstThumb);
                    $('.sp-large', this).append('<a href="' + firstLarge + '" class="sp-current-big"><img src="' + firstThumb + ' alt="' + pName + '" /></a>');
                    $('.sp-wrap').css('display', 'inline-block');
                    // If only one image
                } else {
                    $(this).append('<div class="sp-large"></div>');
                    $('a', this).appendTo($('.sp-large', this)).addClass('.sp-current-big');
                    $('.sp-wrap').css('display', 'inline-block');
                }
                // alert($('.sp-thumbs li').length > 3);
                ////// Add next Previus function for thumbnail crete jquery by Gopal Shende date 20-11-2015
                if ($('.sp-thumbs li').length > 4) {
                    var $item = $('.sp-thumbs li'),
                    visible = 2,
                    index = 0,
                    endIndex = ($item.length / visible) - 1; 

                    $('span.next').click(function () {
                        //alert("1");
                        if (index < endIndex) {
                            index++;
                            $item.animate({ 'left': '-=87px' });
                        }
                    });

                    $('span.prev').click(function () {
                        if (index > 0) {
                            index--;
                            $item.animate({ 'left': '+=87px' });
                        }
                    });
                    $('span.next').show();
                    $('span.prev').show();
                } else {
                    $('span.next').hide();
                    $('span.prev').hide();
                }
            });


            // Prevent clicking while things are happening
            $(document.body).on('click', '.sp-thumbs', function (event) {
                event.preventDefault();
            });


            // Is this a touch screen or not?
            $(document.body).on('mouseover', function (event) {
                $('.sp-wrap').removeClass('sp-touch').addClass('sp-non-touch');
                event.preventDefault();
            });

            $(document.body).on('touchstart', function () {
                $('.sp-wrap').removeClass('sp-non-touch').addClass('sp-touch');
            });

            // Clicking a thumbnail
            //$(document.body).on('.mouseover','.sp-tb-active a',function (event) {
            $('.sp-tb-active a').mouseover(function (event) {

                event.preventDefault();
                $(this).parent().find('.sp-current').removeClass();
                $(this).addClass('sp-current');
                $(this).parents('.sp-wrap').find('.sp-thumbs').removeClass('sp-tb-active');
                $(this).parents('.sp-wrap').find('.sp-zoom').remove();

                var currentHeight = $(this).parents('.sp-wrap').find('.sp-large').height(),
                    currentWidth = $(this).parents('.sp-wrap').find('.sp-large').width();
                $(this).parents('.sp-wrap').find('.sp-large').css({
                    overflow: 'hidden',
                    height: currentHeight + 'px',
                    width: currentWidth + 'px'
                });

                $(this).addClass('sp-current').parents('.sp-wrap').find('.sp-large a').remove();

                var nextLarge = $(this).parent().find('.sp-current').attr('href'),
                    nextThumb = get_url_from_background($(this).parent().find('.sp-current').css('backgroundImage'));

                $(this).parents('.sp-wrap').find('.sp-large').html('<a href="' + nextLarge + '" class="sp-current-big"><img src="' + nextThumb + '" alt="' + pName + '"/></a>');
                //comment for priview page disturb remove function hide comment by Gopal Shende Date 20-11-2015
                // $(this).parents('.sp-wrap').find('.sp-large').hide().fadeIn(250, function () {
                $(this).parents('.sp-wrap').find('.sp-large').fadeIn(250, function () {
                    var autoHeight = $(this).parents('.sp-wrap').find('.sp-large img').height();

                    $(this).parents('.sp-wrap').find('.sp-large').animate({
                        height: autoHeight
                    }, 'fast', function () {
                        $('.sp-large').css({
                            height: '295px',
                            width: 'auto'
                        });
                    });

                    $(this).parents('.sp-wrap').find('.sp-thumbs').addClass('sp-tb-active');
                });
            });
            var imgClon = $(".sp-large").clone();
            /// Select Color thumbnail hover to zoom image jquery by Gopal
            $('.SelectColor input.color').mouseover(function (event) {
                var inpurval = $(this).css('backgroundImage');
                //alert(inpurval);
                var imgclon = $(this).parents('.cells12').find('.sp-large').clone();
                // $('.sp-tb-active a').addClass('sp-current').parents('.sp-wrap').find('.sp-large a').remove();
                var lUrl = inpurval;
                lUrl = lUrl.replace('url("', '');
                lUrl = lUrl.replace('")', '');
               
                $(this).parents('.cells12').find('.sp-large').html('<a href="' + lUrl + '" class="sp-current-big"><img src="' + lUrl + '" alt="' + pName + '"/></a>');
                
            });
            $('.SelectColor input.color').mouseout(function (event) {
                var defaultThumb = $(this).closest('.cells12').find('.sp-wrap');
                var getPath = defaultThumb.find('a:first-child img').attr('src');
                //var getCssPath = defaultThumb.find('a').first().css('backgroundImage');
                //alert(imgClon);
                event.preventDefault();
                //var nextLargeUrl = $(this).parent('.cells12').find('.sp-thumbs').html();
                //alert(nextLargeUrl);
                //var hrUrl = nextLargeUrl.find('li:first-child a').attr('href');
                //var nextThumbUrl = nextLargeUrl.find('li:first-child a').css('backgroundImage');
                //alert(nextLargeUrl);'<a href="' + getPath + '" class="sp-current-big"><img src="' + getPath + '"/></a>'
                $(this).parents('.cells12').find('.sp-large').html('<a href="' + getPath + '" class="sp-current-big"><img src="' + getPath + '" alt="' + pName + '"/></a>');
            });
            //end function
            // Zoom In non-touch
            $(document.body).on('mouseenter', '.sp-non-touch .sp-large', function (event) {               
                pName=$('a img ', this).attr('alt');
                var largeUrl = $('a', this).attr('href');
                $(this).append('<div class="sp-zoom"><img src="' + largeUrl + '" alt="' + pName + '"/></div>');
                $(this).find('.sp-zoom').fadeIn(250);
                event.preventDefault();
            });

            // Zoom Out non-touch
            $(document.body).on('mouseleave', '.sp-non-touch .sp-large', function (event) {
                $(this).find('.sp-zoom').fadeOut(250, function () {
                    $(this).remove();
                });
                event.preventDefault();
            });

            // Open in Lightbox non-touch
            $(document.body).on('click', '.sp-non-touch .sp-zoom', function (event) {
                var currentImg = $(this).html(),
                    thumbAmt = $(this).parents('.sp-wrap').find('.sp-thumbs a').length,
                    currentThumb = ($(this).parents('.sp-wrap').find('.sp-thumbs .sp-current').index()) + 1;
                $(this).parents('.sp-wrap').addClass('sp-selected');
                $('body').append("<div class='sp-lightbox' data-currenteq='" + currentThumb + "'>" + currentImg + " <span class='closediv' onclick='closeModal();'></span></div>");

                if (thumbAmt > 1) {
                    $('.sp-lightbox').append("<a href='#' id='sp-prev'></a><a href='#' id='sp-next'></a>");
                    if (currentThumb == 1) {
                        $('#sp-prev').css('opacity', '.1');
                    } else if (currentThumb == thumbAmt) {
                        $('#sp-next').css('opacity', '.1');
                    }
                }
                $('.sp-lightbox').fadeIn();
                event.preventDefault();
            });

            // Open in Lightbox touch
            $(document.body).on('click', '.sp-large a', function (event) {                
                var currentImg = $(this).attr('href'),
                    thumbAmt = $(this).parents('.sp-wrap').find('.sp-thumbs a').length,
                    currentThumb = ($(this).parents('.sp-wrap').find('.sp-thumbs .sp-current').index()) + 1;
                //pName = $(this).attr('alt');
                $(this).parents('.sp-wrap').addClass('sp-selected');
                $('body').append('<div class="sp-lightbox" data-currenteq="' + currentThumb + '"><img src="' + currentImg + '" alt="' + pName + '"/><span class="closediv" onclick="closeModal();"></span></div>');

                if (thumbAmt > 1) {
                    $('.sp-lightbox').append("<a href='#' id='sp-prev'></a><a href='#' id='sp-next'></a>");
                    if (currentThumb == 1) {
                        $('#sp-prev').css('opacity', '.1');
                    } else if (currentThumb == thumbAmt) {
                        $('#sp-next').css('opacity', '.1');
                    }
                }
                $('.sp-lightbox').fadeIn();
                event.preventDefault();
            });

            // Pagination Forward
            $(document.body).on('click', '#sp-next', function (event) {
                event.stopPropagation();
                var currentEq = $('.sp-lightbox').data('currenteq'),
                    totalItems = $('.sp-selected .sp-thumbs a').length;

                if (currentEq >= totalItems) {
                } else {
                    var nextEq = currentEq + 1,
                    newImg = $('.sp-selected .sp-thumbs').find('a:eq(' + currentEq + ')').attr('href'),
                    newThumb = get_url_from_background($('.sp-selected .sp-thumbs').find('a:eq(' + currentEq + ')').css('backgroundImage'));
                    if (currentEq == (totalItems - 1)) {
                        $('#sp-next').css('opacity', '.1');
                    }
                    $('#sp-prev').css('opacity', '1');
                    $('.sp-selected .sp-current').removeClass();
                    $('.sp-selected .sp-thumbs a:eq(' + currentEq + ')').addClass('sp-current');
                    $('.sp-selected .sp-large').empty().append('<a href=' + newImg + '><img src="' + newThumb + '" alt="' + pName + '"/></a>');
                    $('.sp-lightbox img').fadeOut(250, function () {
                        $(this).remove();
                        $('.sp-lightbox').data('currenteq', nextEq).append('<img src="' + newImg + '" alt="' + pName + '"/>');
                        $('.sp-lightbox img').hide().fadeIn(250);
                    });
                }

                event.preventDefault();
            });

            // Pagination Backward
            $(document.body).on('click', '#sp-prev', function (event) {

                event.stopPropagation();
                var currentEq = $('.sp-lightbox').data('currenteq'),
                    currentEq = currentEq - 1;
                if (currentEq <= 0) {
                } else {
                    if (currentEq == 1) {
                        $('#sp-prev').css('opacity', '.1');
                    }
                    var nextEq = currentEq - 1,
                    newImg = $('.sp-selected .sp-thumbs').find('a:eq(' + nextEq + ')').attr('href'),
                    newThumb = get_url_from_background($('.sp-selected .sp-thumbs').find('a:eq(' + nextEq + ')').css('backgroundImage'));
                    $('#sp-next').css('opacity', '1');
                    $('.sp-selected .sp-current').removeClass();
                    $('.sp-selected .sp-thumbs a:eq(' + nextEq + ')').addClass('sp-current');
                    $('.sp-selected .sp-large').empty().append('<a href=' + newImg + '><img src="' + newThumb + '" alt="' + pName + '"/></a>');
                    $('.sp-lightbox img').fadeOut(250, function () {
                        $(this).remove();
                        $('.sp-lightbox').data('currenteq', currentEq).append('<img src="' + newImg + '" alt="' + pName + '"/>');
                        $('.sp-lightbox img').hide().fadeIn(250);
                    });
                }
                event.preventDefault();
            });


            // Close Lightbox
            $(document.body).on('click', '.sp-lightbox', function () {
                closeModal();
            });

            // Close on Esc
            $(document).keydown(function (e) {
                if (e.keyCode == 27) {
                    closeModal();
                    return false;
                }
            });

            function closeModal() {
                $('.sp-selected').removeClass('sp-selected');
                $('.sp-lightbox').fadeOut(function () {
                    $(this).remove();
                });
            }


            // Panning zoomed image (non-touch)

            $('.sp-large').mousemove(function (e) {
                var viewWidth = $(this).width(),
                    viewHeight = $(this).height(),
                    largeWidth = $(this).find('.sp-zoom').width(),
                    largeHeight = $(this).find('.sp-zoom').height(),
                    parentOffset = $(this).parent().offset(),
                    relativeXPosition = (e.pageX - parentOffset.left),
                    relativeYPosition = (e.pageY - parentOffset.top),
                    moveX = Math.floor((relativeXPosition * (viewWidth - largeWidth) / viewWidth)),
                    moveY = Math.floor((relativeYPosition * (viewHeight - largeHeight) / viewHeight));

                $(this).find('.sp-zoom').css({
                    left: moveX,
                    top: moveY
                });

            });

            function get_url_from_background(bg) {
                return bg.match(/url\([\"\']{0,1}(.+)[\"\']{0,1}\)+/i)[1];
            }
        }
    });
})(jQuery);

