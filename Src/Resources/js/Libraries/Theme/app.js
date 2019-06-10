'use strict';

/*------------------------------------------------
    Page Loader
-------------------------------------------------*/
$(window).on('load', function () {
    setTimeout(function () {
        $('.page-loader').fadeOut();
    }, 500);
});

$(document).ready(function () {
    /*------------------------------------------------
        Search
    -------------------------------------------------*/

    // Active Stat
    $('body').on('focus', '.search__text', function () {
        $(this).closest('.search').addClass('search--focus');
    });

    // Clear
    $('body').on('blur', '.search__text', function () {
        $(this).val('');
        $(this).closest('.search').removeClass('search--focus');
    });

    // Autocomplete
    $("#nav_search").autocomplete({
        source: (request, response) => {
            $.ajax({
                url: `/search?q=${request.term}`,
                dataType: "json",
                success: function (data) {
                    response(data);
                }
            });
        },
        minLength: 3,
        delay: 500
    });

    // Admin settings ship search
    $("#ship_search").autocomplete({
        source: (request, response) => {
            $.ajax({
                url: `/admin/settings/ships/search?q=${request.term}`,
                dataType: "json",
                success: function (data) {
                    response(data);
                }
            });
        },
        minLength: 3,
        delay: 500
    });
    
    /*------------------------------------------------
        Sidebar toggle menu
    -------------------------------------------------*/
    $('body').on('click', '.navigation__sub > a', function (e) {
        e.preventDefault();

        $(this).parent().toggleClass('navigation__sub--toggled');
        $(this).next('ul').slideToggle(250);
    });

    /*------------------------------------------------
       Custom Side Bars
    -------------------------------------------------*/
    // Close sidebar if the user clicks outside of it.
    $(document).click(function (e) {
        if ($(".sidebar-special.active").length > 0) {
            if (!e.target.closest(".sidebar-special")) {
                $('.sidebar-special').removeClass('active');
            }
        }
    });


    /*------------------------------------------------
        Form group bar
    -------------------------------------------------*/
    if($('.form-group--float')[0]) {
        $('.form-group--float').each(function () {
            var p = $(this).find('.form-control').val()

            if(!p.length == 0) {
                $(this).find('.form-control').addClass('form-control--active');
            }
        });

        $('body').on('blur', '.form-group--float .form-control', function(){
            var i = $(this).val();

            if (i.length == 0) {
                $(this).removeClass('form-control--active');
            }
            else {
                $(this).addClass('form-control--active');
            }
        });
    }


    /*------------------------------------------------
        Stay active Dropdown menu
    -------------------------------------------------*/
    $('body').on('click', '.dropdown-menu--active', function (e) {
        e.stopPropagation();
    });

    /*-------------------------------------------------
     Auto complete for react text inputs
     --------------------------------------------------*/
    $(".account-lookup").autocomplete({
        source: (request, response) => {
            $.ajax({
                url: `/search?q=${request.term}&filter=account`,
                dataType: "json",
                success: function (data) {
                    response(data);
                }
            });
        },
        minLength: 3,
        delay: 500
    });

    $(".pilot-lookup").autocomplete({
        source: (request, response) => {
            $.ajax({
                url: `/search?q=${request.term}&filter=pilot`,
                dataType: "json",
                success: function (data) {
                    response(data);
                }
            });
        },
        minLength: 3,
        delay: 500
    });

    $(".lookup").autocomplete({
        source: (request, response) => {
            $.ajax({
                url: `/search?q=${request.term}`,
                dataType: "json",
                success: function (data) {
                    response(data);
                }
            });
        },
        minLength: 3,
        delay: 500
    });
});