var wfp = wfp || {};

wfp.appName = 'Encore Piano Moving';

var imagesPath = '/images/';

//$.fn.validator.Constructor.FOCUS_OFFSET = 100;

// create a general purpose namespace method
// this will allow us to create namespace a bit easier
wfp.createNS = function (namespace) {
    //debugger;
    var nsparts = namespace.split(".");
    var parent = wfp;

    // we want to be able to include or exclude the root namespace 
    // So we strip it if it's in the namespace
    if (nsparts[0] === "wfp") {
        nsparts = nsparts.slice(1);
    }

    // loop through the parts and create 
    // a nested namespace if necessary
    for (var i = 0; i < nsparts.length; i++) {
        var partname = nsparts[i];
        // check if the current parent already has 
        // the namespace declared, if not create it
        if (typeof parent[partname] === "undefined") {
            parent[partname] = {};
        }
        // get a reference to the deepest element 
        // in the hierarchy so far
        parent = parent[partname];
    }
    // the parent is now completely constructed 
    // with empty namespaces and can be used.
    return parent;
};

wfp.createNS("wfp.security");

//success, info, warning, danger
wfp.showSuccess = function (message) {
    toastr.options.closeButton = true;
    toastr.options.closeMethod = 'fadeOut';
    toastr.options.closeDuration = 1000;
    toastr.options.closeEasing = 'swing';
    toastr.options.positionClass = 'toast-bottom-right';
    toastr.success(message, wfp.appName);
}

wfp.showError = function (message) {
    toastr.options.closeButton = true;
    toastr.options.closeMethod = 'fadeOut';
    toastr.options.closeDuration = 1000;
    toastr.options.closeEasing = 'swing';
    toastr.options.positionClass = 'toast-bottom-right';
    toastr.error(message, wfp.appName);
}

wfp.showSwal = function (message, yesCallback, noCallback) {
    swal({ title: message, text: "You will not be able to undo", type: "warning", showLoaderOnConfirm: true, showCancelButton: true, cancelButtonText: "No", confirmButtonText: "Yes", confirmButtonColor: "#DD6B55" },
        function (isConfirm) {
            if (isConfirm) {
                //debugger;
                if (yesCallback)
                    yesCallback();
                else
                    return true;
            } else {
                if (noCallback)
                    noCallback();
                else
                    return false;
            }});
}

wfp.getImagesPath = function() {
    return imagesPath;
}

// wrApper function to  block element(indicate loading)
wfp.blockUI = function (options) {
    //debugger;
    options = $.extend(true, {}, options);
    var html = '';
    if (options.animate) {
        html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '">' + '<div class="block-spinner-bar"><div class="bounce1"></div><div class="bounce2"></div><div class="bounce3"></div></div>' + '</div>';
    } else if (options.iconOnly) {
        html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '"><img src="' + this.getImagesPath() + 'loading-spinner-blue.gif" align=""></div>';
    } else if (options.textOnly) {
        html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '"><span>&nbsp;&nbsp;' + (options.message ? options.message : 'LOADING...') + '</span></div>';
    } else {
        html = '<div class="loading-message ' + (options.boxed ? 'loading-message-boxed' : '') + '"><img src="' + this.getImagesPath() + 'loading-spinner-blue.gif" align=""><span>&nbsp;&nbsp;' + (options.message ? options.message : 'LOADING...') + '</span></div>';
    }

    if (options.target) { // element blocking
        var el = $(options.target);
        if (el.height() <= ($(window).height())) {
            options.cenrerY = true;
        }
        el.block({
            message: html,
            baseZ: options.zIndex ? options.zIndex : 9999999,
            centerY: options.cenrerY !== undefined ? options.cenrerY : false,
            css: {
                top: '10%',
                border: '0',
                padding: '0',
                backgroundColor: 'none'
            },
            overlayCSS: {
                backgroundColor: options.overlayColor ? options.overlayColor : '#555',
                opacity: options.boxed ? 0.05 : 0.1,
                cursor: 'wait'
            }
        });
    } else { // page blocking
        $.blockUI({
            message: html,
            baseZ: options.zIndex ? options.zIndex : 9999999,
            css: {
                border: '0',
                padding: '0',
                backgroundColor: 'none'
            },
            overlayCSS: {
                backgroundColor: options.overlayColor ? options.overlayColor : '#555',
                opacity: options.boxed ? 0.05 : 0.1,
                cursor: 'wait'
            }
        });
    }
}

// wrApper function to  un-block element(finish loading)
wfp.unblockUI = function (target) {
    if (target) {
        $(target).unblock({
            onUnblock: function() {
                $(target).css('position', '');
                $(target).css('zoom', '');
            }
        });
    } else {
        $.unblockUI();
    }
}

wfp.restTooltips = function ()
{
    $('[data-tooltip="tooltip"]').tooltip();
}

//public function to get a paremeter by name from URL
wfp.getURLParameter = function(paramName) {
    var searchString = window.location.search.substring(1),
        i, val, params = searchString.split("&");

    for (i = 0; i < params.length; i++) {
        val = params[i].split("=");
        if (val[0] == paramName) {
            return unescape(val[1]);
        }
    }
    return null;
}

wfp.getQueryParams = function (qs) {
    qs = qs.split('+').join(' ');

    var params = {},
        tokens,
        re = /[?&]?([^=]+)=([^&]*)/g;

    while (tokens = re.exec(qs)) {
        params[decodeURIComponent(tokens[1])] = decodeURIComponent(tokens[2]);
    }

    return params;
}

// check for device touch support
wfp.isTouchDevice = function() {
    try {
        document.createEvent("TouchEvent");
        return true;
    } catch (e) {
        return false;
    }
}

wfp.loadPartial = function (partialURL) {
    debugger;
    //wfp.blockUI({
    //    target: $('#partialLoadingDiv'),
    //    boxed: true,
    //    message: 'Loading...',
    //    dataType:'html',
    //});

    $.ajax({
        url: partialURL,
        type: "GET",
        data: {},
        success: function (response) {
            $("#partialLoadingDiv").empty();
            $("#partialLoadingDiv").html(response);
        },
        error: function (xhr, desc, err) {
            console.log(xhr);
            alert(err + "1");
        }
    });

    //wfp.unblockUI($('#partialLoadingDiv'))
}

wfp.redirectTo = function (controllerAction) {
    if (controllerAction) {
        setTimeout(
        function () {
            location.href = "http://" + $(location).attr('host') + controllerAction;
        }, 2000);
    } else {
        setTimeout(
        function () {
            window.location.reload();
        }, 2000);
    }
}

wfp.reload = function (controllerAction) {
    if(controllerAction)
        location.href = "http://" + $(location).attr('host') + controllerAction;
    else
        //window.location.reload();
        window.location.href = window.location.href;
}

wfp.validateEmail = function (email) {
    var re = /\S+@@\S+\.\S+/;
    return re.test(email);
}

wfp.validatePassword = function (password) {
    var pa = /(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{6,20})$/;
    return pa.test(password);
}

jQuery.fn.extend({
    disable: function (state) {
        return this.each(function () {
            var $this = $(this);
            if ($this.is('input, button, textarea, select'))
                this.disabled = state;
            else
                $this.toggleClass('disabled', state);
        });
    }
});

wfp.encodeQueryData = function (data) {
    let ret = [];
    for (let d in data)
        ret.push(encodeURIComponent(d) + '=' + encodeURIComponent(data[d]));
    return ret.join('&');
}

wfp.doReadonly = function (control) {
    
    $(document).on("focusin", $(control[0]), function () {
        debugger;
        $(this).prop('readonly', true);
    });

    $(document).on("focusout", $(control[0]), function () {
        $(this).prop('readonly', false);
    });
}

/**
* @param {string} filename The name of the file WITHOUT ending
*/
wfp.playSound = function (filename) {
    var filePath = '/assets/sounds/' + filename;
    document.getElementById("sound").innerHTML =
        '<audio autoplay="autoplay"><source src="' + filePath + '.mp3" type="audio/mpeg" /><source src="' + filePath + '.ogg" type="audio/ogg" /><embed hidden="true" autostart="true" loop="false" src="' + filePath + '.mp3" /></audio>';
}

Array.prototype.remove = function () {
    var what, a = arguments, L = a.length, ax;
    while (L && this.length) {
        what = a[--L];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }
    return this;
};