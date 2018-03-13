$(function () {
    if ($(window).width() < 414) {
        $('#Mobile-Feed').show();
        $('#Mobile-Bookmarks').show();
        $('#Mobile-Settings').show();
        $('#Mobile-Logout').show();

        $('#Desktop-Categories').hide();
        $('#Desktop-Sources').hide();
        $('#Desktop-Custom').hide();
    }
    else { $('#Login-Partial').show(); }
});

$(function () {
    var topicList = [];
    var idList = [];

    $('#Publishers button').click(function () {
        var id = $(this).attr('id');
        var name = $(this).attr('name');
        var topic = { 'Id': id, 'Name': name, 'Type': 'publisher' };

        if ($.inArray(id, idList) == -1) {
            idList.push(id);
            topicList.push(topic);

            $('#topicsWindow').show();
            $('#warning').hide();
            var chip = '<span class="chip" id=' + id + ' role="button">' + topic.Name + '</span>';

            $('#selectedTopics').append(chip);
            $(this).removeClass('btn-primary').addClass('btn-success');
            $(this).find('span').removeClass('fa fa-plus').addClass('fa fa-check')
        }
        else {
            idList = $.grep(idList, function (value) {
                return value != id;
            });
            topicList = topicList.filter(function (el) {
                return el.Id !== id;
            });
            console.log(topicList);
            $('#selectedTopics').empty();
            if (idList.length > 0) {
                for (var i = 0; i < idList.length; i++) {
                    var chip = '<span class="chip" role="button">' + topicList[i].Name + '</span>';
                    $('#selectedTopics').append(chip);
                }
            }
            $(this).removeClass('btn-success').addClass('btn-primary');
            $(this).find('span').removeClass('fa fa-check').addClass('fa fa-plus')
            if (idList.length == 0) {
                $('#warning').show();
            }
        }
    });

    $('#Categories button').click(function () {
        var id = $(this).attr('id');
        var name = $(this).attr('name');
        var topic = { 'Id': id, 'Name': name, 'Type': 'category' };

        if ($.inArray(id, idList) == -1) {
            idList.push(id);
            topicList.push(topic);

            $('#topicsWindow').show();
            $('#warning').hide();
            var chip = '<span class="chip" id=' + id + ' role="button">' + topic.Name + '</span>';

            $('#selectedTopics').append(chip);
            $(this).removeClass('btn-primary').addClass('btn-success');
            $(this).find('span').removeClass('fa fa-plus').addClass('fa fa-check')
        }
        else {
            idList = $.grep(idList, function (value) {
                return value != id;
            });
            topicList = topicList.filter(function (el) {
                return el.Id !== id;
            });
            console.log(topicList);
            $('#selectedTopics').empty();
            if (idList.length > 0) {
                for (var i = 0; i < idList.length; i++) {
                    var chip = '<span class="chip" role="button">' + topicList[i].Name + '</span>';
                    $('#selectedTopics').append(chip);
                }
            }
            $(this).removeClass('btn-success').addClass('btn-primary');
            $(this).find('span').removeClass('fa fa-check').addClass('fa fa-plus')
            if (idList.length == 0) {
                $('#warning').show();
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

    $("#saveFollowing").click(function () {
        var url = $(this).data('request-url');

         $.post(url, {topicList}).done(window.location = '/Manage/Following')
    });
});