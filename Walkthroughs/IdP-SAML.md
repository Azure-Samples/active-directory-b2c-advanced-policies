Scenario
========

This document will walk you through adding a SAML-based Identity
provider to Azure AD B2C.

This will enable scenarios such as:

-   Contoso wants to build an app, *Contoso Rewards*, for their
    customers, who are consumers, to keep track of their rewards points
    obtained by purchasing Contoso products in retail stores. Contoso
    also wants to allow Contoso employees to sign in to the same app
    using their corporate credentials to create and manage activities
    that customers can participate in to get more rewards.

    -   Use Azure AD B2C

    -   Local accounts + social IdPs for consumers

    -   SAML IdP for Contoso’s SAML-based IdP

This walkthrough will only focus on the SAML IdP piece of this scenario.

Walkthrough
===========

Setup
-----

Make sure you first complete the [Basic Setup tutorial](BasicSetup.md).

Create the SAML Relaying Party / App in the SAML IdP
----------------------------------------------------

You’ll need to register B2C as a relaying party in the SAML IdP. Each
IdP has different steps to do so, look at your IdPs documentation for
guidance on how to do so. You will be required to provide the following
data points:

-   **Identifier / Entity Id:**

    https://login.microsoftonline.com/te/<tenantName\>.onmicrosoft.com/B2C\_1A\_base

-   **Reply URL / Assertion Consumer Service (ACS) URL:**

    https://login.microsoftonline.com/te/<tenantName\>.onmicrosoft.com/B2C\_1A\_base/samlp/sso/assertionconsumer

Create the SAML Claims Provider
-------------------------------

Now that you’ve got working set of advanced policies, let’s go ahead and
add the SAML IdP.

> ***Note**: The Policy Reference section at the end of this doc
> contains more details around each of the XML elements referenced in
> this section.*

1.  Open the B2C_1A_base.xml policy from your working directory.

2.  Find the section with the &lt;ClaimsProviders&gt; and add a new &lt;ClaimsProvider&gt; as follows:


```xml

    <ClaimsProvider>
      <Domain>contoso</Domain>
      <DisplayName>Contoso</DisplayName>
      <TechnicalProfiles>
        <TechnicalProfile Id="Contoso">
          <DisplayName>Contoso</DisplayName>
          <Description>Login with your Contoso account</Description>
          <Protocol Name="SAML2"/>
          <Metadata>
            <Item Key="RequestsSigned">false</Item>
            <Item Key="WantsEncryptedAssertions">false</Item>
            <Item Key="PartnerEntity">
    <![CDATA[ <md:EntityDescriptor xmlns:md="urn:oasis:names:tc:SAML:2.0:metadata" entityID="https://contoso.com" validUntil="2026-10-05T23:57:13.854Z" xmlns:ds="http://www.w3.org/2000/09/xmldsig#"><md:IDPSSODescriptor protocolSupportEnumeration="urn:oasis:names:tc:SAML:2.0:protocol"><md:KeyDescriptor use="signing"><ds:KeyInfo><ds:X509Data><ds:X509Certificate>MIIErDCCA….qY9SjVXdu7zy8tZ+LqnwFSYIJ4VkE9UR1vvvnzO</ds:X509Certificate></ds:X509Data></ds:KeyInfo></md:KeyDescriptor><md:NameIDFormat>urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified</md:NameIDFormat><md:SingleSignOnService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST" Location="https://contoso.com/idp/endpoint/HttpPost"/><md:SingleSignOnService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect" Location="https://contoso.com/idp/endpoint/HttpRedirect"/></md:IDPSSODescriptor></md:EntityDescriptor>]]></Item>
          </Metadata>       
          <CryptographicKeys>
            <Key Id="SamlAssertionSigning" StorageReferenceId="ContosoIdpSamlCert"/>
            <Key Id="SamlMessageSigning" StorageReferenceId="ContosoIdpSamlCert "/>
          </CryptographicKeys>
          <OutputClaims>
            <OutputClaim ClaimTypeReferenceId="userId" PartnerClaimType="userId"/>
            <OutputClaim ClaimTypeReferenceId="identityProvider" DefaultValue="SAML Idp" />
            <OutputClaim ClaimTypeReferenceId="givenName" PartnerClaimType="given_name"/>
            <OutputClaim ClaimTypeReferenceId="surname" PartnerClaimType="family_name"/>
            <OutputClaim ClaimTypeReferenceId="email" PartnerClaimType="email"/>
            <OutputClaim ClaimTypeReferenceId="displayName" PartnerClaimType="name"/>
            <OutputClaim ClaimTypeReferenceId="authenticationSource" DefaultValue="externalIdp"/>
          </OutputClaims>
          <OutputClaimsTransformations>
            <OutputClaimsTransformation ReferenceId="CreateRandomUPNUserName"/>
            <OutputClaimsTransformation ReferenceId="CreateUserPrincipalName"/>
            <OutputClaimsTransformation ReferenceId="CreateAlternativeSecurityId"/>
            <OutputClaimsTransformation ReferenceId="CreateSubjectClaimFromAlternativeSecurityId"/>
          </OutputClaimsTransformations>
          <UseTechnicalProfileForSessionManagement ReferenceId="SM-Noop"/>
        </TechnicalProfile>
      </TechnicalProfiles>
    </ClaimsProvider>

```

3.  Configure basic settings

    1.  ClaimsProvider/Domain - Drives the value that can be passed in
        to
        [domain_hint](http://www.cloudidentity.com/blog/2014/11/17/skipping-the-home-realm-discovery-page-in-azure-ad/)
        query string parameter, so make it url friendly, i.e. lowercase
        only, no spaces, only alphanumerical.

    2.  ClaimsProvider/DisplayName – Currently not displayed anywhere

    3.  TechnicalProfile@Id – Unique identifier for the technical
        profile, it is referenced elsewhere in the policy (see “Add the
        SAML Claims Provider to the User Journey”).

    4.  TechnicalProfile/DisplayName - Displayed as the button's caption
        in the sign in screen.

        ![](media/idpsaml01.png)

    5.  TechnicalProfile/Description – Currently not displayed anywhere

4.  Configure Metadata section

    1.  PartnerEntity – Either the URL to the metadata endpoint or the
        metadata itself encapsulated by <!\[CDATA\[ *…metadata…* \]\]\>;

    2.  Configure OutputClaims - Map each of the claims in this section
        to a claim in the SAML token response.

        1.  ClaimTypeReferenceId is the name of the property in B2C, do
            not tweak these.

        2. For each of those, set either the DefaultValue or the
            PartnerClaimType.

5.  Upload Certs - These are the certificates used to sign the SAML
    request and message. Even though we’ve configured the Claims
    Provider to not sign these, a certificate must still be provided.

    1.  (If you don’t have a cert already) [Create the cert using makecert](http://www.virtues.it/2015/08/howto-create-selfsigned-certificates-with-makecert/). `makecert` is typically found in a subfolder of `C:\Program Files (x86)\Windows Kits\<Version>\bin` after installing the Windows SDK in Visual Studio.

        1.  `makecert -r -pe -n "CN=idpsaml.yourtenant.onmicrosoft.com"
            -a sha256 -sky signature -len 2048 -e 12/21/2018 -sr
            CurrentUser -ss My ContosoIdpSamlCert.cer`

        2. Go to cert store “Manage User Certificates” &gt; Current
            User &gt; Personal &gt; Certificates &gt;
            idpsaml.yourtenant.onmicrosoft.com

        3. Right click &gt; All Tasks &gt; Export

        4. Yes, export the private key

        5.  Defaults (PFX and first checkbox)

    2.  Run `powershell -noprofile`

    3.  Change directory to the location of the `ExploreAdmin` folder inside this repo

    4.  Run `Import-Module .\ExploreAdmin.dll` (if it fails, it might be because
        the dll hasn't been unblocked)

    5.  Run `New-CpimCertificate -TenantId yourtenant.onmicrosoft.com
        -CertificateId ContosoIdpSamlCert -CertificateFileName path
        -CertificatePassword password`

        -  When you run the command, make sure you sign in with the
            onmicrosoft.com account local to the tenant.

        - You will be prompted for MFA.
        
        - If you have trouble logging in due to multiple directories, run `` to open a shell that will not use cached credentials in your profile. You can use `Login-AzureRmAccount` to authenticate, and then execute the commands above in there. 

6.  Save your changes and upload updated policy

    1.  This time, make sure you check the *Overwrite the policy if it
        exists* checkbox.

    2.  At this point, this will not have any effect, the intent of
        uploading is confirming that what you’ve added thus far doesn’t
        have any issues.

Add the SAML Claims Provider to User Journey(s)
-----------------------------------------------

At this point, the SAML IdP has been set up, but it’s not available in
any of the sign-up / sign-in screens (aka User Journeys). In this
section, we’ll make the IdP available in the SignUpOrSignIn User
Journey.

1.  Open the B2C\_1A\_base.xml policy from your working directory.

2.  Find the section with the &lt;UserJourneys&gt; and duplicate the
    &lt;UserJourney&gt; with Id=”SignUpOrSignIn”

3.  Rename the Id of that new &lt;UserJourney&gt; (i.e
    SignUpOrSignInSaml)

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
    SignUpOrSignInSaml)

9.  Modify its PolicyId to a new Guid.

10. Save your changes and upload this new policy.

 (Optional) Enable Debugging in User Journey(s)
-----------------------------------------------

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

Claims Provider
---------------

![](media/idpsaml02.png)

Identity providers, attribute providers, attribute verifiers, directory
provider, MFA provider, self-asserted attribute provider, etc. are all
modelled as claims providers.

To include a list of claims providers along with their technical
profiles that may be used in the various user journeys, a
*ClaimsProviders* XML element must be declared under the above top-level
*TrustFrameworkPolicy* XML element of the policy XML file. This element
is optional.

This element contains the following XML element:

  XML element        Occurrences   Description
  ------------------ ------------- ------------------------------------------------------------------------------------------------------------------------------
  *ClaimsProvider*   1:n           Declare an accredited claims provider that can be leveraged within the community of interested in the various user journeys.

The *ClaimsProvider* XML elements represents a claims provider, along
with its supported technical profiles. Such an element contains the
following XML elements:

  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  XML element           Occurrences   Description
  --------------------- ------------- ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *Domain*              0:1           Specify The human understandable domain name for the claim provider.
                                      
                                      Type: String

  *DisplayName*         0:1           Specify the human understandable name of the claims provider that can be displayed to the users.
                                      
                                      Type: String

  *TechnicalProfiles*   1:1           Specify a list of technical profiles supported by the claim provider.
                                      
                                      Every claims provider must have one or more technical profiles which determines the end points and the protocols needed to communicate with that claims provider. In fact, in Azure AD B2C Premium, it is the technical profile that is referenced elsewhere for communication with a particular claims provider.
                                      
                                      A claims provider can have multiple technical profiles for various reasons. For example, multiple technical profiles may be defined because the considered claims provider supports multiple protocols, various endpoints with different capabilities, or releases different claims at different assurance levels. It may be acceptable to release sensitive claims in one user journey, but not in another one.
                                      
                                      See next section(s).
  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

The following XML snippet illustrates how to define a claim provider:

```xml
<?xml version="1.0" encoding="utf-8"?>

<TrustFrameworkPolicy xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
xmlns="http://schemas.microsoft.com/online/cpim/schemas/2013/06" PolicySchemaVersion="0.3.0.0" …>
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

    a.  Mentions the list of the claims provider can expect in
        request/persist/respond with.

    b.  And their mapping from the *PartnerClaimType* to the claim’s
        used in the policy XML file.

5.  Input/output claims transformations:

    a.  Reference to the *ClaimsTransformation*’s defined in the policy
        XML file.

    b.  Input transforms are run prior to sending the request to the
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
|-----------|--------|--------------------------------------|
|*TechnicalProfile*|0:n|Define a technical profile supported by the claim provider.|

Each *TechnicalProfile* XML element contains the following attribute:

  Attribute   Required   Description
  ----------- ---------- ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *Id*        True       Specify a machine understandable identifier that is used to uniquely identify a particular technical profile, and reference it from other XML elements in the policy XML file, for example *OrchestrationSteps* and *InputTokenSources.*

Along with the following XML elements:

  ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  XML element                                 Occurrences   Description
  ------------------------------------------- ------------- ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *Domain*                                    0:1           Specify the he human understandable domain name for the technical profile.
                                                            
                                                            Type: String

  *DisplayName*                               0:1           Specify the human understandable name of the technical profile that can be displayed to the users.
                                                            
                                                            Type: String

  *Description*                               0:1           Specify a human understandable description of the technical profile that can be displayed to the users.
                                                            
                                                            Type: String

  *Protocol*                                  0:1           Specify the protocol used for the federation.

  *InputTokenFormat*                          0:1           Specify the format of the input token.
                                                            
                                                            Type: String (enumeration)
                                                            
                                                            Value: one of the types as per *TokenFormat* enumeration in the Azure AD B2C Premium XML schema:
                                                            
                                                            -   **JSON**.
                                                            
                                                            -   **JWT**. JSON Web Token as per IETF specification.
                                                            
                                                            -   **SAML11**. SAML 1.1 security token as per OASIS specification.
                                                            
                                                            -   **SAML2**. SAML 2.0 security token as per OASIS specification.
                                                            
                                                            -   **CpimUnsigned**.
                                                            

  *OutputTokenFormat*                         0:1           Specify the format of the output token.
                                                            
                                                            Type: String (enumeration)
                                                            
                                                            Value: one of the types as per *TokenFormat* enumeration in the Azure AD B2C Premium XML schema. See above.

  *AssuranceLevelOfOutputClaims*              0:1           List the assurance level of the claims that are retrieved from the technical profile.
                                                            
                                                            Type: String

  *RequiredAssuranceLevelsOfInputClaims*      0:1           Lists the assurance levels that a claim must have in order for it to be used as an input claim to the technical profile.
                                                            
                                                            Type: String

  *SubjectAuthenticationRequirements*         0:1           Specify the requirements regarding the conscious and active participation of the subject in authentication.

  *Metadata*                                  0:1           Specify the metadata utilized by the protocol for communicating with the endpoint in the course of a transaction to plumb “on the wire” interoperability between Azure AD B2C Premium and other community participants.
                                                            
                                                            Type: collection of *Item* of key/value pairs.

  *CryptographicKeys*                         0:1           Specify a list of cryptographic keys used in this technical profile.
                                                            
                                                            For additional information, see section § *Managing your key containers for Trust Framework (policies)* in the third part of this series of document.

  *Suppressions*                              0:1           Specify a list of suppressions supported by the protocol.
                                                            
                                                            Type: String

  *PreferredBinding*                          0:1           If the protocol supports multiple bindings, represent binding preferred by the protocol, for example HTTP POST or HTTP GET in the case of SAML 2.0
                                                            
                                                            Type: String

  *IncludeInSSO*                              0:1           Indicate whether usage of this technical profile should apply single sign-on (SSO) behavior for the session and instead require explicit interaction.
                                                            
                                                            **To be deprecated in favor of *UseTechnicalProfileForSessionManagement* that allows a more granular control on SSO behavior**.

  *InputTokenSources*                         0:1           Represent the list of technical profiles of the claims providers from which the original tokens are to be sent. Azure AD B2C Premium can indeed send the original token from one claims provider to another claims provider.

  *InputClaimsTransformations*                0:1           Specify an optional list of references to claims transformations that should be executed before any claims are sent to the claims provider or the relying party.
                                                            
                                                            Each of these elements contains reference to a *ClaimsTransformation* already defined in the *ClaimsTransformations* section.

  *InputClaims*                               0:1           Specify an optional list of the claim types that are taken as input in the technical profile. Each of these elements contains reference to a *ClaimType* already defined in the *ClaimsSchema* section.

  *PersistedClaims*                           0:1           Specify an optional list of the claim types that are persisted by the claims provider that relates to the technical profile. Each of these elements contains reference to a *ClaimType* already defined in the *ClaimsSchema* section.

  *OutputClaims*                              0:1           Specify an optional list of the claim types that are taken as output in the technical profile. Each of these elements contains reference to a *ClaimType* already defined in the *ClaimsSchema* section.

  *OutputClaimsTransformations*               0:1           Specify an optional list of references to claims transformations that should be executed after claims are received from the claims provider.
                                                            
                                                            Each of these elements contains reference to a *ClaimsTransformation* already defined in the *ClaimsTransformations* section.

  *ValidationTechnicalProfiles *              0:n           Specify a list of other technical profiles that the technical profile uses for validation purposes.

  *SubjectNamingInfo*                         0:1           Control the production of the subject name in tokens (e.g. SAML) where subject name is specified separately from claims.

  *SubjectAuthenticationRequirements*         0:1           Specify the requirements regarding the conscious and active participation of the subject in authentication

  *Extensions*                                0:1           Allow any xml from any namespace outside of the document namespaces to be included in the element. This represents an extension point for elements as its name indicates.

  *IncludeClaimsFromTechnicalProfile*         0:1           Indicate a machine understandable identifier that is used to uniquely identify a different technical profile. All input and output claims from referenced technical profile will be added to this technical profile. Referenced technical profile must be defined in the same policy XML file.
                                                            
                                                            Type: String

  *IncludeTechnicalProfile*                   0:1           Indicate a machine understandable identifier that is used to uniquely identify a different technical profile. All data from referenced technical profile will be added to this technical profile. Referenced technical profile must exist in the same policy XML file.

  *UseTechnicalProfileForSessionManagement*   0:1           Indicate a machine understandable identifier to uniquely identify a different technical profile to be used for session management.

  *EnableForUserJourney*                      0:1           Specify if the technical user profile is enable for a user journey. if the technical profile should be used within a user journey, this includes *ClaimProviderSelections*. If this value is set to **true**, it will disable the selection.
                                                            
                                                            Value: one of the following values as per *EnabledForUserJourneysValues* enumeration in the Azure AD B2C Premium XML schema:
                                                            
                                                            -   **true**. Execute the technical profile.
                                                            
                                                            -   **false**. Always skip the technical profile.
                                                            
                                                            -   **OnClaimsExistence**. Only execute the technical profile if the claim specified in the technical profile's metadata is present in the user journey storage.
                                                            
                                                            -   **Always**. (default) Execute the technical profile.
                                                            
                                                            -   **Never**. Always skip the technical profile.
                                                            
  ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

The *Protocol* XML element in the above table contains the following
attributes:

  -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Attribute   Required   Description
  ----------- ---------- ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *Name*      True       Specify the name of a valid protocol supported by Azure AD B2C Premium that is used as part of the technical profile.
                         
                         Type: String (enumeration)
                         
                         Value: one of the following types as per *ProtocolName* enumeration in the Azure AD B2C Premium XML schema:
                         
                         -   **None**.
                         
                         -   **OAuth1**. OAuth 1.0 protocol standard as per IETF specification.
                         
                         -   **OAuth2**. OAuth 2.0 protocol standard as per IETF specification.
                         
                         -   **SAML2**. SAML 2.0 protocol standard as per OASIS specification.
                         
                         -   **OpenIdConnect**. OpenID Connect 1.0 protocol standard as per OpenID foundation specification.
                         
                         -   **WsFed**. WS-Federation (WS-Fed) 1.2 protocol standard as per OASIS specification.
                         
                         -   **WsTrust**. WS-Trust 1.3 protocol standard as per OASIS specification.
                         
                         -   **Proprietary**. For a RESTful based provider.
                         

  *Handler*   False      Specify the fully-qualified name of the assembly that will be used by Azure AD B2C Premium to determine the protocol handler if the protocol name is set to "Proprietary". **It is invalid to provide this attribute with any other protocol name.**
                         
                         Type: String
  -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

The *RequiredAssuranceLevelsOfInputClaims* XML element in the above
table contains the following XML element:

  XML element                             Occurrences   Description
  --------------------------------------- ------------- -----------------------------------------------------------------------------------------------------------------------
  *RequiredAssuranceLevelOfInputClaims*   0:n           List an assurance level that a claim must have in order for it to be used as an input claim to the technical profile.

The *SubjectAuthenticationRequirements* XML element in the above table
contains the following XML attributes:

  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Attribute                      Required   Description
  ------------------------------ ---------- ------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *TimeToLive*                   True       Specify The maximum number of minutes cached credentials can be used following an active authentication by the subject.
                                            
                                            Type: Integer

  *ResetExpiryWhenTokenIssued*   False      If **true** then whenever a token is issued (even using a cached credential), set the expiry time to the current time plus the TimeToLive value. Default is **false**.
                                            
                                            Type: Boolean
  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

The *CryptographicKeys* XML element in the above table contains the
following XML element:

  XML element   Occurrences   Description
  ------------- ------------- ------------------------------------------------------------
  *Key*         0:n           Define a cryptographic key used in this technical profile.

Each *Key* XML element contains in turn the following attribute:

  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Attribute              Required   Description
  ---------------------- ---------- --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *Id*                   True       Specify a machine understandable identifier that is used to uniquely identify a particular key (pair), and reference it from other XML elements in the policy XML file.
                                    
                                    Type: String

  *StorageReferenceId*   True       Specify a machine understandable identifier that is used to uniquely identify a particular storage key container, and reference it from other XML elements in the policy XML file. For additional information, see section § *Managing your key containers for Trust Framework (policies)* in third part of this series of document.
                                    
                                    Type: String
  ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

The *InputTokenSources* XML elements contain the following XML element:

  XML element          Occurrences   Description
  -------------------- ------------- ------------------------------------------------------------------------------------------
  *TechnicalProfile*   1:n           Specify a source for that can be the input assertions for the current technical profile.

Each *TechnicalProfile* XML element contains the following attribute:

  ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Attribute   Required   Description
  ----------- ---------- -----------------------------------------------------------------------------------------------------------------------------------------------------
  *Id*        True       Specify a reference to a machine understandable identifier that is used to uniquely identify a particular technical profile in the policy XML file.
                         
                         Type: String
  ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Each *InputClaim* XML element contains the following attribute:

  ------------------------------------------------------------------------------------------------------------------------------------------------
  Attribute                Required   Description
  ------------------------ ---------- ------------------------------------------------------------------------------------------------------------
  *ClaimTypeReferenceId*   True       Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.
                                      
                                      Type: String
  ------------------------------------------------------------------------------------------------------------------------------------------------

The *InputClaimsTransformations* XML elements contain the following XML
element:

  XML element                   Occurrences   Description
  ----------------------------- ------------- -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *InputClaimsTransformation*   0:n           Specify a claims transformation that should be executed before any claims are sent to the claims provider or the relying party. A claims transformation can indeed be used to modify existing *ClaimsSchema* claims or generate new ones.

Each *InputClaimsTransformation* XML element contains the following
attribute:

  -----------------------------------------------------------------------------------------------------------------------------------------------------------
  Attribute       Required   Description
  --------------- ---------- --------------------------------------------------------------------------------------------------------------------------------
  *ReferenceId*   True       Specify a reference to a *ClaimsTransformation* already defined in the *ClaimsTransformations* section in the policy XML file.
                             
                             Type: String
  -----------------------------------------------------------------------------------------------------------------------------------------------------------

The *InputClaims* XML elements contain the following XML element:

  XML element    Occurrences   Description
  -------------- ------------- --------------------------------- --
  *InputClaim*   0:n           Specify an expected claim type.   

Each *InputClaim* XML element contains the following attribute:

  ------------------------------------------------------------------------------------------------------------------------------------------------
  Attribute                Required   Description
  ------------------------ ---------- ------------------------------------------------------------------------------------------------------------
  *ClaimTypeReferenceId*   True       Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.
                                      
                                      Type: String
  ------------------------------------------------------------------------------------------------------------------------------------------------

the *PersistedClaims* XML elements contain the following XML element:

  XML element        Occurrences   Description
  ------------------ ------------- -------------------------------------------------------------------------------------------------------------------------------------
  *PersistedClaim*   0:n           Specify an expected claim type. Claim mappings are used to determine the provider claim type before sending to the claims provider.

Each *PersistedClaim* XML element contains the following attributes:

  -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Attribute                Required   Description
  ------------------------ ---------- -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *ClaimTypeReferenceId*   True       Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.
                                      
                                      Type: String

  *DefaultValue *          False      Specify a default value to create a claim if the claim indicated by *ClaimTypeReferenceId* does not exist so that the resulting claim can be used as an *InputClaim* by the technical profile.
                                      
                                      Type: String

  *PartnerClaimType*       False      Identify the *ClaimType* of the external partner that the specified policy claim type maps to. If the *PartnerClaimType* attribute is not specified, then the specified policy claim type is mapped to the partner claim type of the same name.
                                      
                                      Type: String

  *OverwriteIfExists*      False      Provides an optional property to the claims provider indicating whether the claim can be overwritten in the claims provider records if the claim provider supports overwriting.
                                      
                                      Type: true or false
  -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

Eventually, the *OutputClaims* XML elements contain the following XML
element:

  XML element     Occurrences   Description
  --------------- ------------- ---------------------------------
  *OutputClaim*   0:n           Specify an expected claim type.

Each *OutputClaim* XML element contains the following attributes:

  ------------------------------------------------------------------------------------------------------------------------------------------------
  Attribute                Required   Description
  ------------------------ ---------- ------------------------------------------------------------------------------------------------------------
  *ClaimTypeReferenceId*   True       Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.
                                      
                                      Type: String

  *DefaultValue *          False      Specify a default value if not set.
                                      
                                      Type: String

  *PartnerClaimType*       False      Specify the partner claim type.
                                      
                                      Type: String

  *Required*               False      Specify this claim is required.
                                      
                                      Type: String
  ------------------------------------------------------------------------------------------------------------------------------------------------

The *OutputClaimsTransformations* XML elements contain the following XML
element:

  XML element                    Occurrences   Description
  ------------------------------ ------------- -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *OutputClaimsTransformation*   0:n           Specify a claims transformation that should be executed after claims are received from the claims provider. A claims transformation can indeed be used to modify existing *ClaimsSchema* claims or generate new ones.

Each *OutputClaimsTransformation* XML element contains the following
attribute:

  -----------------------------------------------------------------------------------------------------------------------------------------------------------
  Attribute       Required   Description
  --------------- ---------- --------------------------------------------------------------------------------------------------------------------------------
  *ReferenceId*   True       Specify a reference to a *ClaimsTransformation* already defined in the *ClaimsTransformations* section in the policy XML file.
                             
                             Type: String
  -----------------------------------------------------------------------------------------------------------------------------------------------------------

The *ValidationTechnicalProfiles* XML element in the above table
contains the following XML element:

  XML element                    Occurrences   Description
  ------------------------------ ------------- ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *ValidationTechnicalProfile*   1:n           Define a to be used for validating some or all of the output claims of the referencing technical profile. Therefore, all the input claims of the referenced technical profile must appear in the output claims of the referencing technical profile.

*ValidationTechnicalProfile* and *IncludeTechnicalProfile* XML elements
contain in turn the following attributes:

  Attribute       Required   Description
  --------------- ---------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *ReferenceId*   True       Specify a reference to a machine understandable identifier that is used to uniquely identify a particular technical profile already defined in the policy XML file.

The *SubjectNamingInfo* XML element in the above table contains the
following attributes:

  -------------------------------------------------------------------------------------------------------------------------------------------
  Attribute           Required   Description
  ------------------- ---------- ------------------------------------------------------------------------------------------------------------
  *ClaimType*         True       Specify a reference to a *ClaimType* already defined in the *ClaimsSchema* section in the policy XML file.
                                 
                                 Type: String

  *NameQualifier*     False      Provide a description
                                 
                                 Type: String

  *SPNameQualifier*   False      Provide a description
                                 
                                 Type: String

  *Format*            False      Provide a description
                                 
                                 Type: String

  *SPProvidedID*      False      Provide a description
                                 
                                 Type: String
  -------------------------------------------------------------------------------------------------------------------------------------------

*IncludeTechnicalProfile* and *UseTechnicalProfileForSessionManagement*
XML elements in the above table contain the following attribute:

  Attribute       Required   Description
  --------------- ---------- ---------------------------------------------------------------------------------------------------------------------------------------------------------------------
  *ReferenceId*   True       Specify a reference to a machine understandable identifier that is used to uniquely identify a particular technical profile already defined in the policy XML file.

The following XML snippet illustrates how to define a technical profile
for a claim provider:

```xml
<?xml version="1.0" encoding="utf-8"?>
<TrustFrameworkPolicy xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"
xmlns="http://schemas.microsoft.com/online/cpim/schemas/2013/06" PolicySchemaVersion="0.3.0.0" …>
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

SAML 2.0 Claims Provider
------------------------

Azure AD B2C Premium provides a support for the SAML 2.0. This section
outlines the specifics of a technical profile for interacting with
claims provider supporting this standardized protocol.

SAML 2.0, as a token format and a protocol, is very popular in the
public sector with government agencies as well as with enterprises and
educational institutions.

SAML 2.0 is a suite of specifications and, as such, comprises a set of
normative and non-normative documents. SAML 2.0 essentially defines
XML-based **assertions** and **protocols**, **bindings**, and
**profiles**.

**A prior understanding or even better a knowledge of the underlying
concepts is necessary to appropriately define a technical profile in
this space.**

If you’re not familiar with all of these concepts, the critical aspects
of SAML 2.0 are covered in detail in the following four normative
documents:

1.  [Assertions and Protocols for
    the OASIS Security Assertion Markup Language
    (SAML) V2.0](http://docs.oasis-open.org/security/saml/v2.0/saml-core-2.0-os.pdf)[^1]
    (SAMLCore), the core specification.

2.  [Bindings for the OASIS
    Security Assertion Markup Language
    (SAML) V2.0](http://docs.oasis-open.org/security/saml/v2.0/saml-bindings-2.0-os.pdf)[^2]
    (SAMLBind), which maps the SAML messages onto the standard messaging
    or communication protocols.

3.  [Profiles for the OASIS
    Security Assertion Markup Language
    (SAML) V2.0](http://docs.oasis-open.org/security/saml/v2.0/saml-profiles-2.0-os.pdf)[^3]
    (SAMLProf), the use cases or the “*How-to*” in regards to the use of
    SAML in various situations.

4.  And [Conformance Requirements
    for the OASIS Security Assertion Markup Language
    (SAML) V2.0](http://docs.oasis-open.org/security/saml/v2.0/saml-conformance-2.0-os.pdf)[^4]
    (SAMLConform), the operational modes for the SAML 2.0
    implementations.

The term SAML Core, in relationship with the SAMLCore core
specification, refers to the general syntax and semantics of SAML
assertions (a.k.a. tokens) as well as the protocol used to request and
transmit those assertions from one system entity to another. Most of the
time, the SAML assertion you may have to consider will be the so-called
"bearer" assertion, a short-lived bearer token (i.e. without a proof of
possession). Such an assertion may include both an authentication
statement and an attribute statement.

A SAML 2.0 protocol describes how certain SAML elements (including
assertions) are packaged within SAML request and response elements, and
gives the processing rules that SAML entities like an IdP in our context
must follow when producing or consuming these elements. For the most
part, a SAML protocol is a simple request-response protocol. It is
important to keep in mind that a SAML protocol always refers to what is
transmitted, and not how (the latter is determined by the choice of
binding). In the context of this paper, the most interesting SAML
protocols are the Authentication Request Protocol, and the Artifact
Resolution Protocol,

A SAML 2.0 binding determines how SAML requests and responses map onto
standard messaging or communications protocols. In other words, it’s a
mapping of a SAML protocol message onto standard messaging formats
and/or communications protocols. SAML 2.0 completely separates the
binding concept from the underlying profile (see below).

The SAML 2.0 standard defines several bindings:

-   HTTP Redirect (GET) binding,

-   HTTP POST binding,

-   HTTP Artifact binding,

-   Etc.

A SAML 2.0 profile is a concrete manifestation of a defined use case or
the “*How-to*” using a particular combination of assertions, protocols,
and bindings, assertions. Indeed, it describes in detail how SAML 2.0
assertions, protocols, and bindings combine to support the considered
use case. These

The SAML 2.0 standard defines several profiles:

-   Web Browser SSO profile,

-   Artifact Resolution profile,

-   Enhanced Client or Proxy (ECP) profile,

-   Etc.

The most important one is certainly the web Browser SSO profile since
this is the primary SAML use case for web SSO and federation. One should
also note that these profiles support various possible deployment
models.

Based on the above short introduction on SAML 2.0, and the related
concepts and normative documents, let’s consider how to define a
suitable technical profile.

The *Name* attribute of the *Protocol* XML element has to be set to
**SAML2** as per *ProtocolName* enumeration in the Azure AD B2C Premium
XML schema.

The following metadata item keys must or may be present in the
*Metadata* XML element:

  --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Item key                            Required   Description
  ----------------------------------- ---------- ---------------------------------------------------------------------------------------------------------------------------------------------
  PartnerEntity                       True       Allow to specify a CDATA section (ignored by the XML parser). This section contains the SAML 2.0 metadata of the SAML 2.0 claims provider.

  IssuerUri                           False      Specify the Issuer Uri in the SAML 2.0 assertion.

  TreatUnsolicitedResponseAsRequest   False      Allow an unsolicited response to serve as an authentication request.
                                                 
                                                 Value: true or false. When set to true, this allows to send an assertion to an IDP by an IDP: IDP initiated SSO.

  WantsSignedRequests                 False      Indicate if you want signed request.
                                                 
                                                 Value: true or false. Set to false (i.e. turned that off) for testing when you don’t have the production keys from the testing environment.

  WantsSignedAssertions               False      Indicate if you want signed assertions.
                                                 
                                                 Value: true or false.

  RequestsSigned                      False      Value: true or false.

  WantsSignedResponses                False      Indicate if you want signed response.
                                                 
                                                 Value: true or false.

  ResponsesSigned                     False      Value: true or false.

  AssertionsEncrypted                 False      Specify that the assertion must be encrypted before being sent.
                                                 
                                                 Value: true or false.
  --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

The *Suppressions* XML element - IN SPECIAL INTERRUPT CASES - provides
the ability to unblock the situation, suppress certain things in the
authentication request.

The following metadata item keys may be present in the *Suppressions*
XML element:

  ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  Item key        Required   Description
  --------------- ---------- -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  AuthnRequest    False      Specify for the authentication request message a comma-separated list of optional attribute/element of the SAML message since some implementations don’t support all the optional attribute/element as per protocol and above specifications, what is considered optional.
                             
                             Values in the list: Consent, Destination, Issuer, Extensions, AssertionConsumerServiceUrl, AssertionConsumerServiceIndex, AttributeConsumingServiceIndex, ProtocolBinding, IsPassive, ForceAuthn, ProviderName, Scoping, RequestedAuthenticationcontext, Conditions, NameIDPolicy, and Subject.

  LogoutRequest   False      Specify for the logout request message a comma-separated list of optional attribute/element of the SAML message since some implementations don’t support all the optional attribute/element as per protocol and above specifications, what is considered optional.
                             
                             Values in the list: Consent, Destination, Issuer, Extensions, NotOnOrAfter, Reason, and SessionIndex.
  ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

**Note** This is very unusual to suppress them.

The *InputClaimsTransformations* XML element is absent.

The *InputClaims* XML element is empty or absent.

The *PersistedClaims* XML element is absent.

The *OutputClaims* XML element contains the claims bag as the output
with possible mapping information between a *ClaimType* already defined
in the *ClaimsSchema* section in the policy XML file and that partner
claim type. **This also enables to take the subject name of the SAML
assertion instead of as an attribute**:

&lt;OutputClaim ClaimTypeReferenceId="UserId"
PartnerClaimType="assertionSubjectName" /&gt;

The *OutputClaimsTransformations* XML element may contain a collection
of *OutputClaimsTransformation* to be used to check or modify the output
claims, or generate new ones.

The following XML snippet illustrates a technical profile for a SAML 2.0
claims provider:

```xml
<TechnicalProfile Id="IdP-SAML2-Outbound">
    <DisplayName>Foo IdP</DisplayName>
    <Description>Some suitable description for foo
IdP</Description>
    <Protocol Name="SAML2" />
    <Metadata>
        <Item Key="WantsSignedAssertions">false</Item>
        <Item Key="TreatUnsolicitedResponseAsRequest">true</Item>
        <Item Key="ResponsesSigned">false</Item>
        <Item Key="IssuerUri">https://te.cpim.windows.net/csdii.onmicrosoft.com/B2C\_1A\_casinitiated</Item>
        <Item Key="PartnerEntity"><!\[CDATA\[<md:EntityDescriptor entityID="https://web.dev1.foo.com/saml20" xmlns:md="urn:oasis:names:tc:SAML:2.0:metadata"><md:IDPSSODescriptor WantAuthnRequestsSigned="true" protocolSupportEnumeration="urn:oasis:names:tc:SAML:2.0:protocol"><md:KeyDescriptor use="signing"><KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig\#"><X509Data><X509Certificate>MIIF1DCCA7ygAwIBAgIHC4Dr2wGTODMA…15lH8uDLHNp/ctEQ=</X509Certificate></X509Data></KeyInfo></md:KeyDescriptor><md:KeyDescriptor use="encryption"><KeyInfo xmlns="http://www.w3.org/2000/09/xmldsig\#"><X509Data><X509Certificate>MIIF0jCCA7qgAwIBAgIHC4D4+…P4V2neBrOzAQWFo</X509Certificate></X509Data></KeyInfo><md:EncryptionMethod Algorithm="http://www.w3.org/2001/04/xmlenc\#rsa-1\_5"/></md:KeyDescriptor><md:ArtifactResolutionService Binding="urn:oasis:names:tc:SAML:2.0:bindings:SOAP" Location="https://web.dev1.foo.com/saml20/soap" index="0" isDefault="true"/><md:SingleLogoutService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact" Location="https://web.dev1.foo.com/saml20/slo"/><md:SingleLogoutService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST" Location="https://web.dev1.foo.com/saml20/slo"/><md:SingleLogoutService Binding="urn:oasis:names:tc:SAML:2.0:bindings:SOAP" Location="https://web.dev1.foo.com/saml20/soap"/><md:NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:persistent</md:NameIDFormat><md:NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:transient</md:NameIDFormat><md:NameIDFormat>urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress</md:NameIDFormat><md:NameIDFormat>urn:oasis:names:tc:SAML:2.0:nameid-format:encrypted</md:NameIDFormat><md:NameIDFormat>urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified</md:NameIDFormat><md:SingleSignOnService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Artifact" Location="https://web.dev1.foo.com/saml20/login"/><md:SingleSignOnService Binding="urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST" Location="https://web.dev1.foo.com/saml20/login"/></md:IDPSSODescriptor><md:Organization><md:OrganizationName xml:lang="en">Foo Inc.</md:OrganizationName><md:OrganizationDisplayName xml:lang="en">Foo Inc.</md:OrganizationDisplayName><md:OrganizationURL xml:lang="en"/></md:Organization><md:ContactPerson contactType="technical"><md:Company>Foo Inc.</md:Company><md:GivenName>John</md:GivenName><md:SurName>Doe</md:SurName><md:EmailAddress>john.doe@foo.com</md:EmailAddress><md:TelephoneNumber>4254166431</md:TelephoneNumber></md:ContactPerson></md:EntityDescriptor>\]\]></Item>
    </Metadata>
    <InputClaims />
    <OutputClaims>
        <OutputClaim ClaimTypeReferenceId="AccessLevel" PartnerClaimType="accessLevel" />
        <OutputClaim ClaimTypeReferenceId="AddressCity" PartnerClaimType="city" />
        <OutputClaim ClaimTypeReferenceId="GivenName" PartnerClaimType="cn" />
        <OutputClaim ClaimTypeReferenceId="Email" PartnerClaimType="email" />
        <OutputClaim ClaimTypeReferenceId="Gender" PartnerClaimType="gender" />
        <OutputClaim ClaimTypeReferenceId="MiddleName" PartnerClaimType="middleName" />
        <OutputClaim ClaimTypeReferenceId="AddressZip" PartnerClaimType="postalCode" />
        <OutputClaim ClaimTypeReferenceId="SurName" PartnerClaimType="sn" />
        <OutputClaim ClaimTypeReferenceId="AddressState" PartnerClaimType="stateCode" />
        <OutputClaim ClaimTypeReferenceId="AddressLine2" PartnerClaimType="streetAddressLine2" />
        <OutputClaim ClaimTypeReferenceId="AddressLine1" PartnerClaimType="streetAddressLine1" />
        <OutputClaim ClaimTypeReferenceId="UserId" PartnerClaimType="assertionSubjectName" /> 
</OutputClaims>
</TechnicalProfile>
```
