
(function () {
    var cookieConsentButton = $("#cookieConsent button[data-cookie-string]");
    cookieConsentButton.on("click",(e)=>{    document.cookie = button.dataset.cookieString;} );
    
})();

(function(){

    var IsAvatarUploaded = false;
    var _allowedAvatarFileExtensions = ["jpg", "jpeg","gif"];
    var _isExtensionNeedChecking=false;


    function _UserAvatarFormClick() {
        if (IsAvatarUploaded) return;
        $('#_UserAvatarInput').click();
    }
    
    
    function UpdateUserAvatar() {
        $("[name='User_Avatar_Img']").attr("src", "/Home/GetAvatar?random=" + Math.floor(Math.random() * 100 + 1));
        $("[name='UserAvatarImg']").attr("src", "/Home/GetAvatar?random=" + Math.floor(Math.random() * 100 + 1));
    }

    function _UserAvatarInputOnChange() {
        var _file_ = document.getElementById("_UserAvatarInput").files[0];
        var _UploadAvatarMsgEle_ = $("#_UploadAvatarMsgEle");
        if (_file_.size > 1024 * 1024) {
            _UploadAvatarMsgEle_.html($('.user-info-row').attr('data-avatar-size-limit'));
            return;
        }
        var _value_ = $("#_UserAvatarInput").val();
        var index = _value_.lastIndexOf(".");
        var _extension = String(_value_.substr(index + 1)).toLowerCase();
        if(_isExtensionNeedChecking){
            var _isExtensionAllowed = false;
            _allowedAvatarFileExtensions.forEach((value, index, array) => {
                if (_extension == value) {
                    _isExtensionAllowed = true;
                    return false;
                }
            });
            if (!_isExtensionAllowed) {
                var _tip=$('.user-info-row').attr('data-extension-limit_');
                _allowedAvatarFileExtensions.forEach((value, index, array)=>{_tip+=`&#128504; ${value} `;});
                _UploadAvatarMsgEle_.html(_tip);
                return;
            }
        }

        if ($("#_UserAvatarInput").val() == "") return;
        var AvatarForm = new FormData();
        AvatarForm.append("avatarFile", _file_);
        $.ajax({
            url: "/Home/UploadAvatar",
            type: "POST",
            data: AvatarForm,
            contentType: false,
            processData: false,
            dataType: "json",
            headers: {
                [$('.user-info-row').attr('data-custom-header-name')]: $('.user-info-row').attr('data-csrf')
            },
            success: function (data) {
                if (data["status"] == true) {
                    _UploadAvatarMsgEle_.html($('.user-info-row').attr('data-upload-successfully')+`!`);
                    UpdateUserAvatar();
                } else {
                    if (data["msg"] == "ExceedSize") {
                        _UploadAvatarMsgEle_.html($('.user-info-row').attr('data-avatar-size-limit'));
                    } else if (data["msg"] == "RefuseExtension") {
                        _UploadAvatarMsgEle_.html($('.user-info-row').attr('data-extension-limit'));
                    }
                }
            },
            error: function (msg) {
                _UploadAvatarMsgEle_.html($('.user-info-row').attr('data-error'));
            }
        });
        $("#_UserAvatarInput").val(null);
    }


    $(document).ready(function () {
        $("input").attr("autocomplete", "off");
        $("#_UserAvatarForm img:first").on("click", function () {
            _UserAvatarFormClick();
            event.preventDefault();
        });
        $("#_UserAvatarInput").on("change", function () {
            _UserAvatarInputOnChange();
            event.preventDefault();
        });
        if(!_isExtensionNeedChecking)$(`#_UserAvatarInput`).attr(`accept`,`image/*`);
    });
})();

(function(){

})();