Scenarios
=========

This document will walk you through adding Azure AD as an Identity
provider to Azure AD B2C.

There are different ways to setup Azure AD as an IdP, each of which has
its own slight variations. The primary intended scenarios for Azure AD
as an IdP are the following:

-  Contoso wants to build an app, *Contoso Rewards*, for their
    customers, who are consumers, to keep track of their rewards points
    obtained by purchasing Contoso products in retail stores. Contoso
    also wants to allow Contoso employees to sign in to the same app
    using their corporate credentials to create and manage activities
    that customers can participate in to get more rewards.

    -   Use Azure AD B2C

    -   Local accounts + social IdPs for consumers

    -   Azure AD IdP for Contoso’s Azure AD

        -   Azure AD v1 app registration – Single tenant

-  Fabrikam Software wants to build an app, *Fabrikam Collab* that it
    wants to sell to both individuals and businesses. This app helps its
    users store documents and share them with other users. Users of the
    app should be able to sign in with local and social accounts as well
    as, after paying for enterprise SSO, sign in with their Azure AD
    backed corporate credentials.

    -   Use Azure AD B2C

    -   Local accounts + social IdPs for consumers

    -   Multiple Azure AD IdPs for each of Fabrikam’s business customers
        with Azure AD

        -   Azure AD v1 – Single tenant or Multi-tenant – Details on
            when to use which TBD

The following configurations/scenarios, while outlined in this document,
aren’t encouraged:

-   Using Azure AD v2 apps – REASONS TBD

This walkthrough will only focus on the Azure AD IdP piece of these
scenarios.

Walkthrough
===========

Setup
-----

Make sure you first complete the Basic Setup tutorial.

Register the App in Azure AD
----------------------------

### Azure AD v1 \*RECOMMENDED\*

1.  Go to the Azure portal (<https://portal.azure.com>)

2.  Open the App Registrations Blade

3.  PENDING

### Azure AD v2

1.  Go to the Application Registration portal
    (<https://identity.microsoft.com>)

2.  PENDING

Create the Azure AD Claims Provider
-----------------------------------

First go through the “Create the OIDC Claims Provider” section in the
generic OIDC Idp tutorial, but skip the “Configure Metadata” step,
you’ll be configuring that differently depending of what kind of setup
do you want. See the sections below for more details on each setup.

### Azure AD v1 – Single tenant \*RECOMMENDED\*

1.  Configure Metadata section as follows

    1.  METADATA -
        https://login.windows.net/&lt;tenantname&gt;/.well-known/openid-configuration

    2.  authorization\_endpoint -
        https://login.windows.net/common/oauth2/authorize

    3.  ProviderName - https://sts.windows.net/&lt;tenantid&gt;/

        1.  You can retrieve the tenant ID by navigating to the Metadata
            URL from (a) above and retrieving the value from the
            “issuer”

    4.  client\_id – Client ID from the app registration

    5.  IdTokenAudience – same as client\_id

    6.  scope - openid

    7.  HttpBinding - POST

    8.  response\_types – id\_token

### Azure AD v2 – Single tenant

1.  Configure Metadata section

    1.  METADATA -
        https://login.windows.net/&lt;tenantname&gt;/v2.0/.well-known/openid-configuration

    2.  authorization\_endpoint -
        https://login.windows.net/common/oauth2/v2.0/authorize

    3.  ProviderName -
        https://login.microsoftonline.com/&lt;tenantid&gt;/v2.0

        1.  You can retrieve the tenant ID by navigating to the Metadata
            URL from (a) above and retrieving the value from the
            “issuer”

    4.  client\_id - Client ID from the app registration

    5.  IdTokenAudience – same as client\_id

    6.  scope - openid

    7.  HttpBinding - POST

    8.  response\_types – id\_token

### Azure AD v1 – Multi tenant

1.  Configure Metadata section as follows

    1.  METADATA - \*remove\*

    2.  DiscoverMetadataByTokenIssuer - true

    3.  ValidTokenIssuerPrefixes - https://sts.windows.net/

    4.  authorization\_endpoint -
        https://login.windows.net/common/oauth2/authorize

    5.  ProviderName - \*remove\*

    6.  client\_id – Client ID from the app registration

    7.  IdTokenAudience – same as client\_id

    8.  scope - openid

    9.  HttpBinding - POST

    10.  response\_types – id\_token

### Azure AD v2 – Multi tenant

1.  Configure Metadata section

    1.  METADATA - \*remove\*

    2.  DiscoverMetadataByTokenIssuer – true

    3.  ValidTokenIssuerPrefixes- https://login.microsoftonline.com/

    4.  authorization\_endpoint -
        https://login.windows.net/common/oauth2/v2.0/authorize

    5.  ProviderName - \*remove\*

    6.  client\_id - Client ID from the app registration

    7.  IdTokenAudience – same as client\_id

    8.  scope - openid

    9.  HttpBinding - POST

    10.  response\_types – id\_token

Add the Azure AD Claims Provider to User Journey(s)
---------------------------------------------------

Configure this as per the “Add the OIDC Claims Provider to User
Journey(s)” section in the generic OIDC Idp tutorial.          
