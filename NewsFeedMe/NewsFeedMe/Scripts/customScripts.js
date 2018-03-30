$(function () {
    if ($(window).width() < 415) {
        $('#Mobile-Feed').show();
        $('#Mobile-Bookmarks').show();
        $('#Mobile-Settings').show();
        $('#Mobile-Logout').show();

        $('#Desktop-Categories').hide();
        $('#Desktop-Sources').hide();
        $('#Desktop-Custom').hide();

        //$('*#panel').removeClass('container-fluid');
        $('#body').removeClass('container');
        $('#body').removeClass('body-content');

        $('#feedNav').addClass('navbar-fixed-bottom');
    }
    else { $('#Login-Partial').show(); }

    $('#feedBtn').click(function () {
        $(this).removeClass('btn-default').addClass('btn-primary');
        $('#newsBtn').removeClass('btn-primary').addClass('btn-default');
        $('#News').hide();
        $('#Mixed').show();
        $('html, body').animate({ scrollTop: 0 }, 'fast');
    });
    $('#newsBtn').click(function () {
        $(this).removeClass('btn-default').addClass('btn-primary').addClass('container-fluid');
        $('#feedBtn').removeClass('btn-primary').addClass('btn-default');
        $('#Mixed').hide();
        $('#News').show();
        $('html, body').animate({ scrollTop: 0 }, 'fast');
    });
});

$(function () {
    var topicList = [];
    var idList = [];

    $('#Publishers button').click(function () {
        var id = $(this).attr('id');
        var name = $(this).attr('name');
        var topic = { 'Id': id, 'Name': name, 'Type': 'publisher' };

        if ($.inArray(id, idList) === -1) {
            idList.push(id);
            topicList.push(topic);

            $('#topicsWindow').show();
            $('#warning').hide();
            var chip = '<span class="chip" id=' + id + ' role="button">' + topic.Name + '</span>';

            $('#selectedTopics').append(chip);

            $(this).removeClass('btn-primary').addClass('btn-success');
            $(this).find('text').empty();
            $(this).find('span').removeClass('fa fa-plus').addClass('fa fa-check');
            $(this).find('text').text(' Following!')
        }
        else {
            idList = $.grep(idList, function (value) {
                return value !== id;
            });
            topicList = topicList.filter(function (el) {
                return el.Id !== id;
            });

            $('#selectedTopics').empty();
            if (idList.length > 0) {
                for (var i = 0; i < idList.length; i++) {
                    chip = '<span class="chip" role="button">' + topicList[i].Name + '</span>';
                    $('#selectedTopics').append(chip);
                }
            }
            $(this).removeClass('btn-success').addClass('btn-primary');
            $(this).find('span').removeClass('fa fa-check').addClass('fa fa-plus');
            $(this).find('text').empty();
            $(this).find('text').text(' Follow');
            if (idList.length === 0) {
                $('#topicsWindow').hide();
            }
        }
    });

    $('#Categories button').click(function () {
        var id = $(this).attr('id');
        var name = $(this).attr('name');
        var topic = { 'Id': id, 'Name': name, 'Type': 'category' };

        if ($.inArray(id, idList) === -1) {
            idList.push(id);
            topicList.push(topic);

            $('#topicsWindow').show();
            $('#warning').hide();
            var chip = '<span class="chip text-capitalize" id=' + id + ' role="button">' + topic.Name + '</span>';

            $('#selectedTopics').append(chip);
            $(this).removeClass('btn-primary').addClass('btn-success');
            $(this).find('text').empty();
            $(this).find('span').removeClass('fa fa-plus').addClass('fa fa-check');
            $(this).find('text').text(' Following!')
        }
        else {
            idList = $.grep(idList, function (value) {
                return value !== id;
            });
            topicList = topicList.filter(function (el) {
                return el.Id !== id;
            });

            $('#selectedTopics').empty();
            if (idList.length > 0) {
                for (var i = 0; i < idList.length; i++) {
                    chip = '<span class="chip text-capitalize" role="button">' + topicList[i].Name + '</span>';
                    $('#selectedTopics').append(chip);
                }
            }
            $(this).removeClass('btn-success').addClass('btn-primary');
            $(this).find('span').removeClass('fa fa-check').addClass('fa fa-plus');
            $(this).find('text').empty();
            $(this).find('text').text(' Follow');
            if (idList.length === 0) {
                $('#topicsWindow').hide();
            }
        }
    });

    $('#categorySwap').click(function () {
        $(this).removeClass('btn-default').addClass('btn-primary');
        $('#newsSwap').removeClass('btn-primary').addClass('btn-default');
        $('#Publishers').hide();
        $('#Categories').show();
    });

    $('#newsSwap').click(function () {
        $(this).removeClass('btn-default').addClass('btn-primary');
        $('#categorySwap').removeClass('btn-primary').addClass('btn-default');
        $('#Publishers').show();
        $('#Categories').hide();
    });

    $("#saveFollowing").click(async function () {
        var url = $(this).data('request-url');
        
       await $.ajax({
            url: url,
            type: 'POST',
            data: {
                topicList: topicList
            },
           success: function (response) {
               if (response.success) {
                   window.location.reload(true);
               }
           }
        });
    });

    $('#searchBar').keyup(function () {
        var query = $('#searchBar').val();
        console.log(query);
        $('*[id*=' + query + ']:visible').each(function () {
            
            console.log(this);
            
        });
    })
});