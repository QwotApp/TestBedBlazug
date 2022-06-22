/*@license  blazug*/


(function (blazug) {

    var _initialized = false;

    var _maxLogs = 500;

    var _blazugElement = null;
    var _consoleElement = null;
    var _inspectorElement = null;

    var _dotNetHelper = null;

    //    var _cssScope = "b-undefined";

    var _textControls = new Object();
    var _buttonControls = new Object();
    var _switchControls = new Object();
    var _radioControls = new Object();

    var _logs = [];

    const init = () => {

        if (_initialized == true) {

            return;

        }

        _initialized = true;

        initBlazugUI();

       // _cssScope = Array.from(_consoleElement.attributes).find(str => str.name.startsWith("b-")).name ?? "b-undefined";

        initConsoleHooks();
    }

    const initDotnet = (dotNetHelper) => {

        if (_initialized == false) {

            return;

        }

        _dotNetHelper = dotNetHelper;
    }

    const show = (visible) => {

        if (_initialized == false) {

            return;

        }

        if (visible) {

            _blazugElement.removeAttribute("hidden");

        }
        else {

            _blazugElement.setAttribute("hidden", "");

        }

    }


    const setMaxLogs = (maxLogs) => {

        _maxLogs = maxLogs;


        let overflow = _logs.length - _maxLogs;

        if (overflow > 0) {

            for (var i = 0; i < overflow; i++) {

                _logs.shift();

            }
        }

        overflow = _consoleElement.childElementCount - _maxLogs;

        if (overflow > 0) {

            for (var i = 0; i < overflow; i++) {

                _consoleElement.removeChild(_consoleElement.firstElementChild);

            }

            scrollToBottomOfConsoleLogs();

        }

    }

    const getLogs = () => {

        let strlogs = "";

        for (var i = 0; i < _logs.length; i++) {

            strlogs += _logs[i].replace(/[\r\n\x0B\x0C\u0085\u2028\u2029]+/g, " | ") + "\r\n";
        }

        /////

        //const blob = new Blob(strlogs);
        //const url = URL.createObjectURL(blob);

        //const anchorElement = document.createElement("a");
        //anchorElement.href = url;
        //anchorElement.download = "blazug.log";
        //anchorElement.click();
        //anchorElement.remove();

        //URL.revokeObjectURL(url);

        ////

        return strlogs;
    }

    const downloadFileFromStream = async (fileName, contentStreamReference) => {

        const arrayBuffer = await contentStreamReference.arrayBuffer();

        const blob = new Blob([arrayBuffer]);

        const url = URL.createObjectURL(blob);

        triggerFileDownload(fileName, url);

        URL.revokeObjectURL(url);
    }

    const triggerFileDownload = (fileName, url) => {

        const anchorElement = document.createElement('a');

        anchorElement.href = url;

        anchorElement.download = fileName ?? '';

        anchorElement.click();

        anchorElement.remove();
    }


    const displayText = (controlId, value, minsize) => {

        if (_initialized == false) {

            return;

        }

        if (!_textControls.hasOwnProperty(controlId)) {

            let html = `<div class="blazug-control"><div class="blazug-control-id"><strong>${controlId}</strong></div><div class="blazug-control blazug-control-padding"></div></div>`;

            let textElement = createDomElementFromHtmlString(html);

            textElement.setAttribute("minsize", minsize);

            _inspectorElement.appendChild(textElement);

            _textControls[controlId] = textElement.querySelector(".blazug-control");
        }

        let textElement = _textControls[controlId];

        textElement.innerText = value;

        // flash

        textElement.classList.remove("flash-on-change");

        setTimeout(() => textElement.classList.add("flash-on-change"), 100);

        
    }

    const createButton = (controlId, buttonText, minsize) => {

        if (_initialized == false) {

            return;

        }

        if (!_buttonControls.hasOwnProperty(controlId)) {

            let html = `<div class="blazug-control"><div class="blazug-control-id"><strong>${controlId}</strong></div><div class="blazug-control"><button class="blazog-button"></button></div></div>`;

            let buttonElement = createDomElementFromHtmlString(html);

            buttonElement.setAttribute("minsize", minsize);

            _inspectorElement.appendChild(buttonElement);

            _buttonControls[controlId] = buttonElement.querySelector(".blazog-button");

            _buttonControls[controlId].innerText = buttonText;

            _buttonControls[controlId].onclick = async () => await _dotNetHelper.invokeMethodAsync('ButtonClicked', controlId);

        } else {

            console.warn(`Blazug button ${controlId} already exists.`);

        }

    }

    const createSwitch = (controlId, initialState, switchTextWhenOn, switchTextWhenOff, minsize) => {

        if (_initialized == false) {

            return;

        }

        if (!_switchControls.hasOwnProperty(controlId)) {

            let html = `<div class="blazug-control"><div class="blazug-control-id"><strong>${controlId}</strong></div><div class="blazug-control blazug-control-padding"><label class="blazug-switch"><input type="checkbox"><span class="blazug-slider"></span></label><span class="blazug-switch-label"></span></div></div>`;

            let switchElement = createDomElementFromHtmlString(html);

            switchElement.setAttribute("minsize", minsize);

            _inspectorElement.appendChild(switchElement);

            _switchControls[controlId] = switchElement;

            let label = _switchControls[controlId].querySelector(".blazug-control>.blazug-control>.blazug-switch-label");

            let input = _switchControls[controlId].querySelector(".blazug-control>.blazug-control>.blazug-switch>input");

            label.innerText = initialState ? switchTextWhenOn : switchTextWhenOff;

            input.checked = initialState;

            input.onchange = async () => {

                label.innerText = input.checked ? switchTextWhenOn : switchTextWhenOff;

                await _dotNetHelper.invokeMethodAsync('SwitchClicked', controlId, input.checked);

            };

            label.onclick = () => input.click();

        } else {

            console.warn(`Blazug switch ${controlId} already exists.`);

        }

    }

    const createRadio = (controlId, initialState, buttonsText, minsize) => {

        if (_initialized == false) {

            return;

        }

        if (!_radioControls.hasOwnProperty(controlId)) {

            let buttonsHtml = "";

            for (var i = 0; i < buttonsText.length; i++) {

                let buttonText = buttonsText[i];

                let id = "id_radio_" + i + "_" + makeRandomId(8);

                let checked = initialState == i ? "checked" : "";

                buttonsHtml += `<input type="radio" name="mode" id="${id}" value="${id}" ${checked}><label for="${id}">${buttonText}</label>`;
            }

            let html = `<div class="blazug-control"><div class="blazug-control-id"><strong>${controlId}</strong></div><div class="blazug-control"><span class="blazug-radio">${buttonsHtml}</span></div></div>`;

            let radioElement = createDomElementFromHtmlString(html);

            radioElement.setAttribute("minsize", minsize);

            _inspectorElement.appendChild(radioElement);

            _radioControls[controlId] = radioElement;

            let inputs = radioElement.querySelectorAll(".blazug-control>.blazug-control>.blazug-radio>input");

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

    const initBlazugUI = (controlId, value, minsize) => {

        if (_initialized == false) {

            return;

        } 

        let html = `<div class="blazug"><div><button><svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512"><!--! Font Awesome Free 6.1.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free (Icons: CC BY 4.0, Fonts: SIL OFL 1.1, Code: MIT License) Copyright 2022 Fonticons, Inc. --><path d="M352 96V99.56C352 115.3 339.3 128 323.6 128H188.4C172.7 128 159.1 115.3 159.1 99.56V96C159.1 42.98 202.1 0 255.1 0C309 0 352 42.98 352 96zM41.37 105.4C53.87 92.88 74.13 92.88 86.63 105.4L150.6 169.4C151.3 170 151.9 170.7 152.5 171.4C166.8 164.1 182.9 160 199.1 160H312C329.1 160 345.2 164.1 359.5 171.4C360.1 170.7 360.7 170 361.4 169.4L425.4 105.4C437.9 92.88 458.1 92.88 470.6 105.4C483.1 117.9 483.1 138.1 470.6 150.6L406.6 214.6C405.1 215.3 405.3 215.9 404.6 216.5C410.7 228.5 414.6 241.9 415.7 256H480C497.7 256 512 270.3 512 288C512 305.7 497.7 320 480 320H416C416 344.6 410.5 367.8 400.6 388.6C402.7 389.9 404.8 391.5 406.6 393.4L470.6 457.4C483.1 469.9 483.1 490.1 470.6 502.6C458.1 515.1 437.9 515.1 425.4 502.6L362.3 439.6C337.8 461.4 306.5 475.8 272 479.2V240C272 231.2 264.8 224 255.1 224C247.2 224 239.1 231.2 239.1 240V479.2C205.5 475.8 174.2 461.4 149.7 439.6L86.63 502.6C74.13 515.1 53.87 515.1 41.37 502.6C28.88 490.1 28.88 469.9 41.37 457.4L105.4 393.4C107.2 391.5 109.3 389.9 111.4 388.6C101.5 367.8 96 344.6 96 320H32C14.33 320 0 305.7 0 288C0 270.3 14.33 256 32 256H96.3C97.38 241.9 101.3 228.5 107.4 216.5C106.7 215.9 106 215.3 105.4 214.6L41.37 150.6C28.88 138.1 28.88 117.9 41.37 105.4H41.37z"/></svg></button></div><div class="blazug-inspector" hidden></div><div class="blazug-console" hidden></div></div>`;

        _blazugElement = createDomElementFromHtmlString(html);

        _consoleElement = _blazugElement.querySelector('.blazug-console');

        _inspectorElement = _blazugElement.querySelector('.blazug-inspector');

        let blazugToggleButton = _blazugElement.querySelector('.blazug > div:first-child > button');

        blazugToggleButton.onclick = async () => {

            let consoleVisible = _consoleElement.getAttribute("hidden") == null;
            let inspectorVisible = _inspectorElement.getAttribute("hidden") == null;

            let inspectorHasChildren = _inspectorElement.childElementCount != 0;

            if (consoleVisible == true) {

                _inspectorElement.setAttribute("hidden", "");

                _consoleElement.setAttribute("hidden", "");
            }
            else if (inspectorHasChildren == false && consoleVisible == false ) {

                _inspectorElement.setAttribute("hidden", "");

                _consoleElement.removeAttribute("hidden");

                scrollToBottomOfConsoleLogs();
            } 
            else if (inspectorHasChildren == true && inspectorVisible == false && consoleVisible == false) {

                _inspectorElement.querySelectorAll(".flash-on-change").forEach(el => el.classList.remove("flash-on-change"));

                _inspectorElement.removeAttribute("hidden");

                _consoleElement.setAttribute("hidden", "");
            }
            else if (inspectorHasChildren == true && inspectorVisible == true) {

                _inspectorElement.setAttribute("hidden", "");

                _consoleElement.removeAttribute("hidden");

                scrollToBottomOfConsoleLogs();
            }

        };

        let attribut = document.currentScript.getAttribute("hiddenAtStartup")

        let hiddenAtStartup = attribut == "" || (attribut != null ? attribut.toLowerCase() == "true" : false);

        if (hiddenAtStartup) {
            _blazugElement.setAttribute("hidden", "");
        }

        document.body.appendChild(_blazugElement);
    }


    const initConsoleHooks = () => {

        window.onerror = function (error, url, line) {

            appendConsoleLog(`${error} ${url} ${line}`, "exception");

            return false;
        }


        window.onunhandledrejection = function (e) {

            appendConsoleLog(e.reason, "promise");

        }


        function hookLogType(logType) {

            const original = console[logType].bind(console)

            return function () {

                Array.from(arguments).forEach(arg => {

                    appendConsoleLog(arg, logType);

                });

                original.apply(console, arguments)
            }
        }

        ["info", "log", "error", "warn", "debug"].forEach(logType => {

            console[logType] = hookLogType(logType);

        })

    }

    const appendConsoleLog = (log, level) => {

        if (_initialized == false) {
            return;
        }

        _logs.push(`${level.padStart(9, ' ')}: ${log}`);

        if (_logs.length > _maxLogs) {

            _logs.shift();

        }


        let multiLineLog = log.replace(/[\r\n\x0B\x0C\u0085\u2028\u2029]+/g, "<br>");


        let html = `<div loglevel="${level}"><span></span><span>${multiLineLog}</span></div>`;

        let logElement = createDomElementFromHtmlString(html);

        _consoleElement.appendChild(logElement);


        // cap max logs displayed.

        if (_consoleElement.childElementCount > _maxLogs) {

            _consoleElement.removeChild(_consoleElement.firstElementChild);

        }


        // update scroll if needed

        if (_consoleElement.offsetHeight < 1) {

            return;

        }

        // scroll to last log if we previous one was at the bottom of the console.

        var cs = getComputedStyle(_consoleElement);

        let consoleHeight = _consoleElement.offsetHeight - parseFloat(cs.borderTopWidth) + parseFloat(cs.borderBottomWidth);

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

    const scrollToBottomOfConsoleLogs = () => {

        var cs = getComputedStyle(_consoleElement);

        let consoleHeight = _consoleElement.offsetHeight - parseFloat(cs.borderTopWidth) + parseFloat(cs.borderBottomWidth);

        _consoleElement.scrollTo({
            top: _consoleElement.scrollHeight - consoleHeight,
            left: 0,
            behavior: 'auto'
        });

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

        let div = document.createElement('div');

        div.innerHTML = html;

        //div.querySelectorAll("*").forEach(el => el.setAttribute(_cssScope, ""));

        return div.firstChild;
    }

    blazug.initDotnet = initDotnet;
    blazug.show = show;
    blazug.setMaxLogs = setMaxLogs;
    blazug.getLogs = getLogs;
    blazug.downloadFileFromStream = downloadFileFromStream;
    blazug.displayText = displayText;
    blazug.createButton = createButton;
    blazug.createSwitch = createSwitch;
    blazug.createRadio = createRadio;

    init();

}(window.blazug = window.blazug || {}));

