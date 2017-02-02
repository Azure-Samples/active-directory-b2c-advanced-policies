/// <summary>
/// Provides extension methods for Base64 URL encoding strings
/// </summary>

function DecodeBytesFromUrlCompliant64(input) {
    var base64PadCharacter = '=';
    var doubleBase64PadCharacter = "==";
    var base64Character62 = '+';
    var base64Character63 = '/';
    var base64UrlCharacter62 = '-';
    var base64UrlCharacter63 = '_';

    input = input.replace(
        base64UrlCharacter62,
        base64Character62);
    input = input.replace(
        base64UrlCharacter63,
        base64Character63);

    switch (input.length % 4) {
        case 0:
            break;
        case 2:
            input += doubleBase64PadCharacter;
            break;
        case 3:
            input += base64PadCharacter;
            break;

        default:
            throw ("base64 decoding error");
    }

    return res = atob(input);
}

function renderOAuthMessage(input)
{
    var segments = input.split(".");
    var result = "decoding error";
    if (segments.length === 3)
    {
        result = G_Resources.JoseHeader + ":\r\n";
        result += renderJson(DecodeBytesFromUrlCompliant64(segments[0]), true) + "\r\n\r\n";

        result += G_Resources.ClaimsSet + ":\r\n";
        result += renderJson(DecodeBytesFromUrlCompliant64(segments[1]), true) + "\r\n\r\n";

        var additional = segments[2].split(";");
        result += G_Resources.Signature + ":\r\n";
        result += additional[0] + "\r\n\r\n";

        result += G_Resources.Miscellaneous + ":\r\n";
        for (var count = 1; count < additional.length; count++)
        {
            result += additional[count] + "\r\n";
        }
    }

    return result;
}

function renderJsonMessage(string)
{
	var eomOffset = string.lastIndexOf("};");
	if (eomOffset > 0)
	{
		string = string.substring(0, eomOffset + 1);
	}

	return renderJson(string, true);
}

function renderJson(string, showError)
{
	var output = "";
	try
	{
		var jsonobj = JSON.parse(string);
		output = JSON.stringify(jsonobj, null, 2);
	}
	catch (err)
	{
		if (showError)
		{
			output = "Defective JSON string - " + err + ":\r\n";				
		}
		output += string;
	}
	
	return output;
}
