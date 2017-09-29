This sample shows *one way* to provide a Password Change user journey (policy)

Everything is configured in TrustFrameworkExtensions.xml & ChangePassword.xml

It was checked with a with signed in account and non-sign in account. You can test it by yourself https://contosoaadb2cchangepassword.azurewebsites.net 

How it works?
•	New user journey ChangePassword invokes LocalAccountWritePasswordChangeUsingObjectId technical profile
•	LocalAccountWritePasswordChangeUsingObjectId is based on password reset, but:
o	Presents currentPassword claim
o	Calls login-NonInteractive2  validation profile to validate the user credentials 
•	login-NonInteractive2 is based on login-NonInteractiv, but sends the currentPassword claim instead of Password claim
•	New claim currentPassword is defined 


