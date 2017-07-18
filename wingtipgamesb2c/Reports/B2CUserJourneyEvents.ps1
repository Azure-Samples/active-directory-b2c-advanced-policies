$loginHost = "https://login.microsoftonline.com"
$tenantId = "b2ctechready.onmicrosoft.com"
$clientId = "f67e0340-6713-4981-b605-37d52c3179c8"
$clientSecret = "Yz24RGZL+lBzyJvAOsy/bPVgH4rD7iMZVkPGQFPl7Bw="

# Acquire an access token for use with Graph API
$tokenRequestBody = @{ grant_type = "client_credentials" ; client_id = $clientID ; client_secret = $clientSecret }

$tokenResponse = Invoke-RestMethod -Method Post -Uri $loginHost/$tenantId/oauth2/token?api-version=1.0 -Body $tokenRequestBody

if ($tokenResponse.access_token -ne $null) {
    $graphRequestUrl = "https://graph.windows.net/$tenantId/reports/b2cUserJourneyEvents?api-version=beta"

    $authorizationRequestHeader = @{ 'Authorization' = "$($tokenResponse.token_type) $($tokenResponse.access_token)" }

    $graphResponseAsJson = (Invoke-WebRequest -UseBasicParsing -Uri $graphRequestUrl -Headers $authorizationRequestHeader)

    $graphResponseAsCsv = ($graphResponseAsJson.Content | ConvertFrom-Json).value | ConvertTo-Csv -NoTypeInformation

    Write-Output $graphResponseAsCsv

    $graphResponseAsCsv | Out-File -FilePath B2CUserJourneyEvents.csv -Encoding ASCII -Force
} else {
    Write-Host "ERROR: The access token couldn't be acquired."
    Write-Output $tokenResponse
}
