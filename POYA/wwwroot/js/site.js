// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$("table tbody tr td a,table thead tr th").css({ "word-break": "break-all", "word-wrap": "break-word" });

$(document).ready(function () {

    var _CULTURE = window.CULTURE;
    if (_CULTURE === undefined) {
        _CULTURE = Cookies.get("CULTURE");
        if (_CULTURE === undefined) {
            _CULTURE = "zh-CN";
        }
        window.CULTURE = _CULTURE;
    }
    var _THEME = window.THEME;
    if (_THEME === undefined) {
        _THEME = Cookies.get("THEME");
        if (_THEME === undefined) {
            _THEME = "Light";
        }
        window.THEME = _THEME;
    }


    ChangeTheme(_THEME);

    $('#Language  option[value=' + _CULTURE + ']').attr("selected", true);
    $("#Language").on("change", function () {
        var _language = $('#Language  option:selected').val();
        window.CULTURE = _language;
        window.location.href = UpdateQueryString(window.location.href, "culture", _language);
    });
    $('#Theme  option[value=' + _THEME + ']').attr("selected", true);

    $("#Theme").on("change", function () {
        var _theme_ = $('#Theme  option:selected').val();
        ChangeTheme(_theme_);
        Cookies.set("THEME", _theme_, { expires: 365 });
    });

    window.cacheRefreshCode = Cookies.get("cacheRefreshCode");
    if (window.cacheRefreshCode === undefined) {
        window.cacheRefreshCode = Math.floor(Math.random() * 100 + 1);
        Cookies.set("cacheRefreshCode", window.cacheRefreshCode);
    }
    SetSrc();

    $(".table tbody tr td a").css({ "word-wrap": "word-wrap", "word-break": "break-all" });
});

function UpdateCacheData() {

    window.cacheRefreshCode = Math.floor(Math.random() * 100 + 1);
    Cookies.set("cacheRefreshCode", window.cacheRefreshCode);
    SetSrc();
}

function SetSrc() {
    var _UserAvatarImg = $(".nav-item .nav-link img");
    var _img_tag_ = $("#_UserAvatarForm img:first");
    _imgUrl_ = "/X_doveFiles/GetAvatar?random=" + String(window.cacheRefreshCode);
    _UserAvatarImg.attr("src", _imgUrl_);
    _img_tag_.attr("src", _imgUrl_);

    _UserAvatarImg.on("error", function () {
        _UserAvatarImg.attr("src", "/img/chat_with_friends_ico.png");
        return true;
    });
    _img_tag_.on("error", function () {
        _img_tag_.attr("src", "/img/chat_with_friends_ico.png");
        return true;
    });
}
function ChangeTheme(_theme_) {
    switch (_theme_) {
        case "Dark":
            $("#_body,footer").css({ "background-color": "black", "color": "#C7EDCC" });
            $(".table").css("color", "#C7EDCC");
            $("input[class=form-control],textarea[class=form-control]").css({ "background-color": "#C7EDCC" });
            $("button").css({ "color": "#C7EDCC" });
            $("select").css({ "background-color": "black", "color": "#C7EDCC" });
            break;
        case "Light":
            $("#_body,footer").css({ "background-color": "white", "color": "black" });
            $(".table").css("color", "black");
            $("input[class=form-control],textarea[class=form-control]").css({ "background-color": "white" });
            $("button").css({ "color": "black" });
            $("select").css({ "background-color": "white" });
            break;
        default:
            $("#_body,footer").css({ "background-color": "#C7EDCC", "color": "black" });
            $(".table").css("color", "black");
            $("button").css({ "color": "black" });
            $("select").css({ "background-color": "#C7EDCC", "color": "black" });
            $("input[type!='submit']").css({ "background-color": "white", "color": "black" });
            break;
    }
}

/*
 * FROM     https://stackoverflow.com/questions/5999118/how-can-i-add-or-update-a-query-string-parameter
 * THANK    https://stackoverflow.com/users/184/niyaz
 * @param {any} uri
 * @param {any} key
 * @param {any} value
 */
function UpdateQueryString(uri, key, value) {
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    uri = String(uri);
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        return uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    return uri + separator + key + "=" + value;

}

