(function ($) {

    $.fn.expandableContent = function (options) {

        // default configuration properties
        var defaults = {
            collapsedHeight: 100,
            showMoreInnerHtml: 'show more',
            showLessInnerHtml: 'show less',
            animationSpeed: 500
        };

        var options = $.extend(defaults, options);

        $(this).each(function () {
            if ($(this).outerHeight(true) <= options.collapsedHeight) {
                return;
            }
            $(this).css('overflow', 'hidden');
            $(this).append('<input type="hidden" class="originalHeight" value="' + $(this).outerHeight(true) + '"/>');
            $(this).after('<div class="expandableExpand" style="display:block;">' + '<img src="/Resources/Images/arrow-down.gif" />' + '</div>');
            $(this).after('<div class="expandableCollapse" style="display:none;">' + '<img src="/Resources/Images/arrow-up.gif" /> ' + '</div>');
            $(this).css('height', options.collapsedHeight);
            $(this).addClass('expandableContentExpandable');
        });

        $(".expandableExpand, .expandableCollapse").click(function () {
            $(this).prevAll('.expandableContentExpandable:first').each(function () {
                var height;
                var overflow;
                if ($(this).outerHeight(true) == options.collapsedHeight) {
                    height = $(this).find('input.originalHeight').val();
                } else {
                    height = options.collapsedHeight;
                }
                if (height < options.collapsedHeight) {
                    return;
                }
                $(this).animate({
                    height: height
                }, options.animationSpeed, function () {
                });
            });
            $(this).toggle();
            if ($(this).prev().hasClass('expandableExpand') || $(this).prev().hasClass('expandableCollapse')) {
                $(this).prev().toggle();
            }
            if ($(this).next().hasClass('expandableExpand') || $(this).next().hasClass('expandableCollapse')) {
                $(this).next().toggle();
            }
        });
    }

})(jQuery);