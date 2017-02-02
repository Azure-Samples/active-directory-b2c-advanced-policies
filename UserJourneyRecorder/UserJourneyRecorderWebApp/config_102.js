    var Config_HasChanged = false;
    var G_StreamId = "";
    var G_Version = "102";

    // variables read from the language file
    var G_StateitemKeyNames;
    var G_HandlerFriendlyNames;
    var G_HandlerDescriptions;
    var G_ComplexStatebagItems;
    var G_Resources;
    var G_HeadersTitle;
    var G_OrchestrationPreconditionTypes;
    var G_OrchestrationPreconditionActionTypes;

    var Config_AutoRefresh = false;
    var Config_SummariesTrumpSuppressions = true;
    var Config_ShowUnsuppressedRequests = false;
    var Config_ShowTransitions = false;
    var Config_ShowFullStatebag = false;
    var Config_EnablePredicateSuppression = true;
    var Config_EnableActionSuppression = true;
    var Config_EnableSuppressionOfEmptyHandlerResults = true;
    var Config_ReportEnabledTechnicalProfiles = false;
    var Config_PlaySound = false;
    var Config_Sound = "segment";
    var Config_RecorderEndpoint = "https://b2crecorder.azurewebsites.net/stream";

    var Config_ConfigurationDefaults = {
        "Config_AutoRefresh": "false",
        "Config_SummariesTrumpSuppressions": "true",
        "Config_ShowUnsuppressedRequests": "false",
        "Config_ShowTransitions": "false",
        "Config_ShowFullStatebag": "false",
        "Config_EnablePredicateSuppression": "true",
        "Config_EnableActionSuppression": "true",
        "Config_EnableSuppressionOfEmptyHandlerResults": "true",
        "Config_ReportEnabledTechnicalProfiles": "false",
        "Config_PlaySound": "true",
        "Config_RecorderEndpoint": "https://b2crecorder.azurewebsites.net/stream"
    };

    var Config_ConfigurationControlTypes = {
        "Config_AutoRefresh": "checkbox",
        "Config_SummariesTrumpSuppressions": "checkbox",
        "Config_ShowUnsuppressedRequests": "checkbox",
        "Config_ShowTransitions": "checkbox",
        "Config_ShowFullStatebag": "checkbox",
        "Config_EnablePredicateSuppression": "checkbox",
        "Config_EnableActionSuppression": "checkbox",
        "Config_EnableSuppressionOfEmptyHandlerResults": "checkbox",
        "Config_ReportEnabledTechnicalProfiles": "checkbox",
        "Config_PlaySound": "checkbox",
        "Config_RecorderEndpoint": "textbox"
    };

    var Config_ConfigurationControlText = {
        "Config_AutoRefresh": "The reader should start in AutoRefresh mode",
        "Config_SummariesTrumpSuppressions": "Always show a result if it contains an engine explanation",
        "Config_ShowUnsuppressedRequests": "Always show Predicate or Action requests",
        "Config_EnablePredicateSuppression": "Don't show unnecessary Predicate requests",
        "Config_EnableActionSuppression": "Don't show unnecessary Action requests",
        "Config_EnableSuppressionOfEmptyHandlerResults": "Don't show empty handler results",
        "Config_ShowTransitions": "Show state transitions",
        "Config_ShowFullStatebag": "Aways show the full the statebag rather than just deltas",
        "Config_ReportEnabledTechnicalProfiles": "Report when check on enabled technical profiles succeeds",
        "Config_PlaySound": "Play a sound when a journey segment arrives using AutoRefresh",
        "Config_RecorderEndpoint": "Recorder URL"
};

    function Config_ReadConfig() {
        setStreamId();

        if (localStorage.Config_AutoRefresh !== undefined) {
            Config_AutoRefresh = localStorage.Config_AutoRefresh === "true";
        }

        if (localStorage.Config_SummariesTrumpSuppressions !== undefined) {
            Config_SummariesTrumpSuppressions = localStorage.Config_SummariesTrumpSuppressions === "true";
        }

        if (localStorage.Config_ShowUnsuppressedRequests !== undefined) {
            Config_ShowUnsuppressedRequests = localStorage.Config_ShowUnsuppressedRequests === "true";
        }

        if (localStorage.Config_ShowTransitions !== undefined) {
            Config_ShowTransitions = localStorage.Config_ShowTransitions === "true";
        }

        if (localStorage.Config_ShowFullStatebag !== undefined) {
            Config_ShowFullStatebag = localStorage.Config_ShowFullStatebag === "true";
        }

        if (localStorage.Config_EnablePredicateSuppression !== undefined) {
            Config_EnablePredicateSuppression = localStorage.Config_EnablePredicateSuppression === "true";
        }

        if (localStorage.Config_EnableActionSuppression !== undefined) {
            Config_EnableActionSuppression = localStorage.Config_EnableActionSuppression === "true";
        }

        if (localStorage.Config_EnableSuppressionOfEmptyHandlerResults !== undefined) {
            Config_EnableSuppressionOfEmptyHandlerResults = localStorage.Config_EnableSuppressionOfEmptyHandlerResults === "true";
        }

        if (localStorage.Config_ReportEnabledTechnicalProfiles !== undefined) {
            Config_ReportEnabledTechnicalProfiles = localStorage.Config_ReportEnabledTechnicalProfiles === "true";
        }

        if (localStorage.Config_PlaySound !== undefined) {
            Config_PlaySound = localStorage.Config_PlaySound === "true";
        }

        if (localStorage.Config_RecorderEndpoint !== undefined) {
            Config_RecorderEndpoint = localStorage.Config_RecorderEndpoint;
        }
    }

    function setStreamId() {
        var stream = getParameter("id");
        G_StreamId = validateStreamId(stream);
    }

    function validateStreamId(id) {
        id = id.replace(/[^a-zA-Z0-9\-\s]/, "");
        if (id.length !== 36) {
            return "INVALID_STREAM_ID";
        }

        return id;
    }

    function getStreamUrl() {
        return Config_RecorderEndpoint + "?id=" + G_StreamId;
    }

    function configControlClicked()
    {
        Config_HasChanged = true;
    }

    function Config_InitializeConfigUI()
    {
        setStreamId();

        var keys = Object.keys(Config_ConfigurationControlText);

        var text = "";
        for (var keyCount = 0; keyCount < keys.length; keyCount++) {
            var key = keys[keyCount];
            var buttonText = Config_ConfigurationControlText[key];
            var divId = "ConfigControl_" + keyCount.toString();
            switch (Config_ConfigurationControlTypes[key])
            {
                case "checkbox":
                    text += "<div id=\"" + divId + "\" class=\"CheckboxClass\" onclick=\"configControlClicked();\">\r\n<input type=\"checkbox\" id=\"" + key + "\" name=\"" + key + "\">" + buttonText + "</input>\r\n</div>\r\n";
                    break;
                case "textbox":
                    text += "<div id=\"" + divId + "\" class=\"TextboxClass\" onclick=\"configControlClicked();\">\r\n" + buttonText + "&nbsp;<input type=\"text\" id=\"" + key + "\" name=\"" + key + "\" size=\"64\"></input>\r\n</div>\r\n";
                    break;
                default:
                    break;
            }
        }

        var element = document.getElementById("configForm");
        element.innerHTML = text;

        readConfigIntoCheckboxes();
    }

    function readConfigIntoCheckboxes()
    {
        var keys = Object.keys(Config_ConfigurationDefaults);

        var saveIsNecessary = false;
        for (var keyCount=0; keyCount < keys.length; keyCount++)
        {
            var key = keys[keyCount];
            var currentValue = localStorage.getItem(key);
            if (!currentValue)
            {
                saveIsNecessary = true;
                currentValue = Config_ConfigurationDefaults[key];
            }

            var element = document.getElementById(key);
            switch (Config_ConfigurationControlTypes[key]) {
                case "checkbox":
                    element.checked = currentValue === "true";
                    break;
                case "textbox":
                    element.value = currentValue;
                    break;
                default:
                    break;
            }

        }

        if (saveIsNecessary)
        {
            Config_WriteConfig();
        }

    }

    function Config_WriteConfig()
    {
        var keys = Object.keys(Config_ConfigurationDefaults);

        for (var keyCount = 0; keyCount < keys.length; keyCount++) {
            var key = keys[keyCount];
            var element = document.getElementById(key);
            switch (Config_ConfigurationControlTypes[key]) {
                case "checkbox":
                    localStorage.setItem(key, element.checked.toString());
                    break;
                case "textbox":
                    localStorage.setItem(key, element.value);
                    break;
                default:
                    break;
            }
        }

        Config_HasChanged = false;
    }

    function Config_ResetConfig()
    {
        var keys = Object.keys(Config_ConfigurationDefaults);

        for (var keyCount = 0; keyCount < keys.length; keyCount++) {
            var key = keys[keyCount];
            localStorage.removeItem(key);
        }

        readConfigIntoCheckboxes();
    }

    function getParameter(val) {
        var result = "Not found",
        tmp = [];
        location.search
        .substr(1).split("&").forEach(function (item) {
            tmp = item.split("=");
            if (tmp[0] === val) result = decodeURIComponent(tmp[1]);
        });
        return result;
    }

    function Config_ReturnToReader() {
        if (Config_HasChanged)
        {
            if (confirm("Do you want to save your changes before returning to the Reader?"))
            {
                Config_WriteConfig();
            }
        }

        var stream = getParameter("stream");
        document.location = "trace_" + G_Version + ".html?stream=" + stream;
    }

    function CreateXMLDocFromString(str) {
        var xmlDoc = null;
        if (window.DOMParser) {
            var parser = new DOMParser();
            xmlDoc = parser.parseFromString(str, "text/xml");
        } else if (window.ActiveXObject) {
            xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
            xmlDoc.async = false;
            xmlDoc.loadXML(str);
        }

        return xmlDoc;
    }

    function CreateXmlDocFromUrl(url) {
        if (window.ActiveXObject) {
            xhttp = new ActiveXObject("Msxml2.XMLHTTP");
        }
        else {
            xhttp = new XMLHttpRequest();
        }
        xhttp.open("GET", url, false);
        try { xhttp.responseType = "msxml-document" } catch (err) { } // Helping IE11
        xhttp.send("");
        return xhttp.responseXML;
    }

    function renderXmlMessage(str) {
        var text = "";
        str = str.replace("&", "");
        xml = CreateXMLDocFromString(str);
        xsl = CreateXmlDocFromUrl("xmltoken.xsl");
        // code for IE
        if (window.ActiveXObject || xhttp.responseType == "msxml-document") {
            text = xml.transformNode(xsl);
        }
        // code for Chrome, Firefox, Opera, etc.
        else if (document.implementation && document.implementation.createDocument) {
            xsltProcessor = new XSLTProcessor();
            xsltProcessor.importStylesheet(xsl);
            text = xsltProcessor.transformToFragment(xml, document);
        }
    }

    function Config_ReadLanguage() {
        var url = "languages/en-us_" + G_Version + ".json";
        $.get(url, processLoadLanguage, "json");
    }

    function processLoadLanguage(languageElements, status, jqXHR) {
        var languageContent = eval(languageElements);

        G_StateitemKeyNames = languageContent.G_StateitemKeyNames;
        G_HandlerFriendlyNames = languageContent.G_HandlerFriendlyNames;
        G_HandlerDescriptions = languageContent.G_HandlerDescriptions;
        G_ComplexStatebagItems = languageContent.G_ComplexStatebagItems;
        G_Resources = languageContent.G_Resources;
        G_HeadersTitle = languageContent.G_HeadersTitle;
        G_OrchestrationPreconditionTypes = languageContent.G_OrchestrationPreconditionTypes;
        G_OrchestrationPreconditionActionTypes = languageContent.G_OrchestrationPreconditionActionTypes;

        // Due to a side-effect of the $get behavior this config needs to happen after 
        // values are loaded and this works
        InitializeRefreshMode();
    }



