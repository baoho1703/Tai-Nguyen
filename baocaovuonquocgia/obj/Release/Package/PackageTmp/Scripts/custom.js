
validation = {
    maxlength: function (value, requirement) {
        var rt = {
            bool: value.length <= requirement,
            msg: 'This value is too long. It should have ' + (requirement) +' characters or fewer.'
        };
        return rt;
    },
    minlength: function (value, requirement) {
        var rt = {
            bool: value.length >= requirement,
            msg: 'This value is too short. It should have ' + (requirement)+' characters or more.'
        };
        return rt;
    },
    rangelength: function (value, requirement) {
        var rt = {
            bool: (value.length >= requirement[0] && value.length <= requirement[1]),
            msg: 'This value should be between ' + (requirement[0]) + ' and '+ (requirement[1])
        };
        return rt;
    },
    number: function (value) {
        var rt = {
            bool: /^-?(\d*\.)?\d+(e[-+]?\d+)?$/.test(value),
            msg: 'This value should be a valid number.'
        };
        return rt;
    },
    integer: function (value) {
        var rt = {
            bool: /^-?\d+$/.test(value),
            msg: 'This value should be a valid integer.'
        };
        return rt;
    },
    email: function (value) {
        var rt = {
            bool: /^((([a-zA-Z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-zA-Z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-zA-Z]|\d|-|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-zA-Z]|\d|-|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$/.test(value),
            msg: 'This value should be a valid email.'
        };
        return rt
    },
    username: function (value) {
        var rt = {
            bool: /^[a-zA-Z0-9]+$/.test(value),
            msg: 'Username is not valid'
        };
        return rt;
    },
}
function clearForm(myFormElement) {

    var elements = myFormElement.elements;
   
    if (parsley) {
        parsley.innerHTML = null;
    }
    myFormElement.reset();

    for (i = 0; i < elements.length; i++) {
        
        field_type = elements[i].type.toLowerCase();
        var checkRemove = false;
        switch (field_type) {

            case "text":
            case "password":
            case "textarea":
            case "hidden":
                checkRemove = true;
                elements[i].value = "";
                break;

            case "radio":
            case "checkbox":
                if (elements[i].checked) {
                    elements[i].checked = false;
                }
                break;

            case "select-one":
            case "select-multi":
                elements[i].selectedIndex = 0;
                break;

            default:
                break;
        }
        if (checkRemove) {
            elements[i].classList.remove('parsley-error');
            if (elements[i].closest('.form-group')) {
                var parsley = elements[i].closest('.form-group').querySelector('.parsley-required');
                if (parsley) {
                    parsley.innerHTML = null;
                }
            }
            
        }
    }
}
function ElementValidation(element, requirement) {
    var rt = {}, Brt = true;
    var parsley = element.closest('.form-group').querySelector('.parsley-required');
    parsley.innerHTML = null;
    element.classList.remove('parsley-error');
    for (var i = 0; i < requirement.length; i++) {
        if (Brt) {
            var $item = requirement[i];
            var rt = validation[$item.name](element.value, $item.requirement);
            if (rt.bool == false) {
                break;
            }
        }
    }
    if (rt.bool == false) {
        element.classList.add('parsley-error');
        parsley.innerHTML = rt.msg;
    }
    return rt;
}

function Alert($title, $callback) {
    var popdb = document.createElement('div');
    popdb.setAttribute('class', 'dv');
    var backdrop = document.createElement('div');
    backdrop.setAttribute('class', 'backdrop visible active');
    popdb.appendChild(backdrop);
    var pop = document.createElement('div');

    pop.setAttribute('class', 'popup-container popup-showing active');
    pop.innerHTML = '<div class="popup">\
        <div class="popup-head" >\
            <h3 class="popup-title ng-binding">Thông báo</h3><div class="popup-close"></div>\
            </div >\
        <div class="popup-body">\
            <span>'+ ($title) + '</span>\
        </div>\
        </div>';
    pop.querySelector('.popup-close').addEventListener('click', function () {
        popdb.remove();
        if ($callback) {
            $callback();
        }
    });
    popdb.appendChild(pop);
    document.body.appendChild(popdb);
}

function JqueryPostData(url, dataPOST, handleCallback) {
    $.ajax({
        type: "POST",
        url: url,
        data: dataPOST,
        content: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response, textStatus, xhr) {
            handleCallback(response);
        },
        error: function (xhr, textStatus, errorThrown) {
            console.log("error");
        }
    });
}