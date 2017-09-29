Scenario
========

This document will walk you through adding a SAML-based Relying party
to Azure AD B2C.


SAML - Relying Party support is available only a private preview feature.  Support is not available for the general public on this functionality as it has only been tested on some specific modalities. The team will ship this into public preview in the first few months of 2018.


This will enable scenarios such as:

-   Contoso has an app, *Contoso Rewards*, where their customers, who
    are consumers, keep track of their rewards points obtained by
    purchasing Contoso products in retail stores. Contoso also wants to
    allow these consumers to use their same Contoso account to access a
    third-party app, *3D Metrics*, to check out rewards points charts
    and metrics available only through that app.

    -   Use Azure AD B2C

    -   Local accounts

    -   SAML RP for 3D Metrics’ SSO

This walkthrough will only focus on the SAML RP piece of this scenario.

Walkthrough
===========

Setup
-----

Make sure you first complete the Basic Setup tutorial.

Create the SAML Token Issuer
----------------------------

Now that you’ve got working set of advanced policies, let’s go ahead and
add the capability for your tenant to issue SAML tokens.

> ***Note**: The Policy Reference section at the end of this doc
> contains more details around each of the XML elements referenced in
> this section.*

1.  Open the B2C\_1A\_base.xml policy from your working directory.

2.  Find the section with the &lt;ClaimsProviders&gt; and look for the
    &lt;ClaimsProvider&gt; with &lt;DisplayName&gt;Token
    Issuer&lt;/DisplayName&gt;

3.  Inside of that &lt;ClaimProvider&gt; add the following
    &lt;TechnicalProfile&gt; right after the one with Id=”JwtIssuer”:

```xml
<TechnicalProfile Id="Saml2AssertionIssuer">
  <DisplayName>Token Issuer</DisplayName>
  <Protocol Name="None"/>
  <OutputTokenFormat>SAML2</OutputTokenFormat>
  <Metadata>
    <Item Key="IssuerUri">https://login.microsoftonline.com/te/contoso.onmicrosoft.com/saml</Item>
  </Metadata>
  <CryptographicKeys>
    <Key Id="SamlAssertionSigning" StorageReferenceId="YourAppNameSamlCert" />
    <Key Id="SamlMessageSigning" StorageReferenceId="YourAppNameSamlCert "/>
  </CryptographicKeys>
  <InputClaims/>
  <OutputClaims/>
</TechnicalProfile>
```

4.  Configure Metadata

    1.  IssuerUri – Update *contoso.onmicrosoft.com* to your tenant

5.  Upload Certs - These are the certificates used to sign the SAML
    response.

    1.  (If you don’t have a cert already) Create the cert [using
        makecert](http://www.virtues.it/2015/08/howto-create-selfsigned-certificates-with-makecert/)

        1.  makecert -r -pe -n
            "CN=yourappname.yourtenant.onmicrosoft.com" -a sha256 -sky
            signature -len 2048 -e 12/21/2018 -sr CurrentUser -ss My
            YourAppNameSamlCert.cer

        2. Go to cert store “Manage User Certificates” &gt; Current
            User &gt; Personal &gt; Certificates &gt;
            yourappname.yourtenant.onmicrosoft.com

        3. Right click &gt; All Tasks &gt; Export

        4. Yes, export the private key

        5.  Defaults (PFX and first checkbox)

    2.  Open Powershell

    3.  Go to ExploreAdmin

    4.  Import-Module ExploreAdmin.dll (if it fails, it might be because
        the dll hasn’t been unblocked)

    5.  Run New-CpimCertificate -TenantId yourtenant.onmicrosoft.com
        -CertificateId YourAppNameSamlCert -CertificateFileName path
        -CertificatePassword password

        1.  When you run the command, make sure you sign in with the
            onmicrosoft.com account local to the tenant.

        2. It’ll ask you for MFA

6.  Save your changes and upload updated policy

    1.  This time, make sure you check the *Overwrite the policy if it
        exists* checkbox.

    2.  At this point, this will not have any effect, the intent of
        uploading is confirming that what you’ve added thus far doesn’t
        have any issues.

Add the SAML Relaying Party to User Journey(s)
----------------------------------------------

Now that your tenant can issue SAML tokens, we need to create a user
journey that will be the one issuing this SAML tokens.

1.  Open the B2C\_1A\_base.xml policy from your working directory.

2.  Find the section with the &lt;UserJourneys&gt; and duplicate the
    &lt;UserJourney&gt; with Id=”SignIn”

3.  Rename the Id of that new &lt;UserJourney&gt; (i.e SignInSaml)

4.  In the last &lt;OrchestrationStep&gt; (Type=”SendClaims”), modify
    the CpimIssuerTechnicalProfileReferenceId value from to
    Saml2AssertionIssuer

5.  Save your changes and upload the updated policy

6.  Copy the SignIn.xml file

7.  Rename it match the Id of the new journey you created (i.e.
    SignInSaml)

8.  Modify its PolicyId to a new Guid.

9.  Update the value of the ReferenceId attribute in the
    &lt;DefaultUserJourney&gt; to match the Id of of the new journey you
    created (i.e. SignInSaml)

10. Replace its &lt;RelayingParty&gt; element with the following:

```xml
<RelyingParty>
  <DefaultUserJourney ReferenceId="SignInSamlSF"/>
  <TechnicalProfile Id="PolicyProfile">
    <DisplayName>PolicyProfile</DisplayName>
    <Protocol Name="SAML2"/>
    <Metadata>
      <Item Key="PartnerEntity">
        <![CDATA[
<md:EntityDescriptor xmlns:md="urn:oasis:names:tc:SAML:2.0:metadata" entityID="https://contoso.3dmetrics.com" validUntil="2026-12-27T23:42:22.079Z" xmlns:ds="http://www.w3.org/2000/09/xmldsig#">
<md:SPSSODescriptor WantAssertionsSigned="true" protocolSupportEnumeration="urn:oasis:names:tc:SAML:2.0:protocol">
<md:NameIDFormat>urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified</md:NameIDFormat>
<md:AssertionConsumerService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST" Location="https://contoso.3dmetrics.com/samlp " index="0" isDefault="true"/> </md:SPSSODescriptor> </md:EntityDescriptor>
 ]]>
      </Item>
      <Item Key="Saml2AttributeEncodingInfo">
        <![CDATA[ 
<saml2:AttributeStatement xmlns:saml2="urn:oasis:names:tc:SAML:2.0:assertion"><saml2:Attribute FriendlyName="UserPrincipalName" Name="UserId"NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:uri"><saml2:AttributeValue xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:type="xs:string"></saml2:AttributeValue></saml2:Attribute></saml2:AttributeStatement> 
 ]]>
      </Item>
      <Item Key="Saml11AttributeEncodingInfo">
        <![CDATA[ 
<saml:AttributeStatement xmlns:saml="urn:oasis:names:tc:SAML:1.0:assertion"><saml:Attribute AttributeName="ImmutableID"AttributeNamespace="http://schemas.microsoft.com/LiveID/Federation/2008/05"><saml:AttributeValue></saml:AttributeValue></saml:Attribute><saml:Attribute AttributeName="UPN" AttributeNamespace="http://schemas.xmlsoap.org/claims"><saml:AttributeValue></saml:AttributeValue></saml:Attribute></saml:AttributeStatement> 
 ]]>
      </Item>
      <Item Key="client_id">customClientId</Item>
    </Metadata>
    <OutputClaims>
      <OutputClaim ClaimTypeReferenceId="displayName"/>
      <OutputClaim ClaimTypeReferenceId="givenName"/>
      <OutputClaim ClaimTypeReferenceId="surname"/>
      <OutputClaim ClaimTypeReferenceId="email"/>
      <OutputClaim ClaimTypeReferenceId="userPrincipalName"/>
      <OutputClaim ClaimTypeReferenceId="identityProvider"/>
    </OutputClaims>
    <SubjectNamingInfo ClaimType="userPrincipalName"/>
  </TechnicalProfile>
</RelyingParty>
```

11. Update the &lt;Item&gt; with Key=”PartnerEntity“ as follows:

    1.  Set the entityId attribute of &lt;md:EntityDescriptor&gt; to the
        SAML RP’s identifier / Entity Id

    2.  Set the Location attribute of
        &lt;md:AssertionConsumerService&gt; to the SAML RP’s Reply URL /
        Assertion Consumer Service (ACS) URL

    3.  Alternatively, you can replace the entire value of the
        &lt;Item&gt; element with the URL to the SAML RP’s metadata if
        such exists

        1.  e.g <Item Key=”PartnerEntity”\>https://app.com/metadata<Item\>;

12. Save your changes and upload this new policy.

Setup the SAML IdP in the App / SAML RP
---------------------------------------

You’ll need to setup B2C as a SAML IdP in the SAML RP / application.
Each application has different steps to do so, look at your app’s
documentation for guidance on how to do so. You will be required to
provide some or all the following data points:

-   **Metadata:**

    https://login.microsoftonline.com/te/<tenantName\>.onmicrosoft.com/b2c\_1a\<policyName\>/Samlp/metadata

-   **Issuer:**

    https://login.microsoftonline.com/te/<tenantName\>.onmicrosoft.com/B2C\_1A\<policyName\>

-   **Login URL / SAML Endpoint / SAML URL:**

    https://login.microsoftonline.com/te/<tenantName\>.onmicrosoft.com/B2C\_1A\<policyName\>/samlp/sso/login

-   **Certificate:**

    This is the YourAppNameSamlCert, but *without* the private key – Do
    not upload the cert you uploaded to B2C via Powershell. Instead, do
    the following:

1.  Go to the metadata URL specified above.

2.  Copy the value in the &lt;X509Certificate&gt; element

3.  Paste it into a text file

4.  Save the text file as a .cer file

 (Optional) Enable Debugging in your User Journey(s)
----------------------------------------------------

You can enable debugging tools to help you follow through the each of
the orchestration steps in the UserJourney and get details on issues
that occur. This should only be enabled during development.

1.  Open your new policy xml file (not the base.xml one)

2.  Add the following attributes to the &lt;TrustFrameworkPolicy&gt;
    element:

    1.  DeploymentMode=”Development”

    2.  UserJourneyRecorderEndpoint="https://b2crecorder.azurewebsites.net/stream?id=<guid\>"

        1.  Replace &lt;guid&gt; with an actual GUID

This will allow you to troubleshoot by going to
https://b2crecorder.azurewebsites.net/trace\_102.html?id=<guid\>

Policy Reference
================

SAML 2.0 Token Issuer
---------------------

A suitable technical profile definition for a SAML 2.0 token issuer
implies the followings.

The *Name* attribute of the *Protocol* XML element has to be set to
**None** as per *ProtocolName* enumeration in the Azure AD B2C Premium
XML schema - this should become default overtime but **None** will be
deprecated - while the *OutputTokenFormat* XML element has to be set to
**SAML2** (for SAML 2.0 assertions) as per *TokenFormat* enumeration in
the Azure AD B2C Premium XML schema.

The following keys must or may be present in the *Metadata* XML element:

|Key|Required|Description|
|---|--------|-----------|
|IssuerUri|False|Specify the issuer in the SAML assertion. If not specified, default to something rational.<br/>Type: String|
|TokenLifeTimeInSeconds|False|Specify the lifetime of the SAML assertion.<br/>Type: Integer|
|RequestOperation|False|Allow if specified to use a remote token issuer service in lieu of the one provided by the Azure AD B2C Premium service. See section § *Using a remote token issuer service*.<br/>Type: String<br/>Value: Must be set to **HttpRequest**.|
|client\_secret|False|Specify a cryptographic key to use if a remote token issuer service is being used. Define the key and algorithm.|

Similarly, the following keys must or may be present in the
*CryptographicKeys* XML element:

|Key|Required|Description|
|---|--------|-----------|
|SamlMessageSigning|True|Specify the X509 certificate (RSA key set) to use to sign SAML messages.|
|SamlAssertionSigning|True|Specify the X509 certificate (RSA key set) to use to sign SAML assertions.|
|SamlAssertionDecryption|True|Specify the X509 certificate (RSA key set) to use to decrypt SAML messages if you have to.|

> **Note** All the above keys may reference the same X509 certificate.

Both the *InputClaims* and *OutputClaims* XML elements are empty or
absent. Likewise, the *OutputClaimsTransformations* XML element is also
absent here.

The following XML snippet illustrates how to define a technical profile
for a SAML 2.0 token issuer:

```xml
<TechnicalProfile Id="Saml2AssertionIssuer">
  <DisplayName>Token Issuer</DisplayName>
  <Protocol Name="None" />
  <OutputTokenFormat>SAML2</OutputTokenFormat>
  <Metadata>
    <Item Key="IssuerUri">https://te.cpim.windows.net/csdii.onmicrosoft.com/B2C_1A_casinitiated</Item>
    <Item Key="TokenLifeTimeInSeconds">600</Item>
  </Metadata>
  <CryptographicKeys>
    <Key Id="SamlMessageSigning" StorageReferenceId="B2CSigningCertificate" />
    <Key Id="SamlAssertionSigning" StorageReferenceId="B2CSigningCertificate" />
    <Key Id="SamlAssertionDecryption" StorageReferenceId="B2CSigningCertificate" />
  </CryptographicKeys>
  <InputClaims />
  <OutputClaims />
</TechnicalProfile>
```

Relying Party
-------------

![](media/image7.png)

The relying party information chooses the user journey to enforce for
the current request. It also chooses the list of claims the relying
party application would like to get as part of the issued token.

Multiple applications can use a given policy. They will all receive the
same token with claims and the end user will go through the same user
journey.

Conversely, a single application can use multiple policies. This allows
the application to achieve functionality such as basic sign-in, step-up,
sign-up, etc.

To specify the relying party information, a *Relying Party* XML element
must be declared must be declared under the top-level XML element of the
policy XML file. This element is optional.

This element contains the following XML elements:

|XML element|Occurrences|Description|
|-----------|-----------|-----------|
|*DefaultUserJourney*|0:1|Define the default user journey for the relying party application.|
|*UserJourneyBehaviors*|0:1|Control the scope of various user journey behaviors.|
|*TechnicalProfile*|0:1|Define a technical profile supported by the relying party application. The technical profile provides in this context a contract for the relying party application to contact Azure AD B2C Premium.|

These above *DefaultUserJourney* and *TechnicalProfile* elements must be
declared for any given *RelyingParty* XML element.

The *DefaultUserJourney* XML element contains in turn the following
attribute:

|Attribute|Required|Description|
|---------|--------|-----------|
|*ReferenceId*|True|Specify a machine understandable identifier that is used to uniquely reference a particular user journey in the policy XML file.|

The *UserJourneyBehaviors* XML element contains the following XML
elements:

|XML element|Occurrences|Description|
|-----------|-----------|-----------|
|*SingleSignOn*|0:1|Control the scope of the single sign-on (SSO) behavior of a user journey.|
|*SessionExpiryType*|0:1|Control the whether the session is rolling or absolute.<br/>Value: one of the following value as per *SessionExpiryTypeTYPE* enumeration in the Azure AD B2C Premium XML schema for the names of the valid values the single sign-on session type:<br/>-   **Rolling**.<br/>-   **Absolute**.|
|*SessionExpiryInSeconds*|0:1|Control the time of the session expiry in seconds.<br/>Value: Integer|
|*AzureApplicationInsights*|0:1|Specify the Microsoft Azure Application Insights instrumentation key to be used in the application insights JavaScript.<br/>For additional information, see the [Visual Studio Application Insights documentation](https://azure.microsoft.com/en-us/documentation/services/application-insights/)|
|*ContentDefinitionParameters*|0:1|Specify the list of key value pairs to be appended to the content definition load Uri.|

The *SingleSignOn* XML element contains in turn the following attribute:

|Attribute|Required|Description|
|---------|--------|-----------|
|*Scope*|True|Define the scope of the single sign-on behavior.<br/>Value: one of the following value as per *UserJourneyBehaviorScopeType* enumeration in the Azure AD B2C Premium XML schema:<br/>-   **Suppressed**. Indicate that the behavior is suppressed. For example in the case of SSO, no session is maintained for the user and the user will always be prompted for identity provider selection.<br/>-   **TrustFramework**. Indicate that the behavior is applied for all policies in the trust framework. For example, a user being put through two policy journeys for a given trust framework will not be prompted for identity provider selection.<br/>-   **Tenant**. Indicates that the behavior is applied for all policies in the tenant. For example, a user being put through two policy journeys for a given tenant will not be prompted for identity provider selection.<br/>-   **Application**. Indicate that the behavior is applied for all policies for the application making the request. For example, a user being put through two policy journeys for a given application will not be prompted for identity provider selection.<br/>-   **Policy**. Indicate that the behavior only applies to a policy. For example, a user being put through two policy journeys for a given trust framework will be prompted for identity provider selection when switching between policies.|

The *AzureApplicationInsights* XML element contains in turn the
following attribute:

|Attribute|Required|Description|
|---------|--------|-----------|
|*InstrumentationKey*|True|Define the instrumentation key for the application insights element.<br/>Value: String|

The *ContentDefinitionParameters* XML element contains the following XML
elements:

|XML element|Occurrences|Description|
|-----------|-----------|-----------|
|*ContentDefinitionParameter*|0:n|Define a key value pair that is to be appended to the query string of content definition load Uri.<br/>Type: String|

The *ContentDefinitionParameter* XML element contains in turn the
following attribute:

|Attribute|Required|Description|
|---------|--------|-----------|
|*Name*|True|Specify the name of the key value pair.|

The *TechnicalProfile* XML element basically follows the structure
outlined before for a technical profile. Thus, it contains in turn the
following attributes:


|Attribute|Required|Description|
|---------|--------|-----------|
|*Id*|True|Specify a machine understandable identifier that is used to uniquely identify a particular technical profile in the policy XML file.|

And the following XML elements:

|XML element|Occurrences|Description|
|-----------|-----------|-----------|
|*DisplayName*|0:1|Specify the human understandable name of the technical profile that can be displayed to the users.<br/>Type: String|
|*Description*|0:1|Specify a human understandable description of the technical profile that can be displayed to the users.<br/>Type: String|
|*Protocol*|0:1|Specify the protocol used for the federation.|
|*Metadata*|1:1|Specify the metadata utilized by the protocol for communicating with the endpoint in the course of a transaction to plumb “on the wire” interoperability between the relying party and other community participants.<br/>Type: collection of *Item* of key/value pairs.|
|*OutputClaims*|0:1|Specify an optional list of the claim types that are taken as output in the technical profile. Each of these elements contains reference to a *ClaimType* already defined in the *ClaimsSchema* section or in a policy from which this policy XML file inherits.|
|*OutputTokenFormat*|0:1|Specify the format of the output token.<br/>Type: String (enumeration)<br/>Value: one of the following types as per *TokenFormat* enumeration in the Azure AD B2C Premium XML schema. See above.|
|*SubjectAuthenticationRequirements*|0:1|Specify the requirements regarding the conscious and active participation of the subject in authentication|
|*SubjectNamingInfo*|0:1|Control the production of the subject name in tokens (e.g. SAML) where subject name is specified separately from claims.|

The *Protocol* XML element in the above table contains the following
attributes:

|Attribute|Required|Description|
|---------|--------|-----------|
|*Name*|True|Specify the name of a valid protocol supported by Azure AD B2C Premium that is used as part of the technical profile.<br/>Type: String (enumeration)<br/>Value: one of the following types as per *ProtocolName* enumeration in the Azure AD B2C Premium XML schema:<br/>-   **OAuth1**. OAuth 1.0 protocol standard as per IETF specification.<br/>-   **OAuth2**. OAuth 2.0 protocol standard as per IETF specification.<br/>-   **SAML2**. SAML 2.0 protocol standard as per OASIS specification.<br/>-   **OpenIdConnect**. OpenID Connect 1.0 protocol standard as per OpenID foundation specification.<br/>-   **WsFed**. WS-Federation (WS-Fed) 1.2 protocol standard as per OASIS specification.<br/>-   **WsTrust**. WS-Trust 1.3 protocol standard as per OASIS specification.

As already introduced, the *OutputClaims* XML elements contain the
following XML elements:

|XML element|Occurrences|Description|
|-----------|-----------|-----------|
|*OutputClaim*|0:n|Specify the name of an expected claim type in the supported list for the policy to which the relying party subscribes. This claim serves as an output for the technical profile.|

Each *OutputClaim* XML element contains the following attributes:


|Attribute|Required|Description|
|---------|--------|-----------|
|*ClaimTypeReferenceId*|True|Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.<br/>Type: String|
|*DefaultValue *|False|Specify a default value if not set.<br/>Type: String|
|*PartnerClaimType*|False|Specify the partner claim type.<br/>Type: String|
|*Required*|False|Specify this claim is required.<br/>Type: String|

The *SubjectAuthenticationRequirements* XML element in the above table
contains the following attributes:

|Attribute|Required|Description|
|---------|--------|-----------|
|*TimeToLive*|True|Specify the maximum number of minutes cached credentials can be used following an active authentication by the subject.<br/>Type: Integer|
|*ResetExpiryWhenTokenIssued*|Optional|Specify how the expiry time is set.<br/>Type: Boolean. Default is False. If True then whenever a token is issued (even using a cached credential) the expiry time is set to the current time plus the *TimeToLive*.|

The *SubjectNamingInfo* XML element in the above table contains the
following attributes:


|Attribute|Required|Description|
|---------|--------|-----------|
|*ClaimType*|True|Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.<br/>Type: String|
|*NameQualifier*|False|Provide a description<br/>Type: String|
|*SPNameQualifier*|False|Provide a description<br/>Type: String|
|*Format*|False|Provide a description<br/>Type: String|
|*SPProvidedID*|False|Provide a description<br/>Type: String|

Considering the above explanation, the following XML snippet illustrates
how to define a relying party:

```xml
<?xml version="1.0" encoding="utf-8"?>
<TrustFrameworkPolicy xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"  xmlns:xsd="http://www.w3.org/2001/XMLSchema"  xmlns="http://schemas.microsoft.com/online/cpim/schemas/2013/06"  PublicPolicyUri="http://example.com"  PolicySchemaVersion="0.3.0.0"  TenantId="contoso369b2c.onmicrosoft.com"  PolicyId="B2C\_1A\_MsolActive">
  <BasePolicy>
    <TenantId>contoso369b2c.onmicrosoft.com</TenantId>
    <PolicyId>B2C\_1A\_base-v2</PolicyId>
  </BasePolicy>
  <RelyingParty>
    <DefaultUserJourney ReferenceId="ActiveRST"/>
    <TechnicalProfile Id="PolicyProfile">
      <DisplayName>WsFedProfile</DisplayName>
      <Protocol Name="WsFed" />
      <OutputTokenFormat>SAML11</OutputTokenFormat>
      <SubjectAuthenticationRequirements TimeToLive="4" ResetExpiryWhenTokenIssued="false" />
      <Metadata>
        <Item Key="Saml2AttributeEncodingInfo">
          <!\[CDATA\[            <saml2:AttributeStatement xmlns:saml2="urn:oasis:names:tc:SAML:2.0:assertion">
            <saml2:Attribute FriendlyName="UserPrincipalName"  Name="IDPEmail"  NameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:uri">
              <saml2:AttributeValue xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:type="xs:string">                </saml2:AttributeValue>
            </saml2:Attribute>
          </saml2:AttributeStatement>  \]\]>
        </Item>
        <Item Key="Saml11AttributeEncodingInfo">
          <!\[CDATA\[            <saml:AttributeStatement xmlns:saml="urn:oasis:names:tc:SAML:1.0:assertion">
            <saml:Attribute AttributeName="ImmutableID"  AttributeNamespace="http://schemas.microsoft.com/LiveID/Federation/2008/05">
              <saml:AttributeValue/>
            </saml:Attribute>
            <saml:Attribute AttributeName="UPN" AttributeNamespace="http://schemas.xmlsoap.org/claims">
              <saml:AttributeValue/>
            </saml:Attribute>
          </saml:AttributeStatement>  \]\]>
        </Item>
        <Item Key="PartnerEntity">https://www.contoso369b2c.com/wp-content/uploads/2015/01/metadata.xml</Item>
        <Item Key="client\_id">customClientId</Item>
      </Metadata>
      <OutputClaims>
        <OutputClaim ClaimTypeReferenceId="immutableId" PartnerClaimType="ImmutableID" />
        <OutputClaim ClaimTypeReferenceId="userPrincipalName" PartnerClaimType="UPN" />
        <OutputClaim ClaimTypeReferenceId="AuthenticationContext" DefaultValue="urn:federation:authentication:windows" />
      </OutputClaims>
      <SubjectNamingInfo ClaimType="immutableId" />
    </TechnicalProfile>
  </RelyingParty>
</TrustFrameworkPolicy>
```
