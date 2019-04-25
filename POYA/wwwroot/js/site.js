// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
//  site.js will be used in all view maybe


/**
 * Handle Data
 */
class Data_ {
    /**
     * - Initialize some data
     * */
    Initial() {

    }

    /**
     * Set key and value in querystring
     * REFERENCE    https://stackoverflow.com/questions/5999118/how-can-i-add-or-update-a-query-string-parameter
     * THANK        https://stackoverflow.com/users/184/niyaz
     * @param {string} uri - The url, default value is window.location.href
     * @param {string} key - The querystring key
     * @param {string} value - The querystring value of value
     * @returns {string} Return the new url
     * */
    SetQueryString(uri = window.location.href, key, value) {
        var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
        var separator = uri.indexOf('?') !== -1 ? "&" : "?";
        if (uri.match(re)) {
            return uri.replace(re, '$1' + key + "=" + value + '$2');
        }
        return uri + separator + key + "=" + value;
    }

}

/**
 * Handle UI
 * */
class UI_ {
    /**
     * - Initialize some UI
     * */
    Initial() {
        this.ChangeTheme(false);    //    _Value_.GetTheme());
        this.ChangeLanguageTagShow();
        this.ChangeThemeTagShow();
        this.SetUserAvatar();
    }
    /**
     * - Set the src value of user avatar img tag
     * */
    SetUserAvatar() {
        $("[name='UserAvatarImg']").attr("src", "/Home/GetAvatar?random=" + window.CacheRefreshCode);
    }
    /**
     * Set the value of theme select tag
     * */
    ChangeThemeTagShow() {
        $('#Theme  option[value=' + _Value_.GetTheme() + ']').attr("selected", true);
    }
    /**
     * Set the value of languge select tag
     * */
    ChangeLanguageTagShow() {
        $('#Language  option[value=' + _Value_.GetCulture() + ']').attr("selected", true);
    }
    /**
     * Change the theme
     * @param {boolean} IsGetValueFromSelectTag - Determine get the theme value from theme select tag or not, the default value is false
     * */
    ChangeTheme(IsGetValueFromSelectTag = false) { //  _theme_ = "Light"  // * @param {string} _theme_ -The theme >_ "Light" | "Dark" | "Care", The default value is "Light"
        var _theme_ = IsGetValueFromSelectTag ? _Value_.GetThemeInSelectTag() : _Value_.GetTheme();
        if (IsGetValueFromSelectTag) {
            Cookies.set(_Value_.THEME, _theme_, { expires: 365 });
        }
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
    /**
     * - Refresh the user avatar
     * */
    RefreshUserAvatar() {
        window.CacheRefreshCode = Math.floor(Math.random() * 100 + 1);
        $("[name='UserAvatarImg']").attr("src", "/Home/GetAvatar?random=" + window.CacheRefreshCode);
    }

}
/**
 * The network access service
 * */
class Service_ {
    /**
     * - Change the language by cluture
     * */
    ChangeLanguage() {
        var _language = $(_Value_.LanguageId + '  option:selected').val();
        window.CULTURE = _language;
        //  Cookies is reset after href is changed
        window.location.href = _Data_.SetQueryString(window.location.href, "culture", _language);
        return;
    }
}

/**
 * Some values
 * */
class Value_ {
    //  #region PROPERTY
    /**
     *  "#Language"
     */
    LanguageId = "#Language";
    /**
     * "THEME"
     */
    THEME = "THEME";
    /**
     * "CULTURE"
     */
    CULTURE = "CULTURE";
    //  #endregion

    //  #region METHOD
     
    /**
     * - Get the theme string in select tag
     * @returns {string} - Return the theme string
     * */
    GetThemeInSelectTag() {
        return String($('#Theme  option:selected').val());
    }

    /**
     * - Get the Theme string in window or Cookies
     * @returns {string} - Return the theme string
     * */
    GetTheme() {
        var _THEME = window.THEME;
        if (_THEME === undefined) {
            _THEME = Cookies.get(this.THEME);
            if (_THEME === undefined) {
                _THEME = "Light";
                Cookies.set(this.THEME, _THEME);
            }
            window.THEME = _THEME;
        }
        return String(_THEME);
    }

    /**
     * Get the culture string in window or Cookies
     * @returns {string} -Return the culture string
     * */
    GetCulture() {
        var _CULTURE = window.CULTURE;
        if (_CULTURE === undefined) {
            _CULTURE = Cookies.get(this.CULTURE);
            if (_CULTURE === undefined) {
                _CULTURE = "zh-CN";
                Cookies.set(this.CULTURE, _CULTURE);
            }
            window.CULTURE = _CULTURE;
        }
        return String(_CULTURE);
    }
    //  #endregion
}


var _Data_ = new Data_();
var _UI_ = new UI_();
var _Value_ = new Value_();
var _Service_ = new Service_();

//  $("table tbody tr td a,table thead tr th").css({ "word-break": "break-all", "word-wrap": "break-word" });

$(document).ready(function () {
    _UI_.Initial();
    $("#Language").on("change", function () {
        _Service_.ChangeLanguage();
    });
    //  $('#Theme  option[value=' + _THEME + ']').attr("selected", true);
    $("#Theme").on("change", function () {
        _UI_.ChangeTheme(true); //  var _theme_ = $('#Theme  option:selected').val();  //  ChangeTheme(_theme_);
    });
});


//  #region TEMPORARY
/*
 * 


function _UpdateCacheData() {
    window.cacheRefreshCode = Math.floor(Math.random() * 100 + 1);
    Cookies.set("cacheRefreshCode", window.cacheRefreshCode);
    SetSrc();
}

function SetSrc() {
    var _UserAvatarImg = $(".nav-item .nav-link img");
    var _img_tag_ = $("#_UserAvatarForm img:first");
    _imgUrl_ = "/Home/GetAvatar?random=" + String(window.cacheRefreshCode);
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

 *
    /
    window.cacheRefreshCode = Cookies.get("cacheRefreshCode");
    if (window.cacheRefreshCode === undefined) {
        window.cacheRefreshCode = Math.floor(Math.random() * 100 + 1);
        Cookies.set("cacheRefreshCode", window.cacheRefreshCode);
    }
    SetSrc();
    $(".table tbody tr td a").css({ "word-wrap": "word-wrap", "word-break": "break-all" });
    
 * 
var _CULTURE = window.CULTURE;
    if (_CULTURE === undefined) {
        _CULTURE = Cookies.get("CULTURE");
        if (_CULTURE === undefined) {
            _CULTURE = "zh-CN";
        }
        window.CULTURE = _CULTURE;
    }

 */
//  #endregion