Scenario
========

This document will walk you through adding an Open ID Connect (OIDC)
based Identity provider to Azure AD B2C.

This will enable scenarios such as:

-   Contoso wants to build an app, *Contoso Rewards*, for their
    customers, who are consumers, to keep track of their rewards points
    obtained by purchasing Contoso products in retail stores. Contoso
    also wants to allow Contoso employees to sign in to the same app
    using their corporate credentials to create and manage activities
    that customers can participate in to get more rewards.

    -   Use Azure AD B2C

    -   Local accounts + social IdPs for consumers

    -   OIDC IdP for Contoso’s OIDC IdP

This walkthrough will only focus on the OIDC IdP piece of this scenario.

Walkthrough
===========

Setup
-----

Make sure you first complete the Basic Setup tutorial.

Register the App in the OIDC IdP
--------------------------------

You’ll need to register an application for B2C in the OIDC IdP. Each IdP
has different steps to do so, look at your IdPs documentation for
guidance on how to do so. You will be required to provide the following
data points:

-   **Reply URL / Redirect URI: **

    https://login.microsoftonline.com/te/<tenantName\>.onmicrosoft.com/oauth2/authresp

Create the OIDC Claims Provider
-------------------------------

Now that you’ve got working set of advanced policies, let’s go ahead and
add the OIDC IdP.

> ***Note**: The Policy Reference section at the end of this doc
> contains more details around each of the XML elements referenced in
> this section.*

1.  Open the B2C\_1A\_base.xml policy from your working directory.

2.  Find the section with the &lt;ClaimsProviders&gt; and add a new
   &lt;ClaimsProvider&gt; as follows:

   ```xml
<ClaimsProvider>
	<Domain>contoso</Domain>
	<DisplayName>Contoso</DisplayName>
	<TechnicalProfiles>
		<TechnicalProfile Id="Contoso">
			<DisplayName>Contoso</DisplayName>
			<Description>Login with your Contoso account</Description>
			<Protocol Name="OpenIdConnect"/>
			<OutputTokenFormat>JWT</OutputTokenFormat>
			<Metadata>
				<Item Key="METADATA">http://www.contoso.com/identity/.well-known/openid-configuration</Item>
				<Item Key="ProviderName">https://www.contoso.com/identity</Item>
				<Item Key="client_id">25d20e93-4cd7-43ee-a24f-03c05141639f</Item>
				<Item Key="BearerTokenTransmissionMethod">AuthorizationHeader</Item>
				<Item Key="scope">openid profile</Item>
				<Item Key="HttpBinding">POST</Item>
				<Item Key="response_types">code</Item>
				<Item Key="IdTokenAudience">25d20e93-4cd7-43ee-a24f-03c05141639f
				</Item>
			</Metadata>
			<CryptographicKeys>
				<Key Id="client_secret" StorageReferenceId="ContosoIdpOidcSecret"/>
			</CryptographicKeys>
			<OutputClaims>
				<OutputClaim ClaimTypeReferenceId="userId" PartnerClaimType="userId"/>
				<OutputClaim ClaimTypeReferenceId="identityProvider" DefaultValue="OIDCIdp" />
				<OutputClaim ClaimTypeReferenceId="givenName" PartnerClaimType="given_name"/>
				<OutputClaim ClaimTypeReferenceId="surname" PartnerClaimType="family_name"/>
				<OutputClaim ClaimTypeReferenceId="email" PartnerClaimType="email"/>
				<OutputClaim ClaimTypeReferenceId="displayName" PartnerClaimType="name"/>
				<OutputClaim ClaimTypeReferenceId="authenticationSource" DefaultValue="externalIdp"/>
			</OutputClaims>
			<OutputClaimsTransformations>
				<OutputClaimsTransformation ReferenceId="CreateRandomUPNUserName"/>
				<OutputClaimsTransformation ReferenceId="CreateUserPrincipalName"/>
				<OutputClaimsTransformation ReferenceId="CreateAlternativeSecurityId"/>
				<OutputClaimsTransformation ReferenceId="CreateSubjectClaimFromAlternativeSecurityId"/>
			</OutputClaimsTransformations>
			<UseTechnicalProfileForSessionManagement ReferenceId="SM-Noop"/>
		</TechnicalProfile>
	</TechnicalProfiles>
</ClaimsProvider>
   ```

3.  Configure basic settings

    1.  ClaimsProvider/Domain - Drives the value that can be passed in
        to
        [domain\_hint](http://www.cloudidentity.com/blog/2014/11/17/skipping-the-home-realm-discovery-page-in-azure-ad/)
        query string parameter, so make it url friendly, i.e. lowercase
        only, no spaces, only alphanumerical.

    2.  ClaimsProvider/DisplayName – Currently not displayed anywhere

    3.  TechnicalProfile@Id – Unique identifier for the technical
        profile, it is referenced elsewhere in the policy (see “Add the
        SAML Claims Provider to the User Journey”).

    4.  TechnicalProfile/DisplayName - Displayed as the button's caption
        in the sign in screen.

        ![](media/idpoidc01.png)

    5.  TechnicalProfile/Description – Currently not displayed anywhere

4.  Configure Metadata section

    1.  METADATA

    2.  ProviderName

    3.  client\_id

    4.  IdTokenAudience

    5.  scope

        1. HttpBinding

        2. response\_types

5.  Upload Client Secret - These are the certificates used to sign the
    SAML request and message. Even though we’ve configured the Claims
    Provider to not sign these, a certificate must still be provided.

    1.  Open Powershell

    2.  Go to ExploreAdmin

    3.  Import-Module ExploreAdmin.dll (if it fails, it might be because
        the dll hasn’t been unblocked)

    4.  Run Set-CpimKeyContainer -Tenant &lt;tenant&gt;
        -StorageReferenceId ContosoIdpOidcSecret -UnencodedAsciiKey
        &lt;client\_secret&gt;

        1.  When you run the command, make sure you sign in with the
            onmicrosoft.com account local to the tenant.

        2. It’ll ask you for MFA

6.  Save your changes and upload updated policy

    1.  This time, make sure you check the *Overwrite the policy if it
        exists* checkbox.

    2.  At this point, this will not have any effect, the intent of
        uploading is confirming that what you’ve added thus far doesn’t
        have any issues.

Add the OIDC Claims Provider to User Journey(s)
-----------------------------------------------

At this point, the OIDC IdP has been set up, but it’s not available in
any of the sign-up / sign-in screens (aka User Journeys). In this
section, we’ll make the IdP available in the SignUpOrSignIn User
Journey.

1.  Open the B2C\_1A\_base.xml policy from your working directory.

2.  Find the section with the &lt;UserJourneys&gt; and duplicate the
    &lt;UserJourney&gt; with Id=”SignUpOrSignIn”

3.  Rename the Id of that new &lt;UserJourney&gt; (i.e
    SignUpOrSignInOidc)

4.  In the &lt;OrchestrationStep&gt; with Order=”2”, add a new
    &lt;ClaimsExchange&gt;

    1.  Set the Id following the same convention as the others
        *\[ClaimProviderName\]Exchange*

    2.  Set the TechnicalProfileReferenceId to the same Id value for the
        Technial Profile you set up in the previous section.

5.  In the &lt;OrchestrationStep&gt; with Order=”1”, add a new
    &lt;ClaimsProviderSelection&gt;

    1.  Set the TargetClaimsExchangeId to the same Id value you set up
        for the ClaimsExchange in the previous step.

6.  Save your changes and upload the updated policy

7.  Copy the SignUpOrSignIn.xml file

8.  Rename it match the Id of the new journey you created (i.e.
    SignUpOrSignInOidc)

9.  Modify its PolicyId to a new Guid.

10. Save your changes and upload this new policy.

(Optional) Enable Debugging in User Journey(s)
----------------------------------------------

You can enable debugging tools to help you follow through the each of
the orchestration steps in the UserJourney and get details on issues
that occur. This should only be enabled during development.

1.  Open your new policy xml file (not the base.xml one)

2.  Add the following attributes to the &lt;TrustFrameworkPolicy&gt;
    element:

    1.  DeploymentMode=”Development”

    2.  UserJourneyRecorderEndpoint="https://b2crecorder.azurewebsites.net/stream?id=<guid\>"

        3.  Replace &lt;guid&gt; with an actual GUID

This will allow you to troubleshoot by going to
https://b2crecorder.azurewebsites.net/trace\_102.html?id=<guid\>

Policy Reference
================

Claims Provider
---------------

![](media/idpoidc02.png)

Identity providers, attribute providers, attribute verifiers, directory
provider, MFA provider, self-asserted attribute provider, etc. are all
modelled as claims providers.

To include a list of claims providers along with their technical
profiles that may be used in the various user journeys, a
*ClaimsProviders* XML element must be declared under the above top-level
*TrustFrameworkPolicy* XML element of the policy XML file. This element
is optional.

This element contains the following XML element:

|XML element|Occurrences|Description|
|------------------|-------------|-------------------------------|
| *ClaimsProvider*|1:n|Declare an accredited claims provider that can be leveraged within the community of interested in the various user journeys.|

The *ClaimsProvider* XML elements represents a claims provider, along
with its supported technical profiles. Such an element contains the
following XML elements:

|XML element|Occurrences|Description|
|------------------|-------------|-------------------------------|
|*Domain*|0:1|Specify The human understandable domain name for the claim provider.<br>Type: String|                                     
|*DisplayName*|0:1|Specify the human understandable name of the claims provider that can be displayed to the users. <br>Type: String|
|*TechnicalProfiles*|1:1|Specify a list of technical profiles supported by the claim provider.<br>Every claims provider must have one or more technical profiles which determines the end points and the protocols needed to communicate with that claims provider. In fact, in Azure AD B2C Premium, it is the technical profile that is referenced elsewhere for communication with a particular claims provider.<br>A claims provider can have multiple technical profiles for various reasons. For example, multiple technical profiles may be defined because the considered claims provider supports multiple protocols, various endpoints with different capabilities, or releases different claims at different assurance levels. It may be acceptable to release sensitive claims in one user journey, but not in another one.<br>See next section(s).|

The following XML snippet illustrates how to define a claim provider:

```xml
<?xml version="1.0" encoding="utf-8"?>
<TrustFrameworkPolicy
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:xsd="http://www.w3.org/2001/XMLSchema"
    xmlns="http://schemas.microsoft.com/online/cpim/schemas/2013/06"
    PolicySchemaVersion="0.3.0.0"
    …>

    …

    <ClaimsProviders>
        <ClaimsProvider>
            <DisplayName>Some Claims Provider</DisplayName>
            <TechnicalProfiles>
                <TechnicalProfile …>
                …
                </TechnicalProfile>
                …
            </TechnicalProfiles>
        </ClaimsProvider>
        …
    </ClaimsProviders>
    …
</TrustFrameworkPolicy>
```

Technical Profile
-----------------

As per the aforementioned OIX model, a technical profile consists in a
set of constraints on the use of a specific technology for the exchange
of digital identity information to ensure interoperability and maintain
compliance with required LOA (and/or LOP).

A policy XML file uses technical profiles for two purposes:

1.  Technical details are published as connection metadata to ensure
    on-the-wire interoperability between participants in the community

2.  Technical profiles must be tagged with unique names that are used to
    “label” the interface(s) for which claims providers have been
    certified for conformance to LOA requirements.

Azure AD B2C Premium mediates connections between relying parties and
claims providers. As part of the Trust Framework policy, technical
profiles provide a contract for Azure AD B2C Premium to contact claims
providers. A claims provider can have multiple technical profiles, each
technical profile defining a specific contract for federating with that
specific provider.

For that purpose, it notably consists of:

1.  Protocol – SAML 2.0, WS-Federation/WS-Trust, OpenID Connect (OIDC),
    OAuth 2.0, etc.

2.  Metadata

3.  Cryptographic references – i.e. reference to a secret in the B2C
    tenant secret store.

4.  Input/persisted/output claims:

    1.  Mentions the list of the claims provider can expect in
        request/persist/respond with.

    2.  And their mapping from the *PartnerClaimType* to the claim’s
        used in the policy XML file.

5.  Input/output claims transformations:

    1.  Reference to the *ClaimsTransformation*’s defined in the policy
        XML file.

    2.  Input transforms are run prior to sending the request to the
        provider and output transforms are run on receiving the response
        from the provider.

In addition, a technical profile is usually certified for a LOA and thus
one claims provider may have multiple technical profiles for different
LOAs. As described later in this document for the relying party,
technical profiles similarly provide a contract for relying parties to
contact Azure AD B2C Premium services.

The *TechnicalProfiles* section of the above *ClaimsProviders* XML
element contains the following XML element:

|XML element|Occurrences|Description|
|--------------------|-------------|-----------------------------|
|*TechnicalProfile*|0:n|Define a technical profile supported by the claim provider.|

Each *TechnicalProfile* XML element contains the following attribute:

|Attribute|Required|Description|
|-----------|----------|-----------------------------------|
|*Id*|True|Specify a machine understandable identifier that is used to uniquely identify a particular technical profile, and reference it from other XML elements in the policy XML file, for example *OrchestrationSteps* and *InputTokenSources.*|

Along with the following XML elements:

|XML element|Occurrences|Description|
|------------------|-------------|-------------------|
|*Domain*|0:1|Specify the he human understandable domain name for the technical profile.<br>Type: String|
|*DisplayName*|0:1|Specify the human understandable name of the technical profile that can be displayed to the users.<br>Type: String|
|*Description*|0:1|Specify a human understandable description of the technical profile that can be displayed to the users.<br>Type: String|
|*Protocol*|0:1|Specify the protocol used for the federation.|
|*InputTokenFormat*|0:1|Specify the format of the input token.<br>Type: String (enumeration)<br>Value: one of the types as per *TokenFormat* enumeration in the Azure AD B2C Premium XML schema:<br>-   **JSON**.<br>-   **JWT**. JSON Web Token as per IETF specification.<br>-   **SAML11**. SAML 1.1 security token as per OASIS specification.<br>-   **SAML2**. SAML 2.0 security token as per OASIS specification.<br>-   **CpimUnsigned**.|
|*OutputTokenFormat*|0:1|Specify the format of the output token.<br>Type: String (enumeration)<br>Value: one of the types as per *TokenFormat* enumeration in the Azure AD B2C Premium XML schema. See above.|
|*AssuranceLevelOfOutputClaims*|0:1|List the assurance level of the claims that are retrieved from the technical profile.<br>Type: String|
|*RequiredAssuranceLevelsOfInputClaims*|0:1|Lists the assurance levels that a claim must have in order for it to be used as an input claim to the technical profile.<br>Type: String|
|*SubjectAuthenticationRequirements*|0:1|Specify the requirements regarding the conscious and active participation of the subject in authentication.|
|*Metadata*|0:1|Specify the metadata utilized by the protocol for communicating with the endpoint in the course of a transaction to plumb “on the wire” interoperability between Azure AD B2C Premium and other community participants.<br>Type: collection of *Item* of key/value pairs.|
|*CryptographicKeys*|0:1|Specify a list of cryptographic keys used in this technical profile.<br>For additional information, see section § *Managing your key containers for Trust Framework (policies)* in the third part of this series of document.|
|*Suppressions*|0:1|Specify a list of suppressions supported by the protocol.<br>Type: String|
|*PreferredBinding*|0:1|If the protocol supports multiple bindings, represent binding preferred by the protocol, for example HTTP POST or HTTP GET in the case of SAML 2.0<br>Type: String|
|*IncludeInSSO*|0:1|Indicate whether usage of this technical profile should apply single sign-on (SSO) behavior for the session and instead require explicit interaction.<br>**To be deprecated in favor of *UseTechnicalProfileForSessionManagement* that allows a more granular control on SSO behavior**.|
|*InputTokenSources*|0:1|Represent the list of technical profiles of the claims providers from which the original tokens are to be sent. Azure AD B2C Premium can indeed send the original token from one claims provider to another claims provider.
|*InputClaimsTransformations*|0:1|Specify an optional list of references to claims transformations that should be executed before any claims are sent to the claims provider or the relying party.<br>Each of these elements contains reference to a *ClaimsTransformation* already defined in the *ClaimsTransformations* section.|
|*InputClaims*|0:1|Specify an optional list of the claim types that are taken as input in the technical profile. Each of these elements contains reference to a *ClaimType* already defined in the *ClaimsSchema* section.|
|*PersistedClaims*|0:1|Specify an optional list of the claim types that are persisted by the claims provider that relates to the technical profile. Each of these elements contains reference to a *ClaimType* already defined in the *ClaimsSchema* section.|
|*OutputClaims*|0:1|Specify an optional list of the claim types that are taken as output in the technical profile. Each of these elements contains reference to a *ClaimType* already defined in the *ClaimsSchema* section.|
|*OutputClaimsTransformations*|0:1|Specify an optional list of references to claims transformations that should be executed after claims are received from the claims provider.<br>Each of these elements contains reference to a *ClaimsTransformation* already defined in the *ClaimsTransformations* section.|
|*ValidationTechnicalProfiles *|0:n|Specify a list of other technical profiles that the technical profile uses for validation purposes.|
|*SubjectNamingInfo*|0:1|Control the production of the subject name in tokens (e.g. SAML) where subject name is specified separately from claims.|
|*SubjectAuthenticationRequirements*|0:1|Specify the requirements regarding the conscious and active participation of the subject in authentication|
|*Extensions*|0:1|Allow any xml from any namespace outside of the document namespaces to be included in the element. This represents an extension point for elements as its name indicates.|
|*IncludeClaimsFromTechnicalProfile*|0:1|Indicate a machine understandable identifier that is used to uniquely identify a different technical profile. All input and output claims from referenced technical profile will be added to this technical profile. Referenced technical profile must be defined in the same policy XML file.<br>Type: String|
|*IncludeTechnicalProfile*|0:1|Indicate a machine understandable identifier that is used to uniquely identify a different technical profile. All data from referenced technical profile will be added to this technical profile. Referenced technical profile must exist in the same policy XML file.|
|*UseTechnicalProfileForSessionManagement*|0:1|Indicate a machine understandable identifier to uniquely identify a different technical profile to be used for session management.|
|*EnableForUserJourney*|0:1|Specify if the technical user profile is enable for a user journey. if the technical profile should be used within a user journey, this includes *ClaimProviderSelections*. If this value is set to **true**, it will disable the selection.<br> Value: one of the following values as per *EnabledForUserJourneysValues* enumeration in the Azure AD B2C Premium XML schema:<br>-   **true**. Execute the technical profile.<br>-   **false**. Always skip the technical profile.<br>-   **OnClaimsExistence**. Only execute the technical profile if the claim specified in the technical profile's metadata is present in the user journey storage.<br>-   **Always**. (default) Execute the technical profile.<br>-   **Never**. Always skip the technical profile.|


The *Protocol* XML element in the above table contains the following
attributes:

|Attribute|Required|Description|
|-----------|----------|--------------------------|
|*Name*|True|Specify the name of a valid protocol supported by Azure AD B2C Premium that is used as part of the technical profile.<br>Type: String (enumeration)<br>Value: one of the following types as per *ProtocolName* enumeration in the Azure AD B2C Premium XML schema:<br>-   **None**.<br>-   **OAuth1**. OAuth 1.0 protocol standard as per IETF specification.<br>-   **OAuth2**. OAuth 2.0 protocol standard as per IETF specification.<br>-   **SAML2**. SAML 2.0 protocol standard as per OASIS specification.<br>-   **OpenIdConnect**. OpenID Connect 1.0 protocol standard as per OpenID foundation specification.<br>-   **WsFed**. WS-Federation (WS-Fed) 1.2 protocol standard as per OASIS specification.<br>-   **WsTrust**. WS-Trust 1.3 protocol standard as per OASIS specification.<br>-   **Proprietary**. For a RESTful based provider.|
|*Handler*|False|Specify the fully-qualified name of the assembly that will be used by Azure AD B2C Premium to determine the protocol handler if the protocol name is set to "Proprietary". **It is invalid to provide this attribute with any other protocol name.**<br>Type: String|

The *RequiredAssuranceLevelsOfInputClaims* XML element in the above
table contains the following XML element:

|XML element|Occurrences|Description|
|-----------|-----------|-----------|
|*RequiredAssuranceLevelOfInputClaims*|0:n|List an assurance level that a claim must have in order for it to be used as an input claim to the technical profile.|

The *SubjectAuthenticationRequirements* XML element in the above table
contains the following XML attributes:


|Attribute|Required|Description|
|---------|--------|-----------|
|*TimeToLive*|True|Specify The maximum number of minutes cached credentials can be used following an active authentication by the subject.<br>Type: Integer|
|*ResetExpiryWhenTokenIssued*|False|If **true** then whenever a token is issued (even using a cached credential), set the expiry time to the current time plus the TimeToLive value. Default is **false**.<br>Type: Boolean|

The *CryptographicKeys* XML element in the above table contains the
following XML element:

|XML element|Occurrences|Description|
|-------------|-------------|-------------------------|
|*Key*|0:n|Define a cryptographic key used in this technical profile.|

Each *Key* XML element contains in turn the following attribute:

|Attribute|Required|Description|
|---------|----------|-------------------------------|
|*Id*|True|Specify a machine understandable identifier that is used to uniquely identify a particular key (pair), and reference it from other XML elements in the policy XML file.<br>Type: String|
|*StorageReferenceId*|True|Specify a machine understandable identifier that is used to uniquely identify a particular storage key container, and reference it from other XML elements in the policy XML file. For additional information, see section § *Managing your key containers for Trust Framework (policies)* in third part of this series of document.<br>Type: String|

The *InputTokenSources* XML elements contain the following XML element:

|XML element|Occurrences|Description|
|--------------|---------|---------------------------------------------|
|*TechnicalProfile*|1:n|Specify a source for that can be the input assertions for the current technical profile.

Each *TechnicalProfile* XML element contains the following attribute:

|Attribute|Required|Description|
|-------|-----|-------------------------|
|*Id*|True|Specify a reference to a machine understandable identifier that is used to uniquely identify a particular technical profile in the policy XML file.<br>Type: String|

Each *InputClaim* XML element contains the following attribute:

|Attribute|Required|Description|
|------------------|------|------------------------------|
|*ClaimTypeReferenceId*|True|Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.<br> Type: String|

The *InputClaimsTransformations* XML elements contain the following XML
element:

|XML element|Occurrences|Description|
|--------------------------|---------|-------------------------|
|*InputClaimsTransformation*|0:n|Specify a claims transformation that should be executed before any claims are sent to the claims provider or the relying party. A claims transformation can indeed be used to modify existing *ClaimsSchema* claims or generate new ones.|

Each *InputClaimsTransformation* XML element contains the following
attribute:

|Attribute|Required|Description|
|-------------|-----|----------------------------|
|*ReferenceId*|True|Specify a reference to a *ClaimsTransformation* already defined in the *ClaimsTransformations* section in the policy XML file.<br> Type: String|

The *InputClaims* XML elements contain the following XML element:

|XML element|Occurrences|Description|
|------------|----------|------------------------|
|*InputClaim*|0:n|Specify an expected claim type.|  

Each *InputClaim* XML element contains the following attribute:

|Attribute|Required|Description|
|-----------------------|-------|---------------------------------------|
|*ClaimTypeReferenceId*|True|Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.<br>Type: String|

The *PersistedClaims* XML elements contain the following XML element:

|XML element|Occurrences|Description|
|-------------|----------|-------------------------------------|
|*PersistedClaim*|0:n|Specify an expected claim type. Claim mappings are used to determine the provider claim type before sending to the claims provider.|

Each *PersistedClaim* XML element contains the following attributes:

|Attribute|Required|Description|
|-------------------|------|---------------------|
|*ClaimTypeReferenceId*|True|Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.<br>Type: String|
|*DefaultValue*|False|Specify a default value to create a claim if the claim indicated by *ClaimTypeReferenceId* does not exist so that the resulting claim can be used as an *InputClaim* by the technical profile.<br>Type: String|
|*PartnerClaimType*|False|Identify the *ClaimType* of the external partner that the specified policy claim type maps to. If the *PartnerClaimType* attribute is not specified, then the specified policy claim type is mapped to the partner claim type of the same name.<br>Type: String|
|*OverwriteIfExists*|False|Provides an optional property to the claims provider indicating whether the claim can be overwritten in the claims provider records if the claim provider supports overwriting.<br>Type: true or false|

Eventually, the *OutputClaims* XML elements contain the following XML
element:

|XML element|Occurrences|Description|
|----------|--------|--------------------------------|
|*OutputClaim*|0:n|Specify an expected claim type.|

Each *OutputClaim* XML element contains the following attributes:

|Attribute|Required|Description|
|-------------|--------|-------------------------------------|
|*ClaimTypeReferenceId*|True|Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.<br>Type: String|
|*DefaultValue*|False|Specify a default value if not set.<br>Type: String|
|*PartnerClaimType*|False|Specify the partner claim type.<br>Type: String|
|*Required*|False|Specify this claim is required.<br>Type: String|


The *OutputClaimsTransformations* XML elements contain the following XML
element:

|XML element|Occurrences|Description|
|------------------------|----------|----------------------------------|
|*OutputClaimsTransformation*|0:n|Specify a claims transformation that should be executed after claims are received from the claims provider. A claims transformation can indeed be used to modify existing *ClaimsSchema* claims or generate new ones.|

Each *OutputClaimsTransformation* XML element contains the following
attribute:


|Attribute|Required|Description|
|------------|------|----------------------------|
|*ReferenceId*|True|Specify a reference to a *ClaimsTransformation* already defined in the *ClaimsTransformations* section in the policy XML file.<br>Type: String|


The *ValidationTechnicalProfiles* XML element in the above table
contains the following XML element:

|XML element|Occurrences|Description|
|----------------------------|----------|------------------------|
|*ValidationTechnicalProfile*|1:n|Define a to be used for validating some or all of the output claims of the referencing technical profile. Therefore, all the input claims of the referenced technical profile must appear in the output claims of the referencing technical profile.|

*ValidationTechnicalProfile* and *IncludeTechnicalProfile* XML elements
contain in turn the following attributes:

|Attribute|Required|Description|
|-------------|------|---------------------------------------|
|*ReferenceId*|True|Specify a reference to a machine understandable identifier that is used to uniquely identify a particular technical profile already defined in the policy XML file.|

The *SubjectNamingInfo* XML element in the above table contains the
following attributes:

|Attribute|Required|Description|
|-------------------|----------|---------------------|
|*ClaimType*|True|Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.<br>Type: String|
|*NameQualifier*|False|Provide a description<br>Type: String|
|*SPNameQualifier*|False|Provide a description<br> Type: String|
|*Format*|False|Provide a description<br>Type: String|
|*SPProvidedID*|False|Provide a description<br>Type: String|

*IncludeTechnicalProfile* and *UseTechnicalProfileForSessionManagement*
XML elements in the above table contain the following attribute:

|Attribute|Required|Description|
|--------------|---------|----------------------|
|*ReferenceId*|True|Specify a reference to a machine understandable identifier that is used to uniquely identify a particular technical profile already defined in the policy XML file.|

The following XML snippet illustrates how to define a technical profile
for a claim provider:

```xml
<?xml version="1.0" encoding="utf-8"?>
<TrustFrameworkPolicy
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:xsd="http://www.w3.org/2001/XMLSchema"
    xmlns="http://schemas.microsoft.com/online/cpim/schemas/2013/06"
    PolicySchemaVersion="0.3.0.0"
    …>
    …
    <ClaimsProviders>
        <ClaimsProvider>
            <DisplayName>Some Claims Provider</DisplayName>
            <TechnicalProfiles>
                <TechnicalProfile …>
                    …
                </TechnicalProfile>
                …
            </TechnicalProfiles>
        </ClaimsProvider>
        …
    </ClaimsProviders>
    …
</TrustFrameworkPolicy>
```

**The next sections further detail how to specify adequate technical
profiles depending on the nature of the claims provider.** It more
particularly describes the related metadata information to specify.

OpenID Connect Claims Provider
------------------------------

Azure AD B2C Premium provides a support for the OpenID Connect protocol.
This section outlines the specifics of a technical profile for
interacting with claims provider supporting this standardized protocol.

OpenID Connect 1.0 defines an identity layer on top of OAuth 2.0 and
represents the state of the art in modern authentication protocols. It’s
a suite of lightweight specifications that provide a framework for
identity interactions via REST like APIs. It is based on OAuth 2.0.

> **Note** For more information about and details that pertain to OpenID
> Connect, see the specification [<span
> style="font-variant:small-caps;">OpenID Connect Core
> 1.0</span>](http://openid.net/specs/openid-connect-core-1_0.html)[^1].
>
> **Note** The OpenID Foundation has recently launched a certification
> program for OpenID Connect 1.0 implementations. For more information,
> see the article [<span style="font-variant:small-caps;">The OpenID
> Foundation Launches OpenID Connect Certification
> Program</span>](http://openid.net/2015/04/17/openid-connect-certification-program/)[^2].
>
> Having an OpenID Connect certification program provides confidence
> that certified implementations will "just work" together. This
> represents another important step on the road to widely-available
> secure interoperable digital identity for all the devices and
> applications that people use. Microsoft is proud to be a key
> contributor to the development of OpenID Connect 1.0 and now of its
> certification program. Azure AD has successfully passed the
> certification and is [certified](http://openid.net/certification/)[^3]
> as an OpenID Connect 1.0 identity provider.

OpenID Connect 1.0 provides a JSON configuration document
(*openid-configuration.json*) as per [OpenID Connect
Discovery](http://openid.net/specs/openid-connect-discovery-1_0.html)[^4]
specification. This metadata information in JSON format provides
configuration information such as the OAuth 2.0 endpoint locations, the
signing key and issuer values to validate, etc. in analogy to what
happens for WS-Federation and SAML 2.0 as described before.

With this in mind, a suitable technical profile definition for an OAuth
2.0 claims provider implies the followings.

The *Name* attribute of the *Protocol* XML element has to be set to
**OpenIdConnect** as per *ProtocolName* enumeration in the Azure AD B2C
Premium XML schema.

The following keys must or may be present in the *Metadata* XML element:

|Key|Required|Description|
|-------------------------|----------|-------------------------------------|
|client\_id|False|Specify the identifier attributed by the OpenID Connect provider.|
|IdTokenAudience|False|Specify the audience of the id token.|
|ProviderName|True|Specify the name of the OpenID Connect provider.|
|METADATA|False|Specify the JSON configuration document as per OpenID Connect Discovery specification.|
|authorization_endpoint|True|Indicate the URL of the authorization endpoint as per OpenID Connect Core 1.0 specification.|
|redirect_uri|False|Indicate the redirection point URL as per OpenID Connect Core 1.0 specification.|
|response_types|False|Specify the response type as per OpenID Connect Core 1.0 specification.<br>Value: id\_token, code or token|
|response_mode|False|Specify the response mode.<br>Value: query|
|scope|False|Specify the scope of the access request as per OpenID Connect Core 1.0 specification.|
|issuer|False|Specify the issuer of the access request as per OpenID Connect Core 1.0 specification.|
|HttpBinding|False|Specify the expected HTTP binding.|<br>Value: **GET** or **POST**|
|LocalAccountProfile|False|TBD.<br>Value: true or false|

The *InputClaimsTransformations* XML element is absent.

The *InputClaims* XML element contains the claims bag as the input with
possible mapping information between a *ClaimType* already defined in
the *ClaimsSchema* section in the policy XML file and that partner claim
type.

The *PersistedClaims* XML element is absent.

The *OutputClaims* XML element contains the claims bag as the output
with possible mapping information between a *ClaimType* already defined
in the *ClaimsSchema* section in the policy XML file and that partner
claim type, and/or a default value.

The *OutputClaimsTransformations* XML element may contain a collection
of *OutputClaimsTransformation* to be used to modify the output claims
or generate new ones.

The following XML snippet illustrates a technical profile for an OpenID
Connect claims provider:

```xml
<TechnicalProfile Id="LocalAccountSignInWithEvoSts">
	<DisplayName>Local Account SignIn</DisplayName>
	<Protocol Name="OpenIdConnect" />
	<Metadata>
		<Item Key="ProviderName">https://sts.windows.net/</Item>
		<Item Key="METADATA">https://login.microsoftonline.com/{tenant}/.well-known/openid-configuration</Item>
		<Item Key="authorization_endpoint">https://login.microsoftonline.com/{tenant}/oauth2/authorize</Item>
		<Item Key="response_types">id_token</Item>
		<Item Key="response_mode">query</Item>
		<Item Key="scope">email openid</Item>
		<Item Key="client_id">bb2a2e3a-c5e7-4f0a-88e0-8e01fd3fc1f4</Item>
		<Item Key="IdTokenAudience">bb2a2e3a-c5e7-4f0a-88e0-8e01fd3fc1f4</Item>
		<Item Key="UsePolicyInRedirectUri">false</Item>
		<Item Key="HttpBinding">POST</Item>
		<Item Key="LocalAccountProfile">true</Item>
	</Metadata>
	<InputClaims>
		<InputClaim ClaimTypeReferenceId="logonIdentifier"PartnerClaimType="login_hint" />
		<InputClaim ClaimTypeReferenceId="nux" PartnerClaimType="nux"DefaultValue="1" />
		<InputClaim ClaimTypeReferenceId="nca" PartnerClaimType="nca"DefaultValue="1" />
		<InputClaim ClaimTypeReferenceId="tenantId"PartnerClaimType="domain_hint"DefaultValue="{Policy:RelyingPartyTenantId}" />
		<InputClaim ClaimTypeReferenceId="prompt" PartnerClaimType="prompt"DefaultValue="{OIDC:prompt}" />
		<InputClaim ClaimTypeReferenceId="mkt" PartnerClaimType="mkt"DefaultValue="{Culture:RFC5646}" />
		<InputClaim ClaimTypeReferenceId="lc" PartnerClaimType="lc"DefaultValue="{Culture:LCID}" />
	</InputClaims>
	<OutputClaims>
		<OutputClaim ClaimTypeReferenceId="objectId" PartnerClaimType="oid"/>
		<OutputClaim ClaimTypeReferenceId="tenantId" PartnerClaimType="tid"/>
		<OutputClaim ClaimTypeReferenceId="givenName"PartnerClaimType="given_name" />
		<OutputClaim ClaimTypeReferenceId="surName"PartnerClaimType="family_name" />
		<OutputClaim ClaimTypeReferenceId="displayName"PartnerClaimType="name" />
		<OutputClaim ClaimTypeReferenceId="userPrincipalName"PartnerClaimType="upn" />
		<OutputClaim ClaimTypeReferenceId="authenticationSource"DefaultValue="evoStsAuthentication" />
	</OutputClaims>
	<OutputClaimsTransformations>
		<OutputClaimsTransformationReferenceId="CreateSubjectClaimFromObjectID" />
	</OutputClaimsTransformations>
</TechnicalProfile>
```
