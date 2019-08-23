// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
//  site.js will be used in all view maybe


var THEME_String = "THEME";

var CULTURE_String=`CULTURE`;

var _Language_String=`_Language`;

var AspNetCore_Culture_String=`.AspNetCore.Culture`;



function PageSize_Input(){
    $("#_pageSize").on("input", function () {
        var _value = $(this).val();
        if (_value.length === 0) return;
        $(this).val(isNaN(_value) ? 8 :  _value>20?20:_value );
    });

}

function GetCulture() {
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

function   GetTheme() {
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

function   GetThemeInSelectTag() {
    return String($('#Theme  option:selected').val());
}


function ChangeTheme(IsGetValueFromSelectTag = false) {
    var _theme_ = IsGetValueFromSelectTag ? GetThemeInSelectTag() : GetTheme();
    if (IsGetValueFromSelectTag) {
        Cookies.set(THEME_String, _theme_, { expires: 365 });
    }
    location.reload();
}


function  ChangeLanguage() {
    var _Language_=$(`#${_Language_String} option:selected`).val().toString();
    if(_Language_==Cookies.get(CULTURE_String))return;
    Cookies.set(CULTURE_String,`${_Language_}`);
    location.reload()
    return;
}

function KeepLogin() {
    setInterval(function () {
        $.ajax({
            url: "/Home/KeepLogin",
            type: "GET"
        });
        console.log("&#128139;");
    }, 5 * 60 * 1000);

}


function PageSizeKeyPress() {
    var keyCode = event.keyCode ? event.keyCode : event.which ? event.which : event.charCode;
    if (Number(keyCode) === 13) {
        var _val = Number($(`#_pageSize`).val().replace(/[^0-9]/ig, ""));
        Cookies.set("PageSize",_val<8?8:_val);
        location.reload();
    }
}

/**Generate new guid
 */
function NewGuid() {
    return `${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }-${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }-4${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1).substr(0, 3)
        }-${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }-${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }`.toLowerCase();
}

/**Set key and value in querystring
 * REFERENCE    https://stackoverflow.com/questions/5999118/how-can-i-add-or-update-a-query-string-parameter
 * THANK        https://stackoverflow.com/users/184/niyaz
 * @param {string} uri - The url, default value is window.location.href
 * @param {string} key - The querystring key
 * @param {string} value - The querystring value of value
 * @returns {string} Return the new url
 * */
function SetQueryString(uri = window.location.href, key, value) {
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        return uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    return uri + separator + key + "=" + value;
}


/**Optime the storage size for showing
 * @param {number} byte -The byte
 * @returns {string} -Return the optimized string
 */
function OptimizeFileSizeShow (byte) {
    return byte < 1024 ? `${byte}b`
        : byte < Math.pow(1024, 2) ? `${(byte / 1024).toFixed(1)}k`
            : byte < Math.pow(1024, 3) ? `${(byte / Math.pow(1024, 2)).toFixed(1)}M`
                : `${(byte / Math.pow(1024, 3)).toFixed(1)}G`;
};


$(document).ready(function () {

    $("[name='UserAvatarImg']").attr("src", "/Home/GetAvatar?random=" +NewGuid());

    $("td,td *,th,th *").css({ "word-wrap": "break-word", "break-word": "break-word" });

    $(`#_Language  option[value='${GetCulture()}']`).attr("selected", true);

    $("#_Language").on("change", ()=>{ChangeLanguage();});

    $("#Theme").on("change", ()=>{ ChangeTheme(true); });

    $("#BackA,#BackEle").on("click", ()=>{ history.go(-1);});

    $("#_pageSize").keypress(()=>{ PageSizeKeyPress(); });
    
    PageSize_Input();
    
    KeepLogin();
    
});