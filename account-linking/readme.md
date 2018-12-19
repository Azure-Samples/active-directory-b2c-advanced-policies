
This folder contains a complete starterpack to enable account linking.  The key pieces are defined and identified below (e.g. user journey, technical profiles, and claims transformations)



Enabling end users to sign up or sign in to your applications by using existing digital identities t (e.g. login with a google account) often provides a faster onboarding experience and greater user satisfaction as no additional passwords are needed.  When more than one identity provider option is offered, it may be desired that the user be able to login with different options to the same end user account.  Two or more identity provider options accessing one account is called account linking.  
Note that “social” accounts (e.g. facebook) and “work” accounts (e.g. Azure AD, or a SAML Login) are treated the same in Azure AD B2C-these are federated accounts. “Local accounts” are authenticated directly in b2c and are created with a userid and password that may be unique to the b2c tenant.




Pre-requisites
Linking (and unlinking) accounts is supported via Custom policies in Azure AD B2C.  Read and complete the Get started with custom policies in Azure AD B2C
Review Azure AD B2C: Migrate users with social identities to gain an understanding of how consumer (b2c) accounts are stored in an Azure AD B2C tenant.

User attributes for local and social account users
The immutable identifier of a user account in Azure AD B2C is the objectId
, a user account that can login with both local and social account identities will have attributes for both options. Local account sign-in names, and social account identities are stored in different attributes. signInNames is used for local account, while userIdentities for social account. A single Azure AD B2C account, can be a local account only, social account only, or combine a local account with social identity in one user record. This behavior allows you to manage a single account, while a user can sign in with the local account credential(s) or with the social identities.

How it works
Account linking is achieved by adding an additional userIdentity to the userIdentities collection in the target user account (objectId).  You may add a userIdenitity to a user with none (e.g. a local account user) or to user who may already have one or more userIndentities.  Conversely, account unlinking consists of removing unwanted userIdentities from an accounts userIdentities collection,  While this may be achieved directly by an application calling Graph, it is usually in the context of a live session, a user journey, that a user can self-serve. 

The building blocks

A.	The End user experiences:
Account linking by  adding a social account. User can choose from among potential accounts by selecting a social account when there are more than one. Authentication of the account that will be added is required.  A “local account” cannot be added.
Account Unlink by removing one social account selected by user. Authentication of the account that will be removed is not required.  The last social account in an account with no other methods of login (e.g. no signinName) cannot be removed.

The SignInandLinkAccount UserJourney takes the end user through the steps of authenticating(local or social), then selecting an ID provider for linking, authenticating to that Id Provider, and then adding that ID provider to the user object if it is not already there.  The option to link or unlink is provided in the same page so users can either add new or remove existing ID providers.

B.	Technical Profiles
For every desired identity provider, there will be four technical profiles.  Microsoft Account (MSA) is described below for example
1.	MSA-OIDC-Base – Contains the metadata required for all interaction with the IDP
2.	“MSA-OIDC” – this is the technical profile used to complete the authentication with the ID provider.  Upon successful auth by the user, it gathers the claims identified in the technical profile
3.	MSA-OIDC-Link – Authenticates and adds the MSA issuer and issuer Id to the collection
4.	MSA-OIDC-Unlink-Authenticates and removes the MSA issuer and issuer Id(s) from the userIdentities collection


Claims Transformations

|    Transformation    |    Method    |    Used For    |
|------------------------------------------------|----------------------------------------------------------|--------------------------------------------------------------------|
|    CreateUserIdentity    |    CreateUserIdentity    |    Create   a userIdentity pair (issuer, issuerID)    |
|    CreateUserIdentityToLink    |    CreateUserIdentity    |    Create   the userIdentity pair that will be added    |
|    AppendUserIdentity    |    AddItemToUserIdentityCollection    |    Add   a userIdentity to an existing Identity Collection    |
|    AppendUserIdentitytoLink    |    AddItemToUserIdentityCollection    |    Add   the new user Identity to an existing collection    |
|    RemoveUserIdentityFromCollectionByIssuer    |    RemoveUserIdentityFromCollectionByIssuer    |    Remove   all the userIdentities of a user for a given issuer    |
|    ExtractIssuers    |    GetIssuersFRomUserIdentityCollectionTransformation    |    Get   the issuers from a userIdentity Collection    |


