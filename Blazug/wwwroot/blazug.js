var _initialized = false;

var _maxLogs = 0;

var _consoleElement = null;
var _inspectorElement = null;

var _dotNetHelper = null;

var _cssScope = "b-undefined";

var _textControls = new Object();
var _buttonControls = new Object();
var _switchControls = new Object();
var _radioControls = new Object();


export const init = (maxLogs, dotNetHelper) => {

    _maxLogs = maxLogs;

    if (_initialized == true) {

        return;

    }

    _initialized = true;

    _dotNetHelper = dotNetHelper;

    _consoleElement = document.querySelector('.debug-console');

    _inspectorElement = document.querySelector('.debug-inspector');

    _cssScope = Array.from(_consoleElement.attributes).find(str => str.name.startsWith("b-")).name ?? "b-undefined";

    initConsoleHooks();
}


const initConsoleHooks = () => {

    window.onerror = function (error, url, line) {

        appendConsoleLog(_consoleElement, `${error} ${url} ${line}`, "exception");

        return false;
    }


    window.onunhandledrejection = function (e) {

        appendConsoleLog(_consoleElement, e.reason, "promise");

    }


    function hookLogType(logType) {

        const original = console[logType].bind(console)

        return function () {
                
            Array.from(arguments).forEach(arg => {

                appendConsoleLog(_consoleElement, arg, logType);

            });

            original.apply(console, arguments)
        }
    }

    ["log", "error", "warn", "debug"].forEach(logType => {

        console[logType] = hookLogType(logType);

    })

}


const appendConsoleLog = (_consoleElement, log, level) => {

    if (_initialized == false) {
        return;
    }

    let html = `<div loglevel="${level}">
                         <span></span>
                         <span>${log}</span>
                     </div>`;

    var logElement = createDomElementFromHtmlString(html);

    _consoleElement.appendChild(logElement);


    // cap max logs displayed.

    if (_consoleElement.childElementCount > _maxLogs) {

        _consoleElement.removeChild(_consoleElement.firstElementChild);

    }


    // update scroll if needed

    if (_consoleElement.offsetHeight < 1) {

        return;

    }

    var cs = getComputedStyle(_consoleElement);

    var consoleHeight = _consoleElement.offsetHeight - parseFloat(cs.borderTopWidth) + parseFloat(cs.borderBottomWidth);

    if (_consoleElement.scrollHeight > consoleHeight) {

        if (_consoleElement.scrollHeight - _consoleElement.scrollTop - consoleHeight - _consoleElement.lastChild.offsetHeight <= _consoleElement.lastChild.offsetHeight) {

            _consoleElement.scrollTo({
                top: _consoleElement.scrollHeight - consoleHeight,
                left: 0,
                behavior: 'smooth'
            });

        }

    }
}






export const displayText = (controlId, value, minsize) => {

    if (_initialized == false) {

        return;

    }

    if (!_textControls.hasOwnProperty(controlId)) {

        let html = `<div class="debug-item">
                        <div class="debug-controlId-tip">
                            <strong>
                            ${controlId}
                            </strong>
                        </div>
                        <div class="debug-control debug-control-padding">
                        </div>
                    </div>`;

        let textElement = createDomElementFromHtmlString(html);

        textElement.setAttribute("minsize", minsize);

        _inspectorElement.appendChild(textElement);

        _textControls[controlId] = textElement.querySelector(".debug-control");
    }

    let textElement = _textControls[controlId];

    textElement.innerText = value;

}

export const createButton = (controlId, buttonText, minsize) => {

    if (_initialized == false) {

        return;

    }

    if (!_buttonControls.hasOwnProperty(controlId)) {

        let html = `<div class="debug-item">
                        <div class="debug-controlId-tip">
                            <strong>
                            ${controlId}
                            </strong>
                        </div>
                        <div class="debug-control">
                            <button class="simple-button">
                            </button>
                        </div>
                    </div>`;

        let buttonElement = createDomElementFromHtmlString(html);

        buttonElement.setAttribute("minsize", minsize);

        _inspectorElement.appendChild(buttonElement);

        _buttonControls[controlId] = buttonElement.querySelector(".simple-button");

        _buttonControls[controlId].innerText = buttonText;

        _buttonControls[controlId].onclick = async () => await _dotNetHelper.invokeMethodAsync('ButtonClicked', controlId);

    } else {

        console.warn(`Blazug button ${controlId} already exists.`);

    }

}

export const createSwitch = (controlId, initialState, switchTextWhenOn, switchTextWhenOff, minsize) => {

    if (_initialized == false) {

        return;

    }

    if (!_switchControls.hasOwnProperty(controlId)) {

        let html = `<div class="debug-item">
                        <div class="debug-controlId-tip">
                            <strong>
                            ${controlId}
                            </strong>
                        </div>
                        <div class="debug-control  debug-control-padding" >
                            <label class="switch">
                                <input type="checkbox" >
                                <span class="slider">
                                </span>
                            </label>
                            <span class="switch-label">
                            </span>
                        </div>
                    </div>`;

        let switchElement = createDomElementFromHtmlString(html);

        switchElement.setAttribute("minsize", minsize);

        _inspectorElement.appendChild(switchElement);

        _switchControls[controlId] = switchElement;

        var label = _switchControls[controlId].querySelector(".debug-item>.debug-control>.switch-label");

        var input = _switchControls[controlId].querySelector(".debug-item>.debug-control>.switch>input");

        label.innerText = initialState ? switchTextWhenOn : switchTextWhenOff;

        input.checked = initialState;

        input.onchange = async () => {

            label.innerText = input.checked ? switchTextWhenOn : switchTextWhenOff;

            await _dotNetHelper.invokeMethodAsync('SwitchClicked', controlId, input.checked);

        };

    } else {

        console.warn(`Blazug switch ${controlId} already exists.`);

    }

}

export const createRadio = (controlId, initialState, buttonsText, minsize) => {

    if (_initialized == false) {

        return;

    }

    if (!_radioControls.hasOwnProperty(controlId)) {

        let buttonsHtml = "";

        for (var i = 0; i < buttonsText.length; i++) {

            let buttonText = buttonsText[i];

            let id = "id_radio_" + i + "_" + makeRandomId(8);

            let checked = initialState == i ? "checked" : "";

            buttonsHtml += `<input type="radio" name="mode" id="${id}" value="${id}" ${checked}>
                     <label for="${id}">${buttonText}</label>`;
        }

        let html = `<div class="debug-item">
                        <div class="debug-controlId-tip">
                            <strong>
                            ${controlId}
                            </strong>
                        </div>
                        <div class="debug-control" >
                            <span class="toggle-radio">
                                ${buttonsHtml}
                            </span>
                        </div>
                    </div>`;

        let radioElement = createDomElementFromHtmlString(html);

        radioElement.setAttribute("minsize", minsize);

        _inspectorElement.appendChild(radioElement);

        _radioControls[controlId] = radioElement;

        var inputs = radioElement.querySelectorAll(".debug-item>.debug-control>.toggle-radio>input");

        for (var i = 0; i < inputs.length; i++) {

            let input = inputs[i];

            input.setAttribute("data-index", i);

            input.onchange = async (e) => {

                await _dotNetHelper.invokeMethodAsync('RadioClicked', controlId, parseInt(e.srcElement.dataset.index));

            };

        }

    } else {

        console.warn(`Blazug radio buttons ${controlId} already exists.`);

    }

}

const makeRandomId = (length) => {
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() *
            charactersLength));
    }
    return result;
}


const createDomElementFromHtmlString = (html) => {

    var div = document.createElement('div');

    div.innerHTML = html;

    div.querySelectorAll("*").forEach(el => el.setAttribute(_cssScope, ""));

    return div.firstChild;
}
