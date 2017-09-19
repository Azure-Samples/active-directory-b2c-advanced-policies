var Config_HasChanged = false;
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

var Config_SummariesTrumpSuppressions = true;
var Config_ShowUnsuppressedRequests = false;
var Config_ShowTransitions = false;
var Config_ShowFullStatebag = false;
var Config_EnablePredicateSuppression = true;
var Config_EnableActionSuppression = true;
var Config_EnableSuppressionOfEmptyHandlerResults = true;
var Config_ReportEnabledTechnicalProfiles = false;
var Config_RecorderEndpoint = "https://wingtipb2cplayer.azurewebsites.net/api/stream";
var Config_TimeRange = "PT1H";

var Config_ConfigurationDefaults = {
    "Config_SummariesTrumpSuppressions": "true",
    "Config_ShowUnsuppressedRequests": "false",
    "Config_ShowTransitions": "false",
    "Config_ShowFullStatebag": "false",
    "Config_EnablePredicateSuppression": "true",
    "Config_EnableActionSuppression": "true",
    "Config_EnableSuppressionOfEmptyHandlerResults": "true",
    "Config_ReportEnabledTechnicalProfiles": "false",
    "Config_RecorderEndpoint": "https://wingtipb2cplayer.azurewebsites.net/api/stream",
    "Config_RecorderTimeRange": "PT1H"
};

var Config_ConfigurationControlTypes = {
    "Config_SummariesTrumpSuppressions": "checkbox",
    "Config_ShowUnsuppressedRequests": "checkbox",
    "Config_ShowTransitions": "checkbox",
    "Config_ShowFullStatebag": "checkbox",
    "Config_EnablePredicateSuppression": "checkbox",
    "Config_EnableActionSuppression": "checkbox",
    "Config_EnableSuppressionOfEmptyHandlerResults": "checkbox",
    "Config_ReportEnabledTechnicalProfiles": "checkbox",
    "Config_RecorderEndpoint": "textbox",
    "Config_RecorderTimeRange": "select"
};

var Config_ConfigurationControlText = {
    "Config_SummariesTrumpSuppressions": "Always show a result if it contains an engine explanation",
    "Config_ShowUnsuppressedRequests": "Always show Predicate or Action requests",
    "Config_EnablePredicateSuppression": "Don't show unnecessary Predicate requests",
    "Config_EnableActionSuppression": "Don't show unnecessary Action requests",
    "Config_EnableSuppressionOfEmptyHandlerResults": "Don't show empty handler results",
    "Config_ShowTransitions": "Show state transitions",
    "Config_ShowFullStatebag": "Aways show the full the statebag rather than just deltas",
    "Config_ReportEnabledTechnicalProfiles": "Report when check on enabled technical profiles succeeds",
    "Config_RecorderEndpoint": "Recorder URL",
    "Config_RecorderTimeRange": "Recorder time range"
};

var Config_ConfigurationControlOptions = {
    "Config_RecorderTimeRange": [
        ["PT30M", "Last 30 minutes"],
        ["PT1H", "Last 1 hour"],
        ["PT4H", "Last 4 hours"],
        ["PT12H", "Last 12 hours"],
        ["PT1D", "Last 24 hours"]
    ]
};

function Config_ReadConfig() {
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

    if (localStorage.Config_RecorderEndpoint !== undefined) {
        Config_RecorderEndpoint = localStorage.Config_RecorderEndpoint;
    }

    if (localStorage.Config_RecorderTimeRange !== undefined) {
        Config_RecorderTimeRange = localStorage.Config_RecorderTimeRange;
    }
}

function validateStreamId(id) {
    id = id.replace(/[^a-zA-Z0-9\-\s]/, "");
    if (id.length !== 36) {
        return "INVALID_STREAM_ID";
    }

    return id;
}

function getStreamUrl() {
    return Config_RecorderEndpoint + "?timerange=" + Config_RecorderTimeRange;
}

function configControlClicked() {
    Config_HasChanged = true;
}

function Config_InitializeConfigUI() {
    var keys = Object.keys(Config_ConfigurationControlText);

    var text = "";
    for (var keyCount = 0; keyCount < keys.length; keyCount++) {
        var key = keys[keyCount];
        var buttonText = Config_ConfigurationControlText[key];
        var divId = "ConfigControl_" + keyCount.toString();
        switch (Config_ConfigurationControlTypes[key]) {
            case "checkbox":
                text += "<div id=\"" + divId + "\" class=\"CheckboxClass\" onclick=\"configControlClicked();\">\r\n<input type=\"checkbox\" id=\"" + key + "\" name=\"" + key + "\">" + buttonText + "</input>\r\n</div>\r\n";
                break;
            case "textbox":
                text += "<div id=\"" + divId + "\" class=\"TextboxClass\" onclick=\"configControlClicked();\">\r\n" + buttonText + "&nbsp;<input type=\"text\" id=\"" + key + "\" name=\"" + key + "\" size=\"64\"></input>\r\n</div>\r\n";
                break;
            case "select":
                text += "<div id=\"" + divId + "\" class=\"SelectClass\" onclick=\"configControlClicked();\">\r\n" + buttonText + "&nbsp;<select id=\"" + key + "\" name=\"" + key + "\">";
                var options = Config_ConfigurationControlOptions[key];

                for (var index = 0; index < options.length; index++) {
                    var option = options[index];
                    text += "<option value=\"" + option[0] + "\">" + option[1] + "</option>";
                }

                text += "</select>\r\n</div>\r\n";
                break;
            default:
                break;
        }
    }

    var element = document.getElementById("configForm");
    element.innerHTML = text;

    readConfigIntoCheckboxes();
}

function readConfigIntoCheckboxes() {
    var keys = Object.keys(Config_ConfigurationDefaults);

    var saveIsNecessary = false;
    for (var keyCount = 0; keyCount < keys.length; keyCount++) {
        var key = keys[keyCount];
        var currentValue = localStorage.getItem(key);
        if (!currentValue) {
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
            case "select":
                element.value = currentValue;
                break;
            default:
                break;
        }

    }

    if (saveIsNecessary) {
        Config_WriteConfig();
    }

}

function Config_WriteConfig() {
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
            case "select":
                localStorage.setItem(key, element.value);
                break;
            default:
                break;
        }
    }

    Config_HasChanged = false;
}

function Config_ResetConfig() {
    var keys = Object.keys(Config_ConfigurationDefaults);

    for (var keyCount = 0; keyCount < keys.length; keyCount++) {
        var key = keys[keyCount];
        localStorage.removeItem(key);
    }

    readConfigIntoCheckboxes();
}

function Config_ReturnToReader() {
    if (Config_HasChanged) {
        if (confirm("Do you want to save your changes before returning to the Reader?")) {
            Config_WriteConfig();
        }
    }

    document.location = "/";
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
}
