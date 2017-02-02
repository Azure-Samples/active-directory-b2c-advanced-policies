# This PS1 script will create the required applications for B2C Advanced
# Supply the location of your base policy so that the script updates the necessary values.
Param(
    $BasePolicyPath = ""
)


$ErrorActionPreference = "Stop"

"Loading binaries"
$adal = "$PSScriptRoot\Microsoft.IdentityModel.Clients.ActiveDirectory.dll"
if (!(Test-Path $adal)) { "Could not find $adal. Please make sure you run this obtain the full ExploreAdmin folder and run the script from there."; return;}
[System.Reflection.Assembly]::LoadFile($adal);

$adalWin = "$PSScriptRoot\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll";
if (!(Test-Path $adal)) { "Could not find $adal. Please make sure you run this obtain the full ExploreAdmin folder and run the script from there."; return;}
[System.Reflection.Assembly]::LoadFile($adalWin);

$authority = "https://login.microsoftonline.com/common";
$resource = "https://graph.windows.net/"
$clientId = "1950a258-227b-4e31-a9cf-717495945fc2"
$redirectUri = "urn:ietf:wg:oauth:2.0:oob"

$promptBehavoir = [Microsoft.IdentityModel.Clients.ActiveDirectory.PromptBehavior]::Always;

$context = New-Object Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext($authority);
$token = $context.AcquireToken($resource, $clientId, $redirectUri, $promptBehavoir);

$expiryTime = ((Get-Date).ToString("yyyy-MM-dd"))

$tenantId = $token.TenantId;
$accessToken = $token.AccessToken;

"Getting Tenant Details"
$tenantDetailsResponse = Invoke-WebRequest "https://graph.windows.net/$tenantId/tenantDetails/?api-version=1.6" -Method Get -Headers @{Authorization="Bearer $accessToken"}
$tenantDetails = (ConvertFrom-Json $tenantDetailsResponse.Content).value
$tenantName = $tenantDetails.VerifiedDomains | Where Initial -eq "True" | Select -First 1 -ExpandProperty Name
"TenantName: $tenantName"

"Getting Azure AD Service Principal"
$aadSpResponse = Invoke-WebRequest "https://graph.windows.net/$tenantId/servicePrincipals?`$filter=appId+eq+'00000002-0000-0000-c000-000000000000'&api-version=1.6" -Method Get -Headers @{Authorization="Bearer $accessToken"}
$aadSp = (ConvertFrom-Json $aadSpResponse.Content).value[0]
$aadSpOid = $aadSp.objectId
$aadPermissionId = $aadSp.oauth2Permissions | where value -eq 'User.Read' | Select -First 1 -ExpandProperty Id


#----------- Web App ------------
"Registering Web App"
$webAppBody = "{`"appRoles`":[],`"availableToOtherTenants`":false,`"displayName`":`"PolicyEngine_Auto`",`"errorUrl`":null,`"groupMembershipClaims`":null,`"homepage`":`"https://login.microsoftonline.com/$tenantName`",`"identifierUris`":[`"https://$tenantName/$(New-Guid)`"],`"keyCredentials`":[],`"knownClientApplications`":[],`"logoutUrl`":null,`"oauth2AllowImplicitFlow`":false,`"oauth2AllowUrlPathMatching`":false,`"oauth2Permissions`":[],`"oauth2RequirePostResponse`":false,`"passwordCredentials`":[],`"publicClient`":false,`"recordConsentConditions`":null,`"replyUrls`":[`"https://login.microsoftonline.com/sacacorpb2c.onmicrosoft.com`"],`"requiredResourceAccess`":[{`"resourceAppId`":`"00000002-0000-0000-c000-000000000000`",`"resourceAccess`":[{`"id`":`"$aadPermissionId`",`"type`":`"Scope`"}]}],`"samlMetadataUrl`":null}"
$webAppResponse = Invoke-WebRequest "https://graph.windows.net/$tenantId/applications/?api-version=1.6" -Method Post -Headers @{Authorization="Bearer $accessToken";"Content-Type"="application/json"} -Body $webAppBody
$webApp = (ConvertFrom-Json $webAppResponse.Content)
$webAppId = $webApp.appId

"Creating Service Principal for Web App"
$webSpBody = "{`"appId`":`"$webAppId`" }"
$webSpResponse = Invoke-WebRequest "https://graph.windows.net/$tenantId/servicePrincipals/?api-version=1.6" -Method Post -Headers @{Authorization="Bearer $accessToken";"Content-Type"="application/json"} -Body $webSpBody
$webSp = (ConvertFrom-Json $webSpResponse.Content)
$webSpOid = $webSp.objectId
$webAppPermissionId = $webApp.oauth2Permissions[0].id


#----------- Native App ------------
"Registering Native App"
$nativeAppBody = "{`"appRoles`":[],`"availableToOtherTenants`":false,`"displayName`":`"PolicyEngineProxy_Auto`",`"errorUrl`":null,`"groupMembershipClaims`":null,`"homepage`":null,`"identifierUris`":[],`"keyCredentials`":[],`"knownClientApplications`":[],`"logoutUrl`":null,`"oauth2AllowImplicitFlow`":false,`"oauth2AllowUrlPathMatching`":false,`"oauth2Permissions`":[],`"oauth2RequirePostResponse`":false,`"passwordCredentials`":[],`"publicClient`":true,`"recordConsentConditions`":null,`"replyUrls`":[`"https://login.microsoftonline.com/sacacorpb2c.onmicrosoft.com`"],`"requiredResourceAccess`":[{`"resourceAppId`":`"$webAppId`",`"resourceAccess`":[{`"id`":`"$webAppPermissionId`",`"type`":`"Scope`"}]},{`"resourceAppId`":`"00000002-0000-0000-c000-000000000000`",`"resourceAccess`":[{`"id`":`"$aadPermissionId`",`"type`":`"Scope`"}]}],`"samlMetadataUrl`":null}"
$nativeAppResponse = Invoke-WebRequest "https://graph.windows.net/$tenantId/applications/?api-version=1.6" -Method Post -Headers @{Authorization="Bearer $accessToken";"Content-Type"="application/json"} -Body $nativeAppBody
$nativeApp = (ConvertFrom-Json $nativeAppResponse.Content)
$nativeAppId = $nativeApp.appId

"Creating Service Principal for Native App"
$nativeSpBody = "{`"appId`":`"$nativeAppId`" }"
$nativeSpResponse = Invoke-WebRequest "https://graph.windows.net/$tenantId/servicePrincipals/?api-version=1.6" -Method Post -Headers @{Authorization="Bearer $accessToken";"Content-Type"="application/json"} -Body $nativeSpBody
$nativeSp = (ConvertFrom-Json $nativeSpResponse.Content)
$nativeSpOid = $nativeSp.objectId


#----------- Consent (OAuth2PermissionGrants) -------------
"Consenting to Web App"
$webGrantBody = "{`"clientId`": `"$webSpOid`",`"consentType`": `"AllPrincipals`",`"resourceId`": `"$aadSpOid`",`"scope`": `"User.Read`", `"expiryTime`":`"$expiryTime`"}"
$webGrantResponse = Invoke-WebRequest "https://graph.windows.net/$tenantId/oauth2PermissionGrants/?api-version=1.6" -Method Post -Headers @{Authorization="Bearer $accessToken";"Content-Type"="application/json"} -Body $webGrantBody
$webGrant = (ConvertFrom-Json $webGrantResponse.Content)

"Consenting to Native App"
$nativeGrantBody = "{`"clientId`": `"$nativeSpOid`",`"consentType`": `"AllPrincipals`",`"resourceId`": `"$aadSpOid`",`"scope`": `"User.Read`", `"expiryTime`":`"$expiryTime`"}"
$nativeGrantResponse = Invoke-WebRequest "https://graph.windows.net/$tenantId/oauth2PermissionGrants/?api-version=1.6" -Method Post -Headers @{Authorization="Bearer $accessToken";"Content-Type"="application/json"} -Body $nativeGrantBody
$nativeGrant = (ConvertFrom-Json $nativeGrantResponse.Content)

"Conseting to Native App's use of Web App"
$nativeGrantWebBody = "{`"clientId`": `"$nativeSpOid`",`"consentType`": `"AllPrincipals`",`"resourceId`": `"$webSpOid`",`"scope`": `"user_impersonation`", `"expiryTime`":`"$expiryTime`"}"
$nativeGrantWebResponse = Invoke-WebRequest "https://graph.windows.net/$tenantId/oauth2PermissionGrants/?api-version=1.6" -Method Post -Headers @{Authorization="Bearer $accessToken";"Content-Type"="application/json"} -Body $nativeGrantWebBody
$nativeGrantWeb = (ConvertFrom-Json $nativeGrantWebResponse.Content)


#Final-Test
#$username = ""
#$password = ""
#$resourceOwnerBody = "grant_type=password&username=$username&password=$password&resource=$webAppId&client_id=$nativeAppID"
#Invoke-WebRequest "https://login.microsoftonline.com/$tenantName/oauth2/token" -Method Post -Body $resourceOwnerBody

"-------------------------"
"Web App ID: $webAppId"
"Native App ID: $nativeAppId"
"-------------------------"


if ($BasePolicyPath) {
    "Updating $BasePolicyPath"
    $newPolicy = @()
    Get-Content $basePolicyPath| foreach {
        $newPolicy += $_.Replace("(insert client_id of native client here)", $nativeAppId).Replace("(insert client_id of web app here)", $webAppId);
    }
    $outputPolicyPath = $basePolicyPath.Replace(".xml", "_updated.xml")
    $newPolicy > $outputPolicyPath
    "Updates saved to new file: $outputPolicyPath"
}
else {
    "Please proceed to update your Base policy"
}
