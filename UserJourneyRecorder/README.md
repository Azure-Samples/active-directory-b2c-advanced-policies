Azure AD B2C can log activity data into Azure Application Insights for the purposes of troubleshooting during development.  
https://github.com/MicrosoftDocs/azure-docs/blob/master/articles/active-directory-b2c/active-directory-b2c-troubleshoot-custom.md

The article added here (.docx) also explains how to use a sample to deploy a simple web service that will enhance the visualization of the troubleshooting output, it is called the userjourney recorder.

The userjourney recorder version in this folder is the generic sample.

A different version of the userjourney recorder, adapted to read B2C logs in Application Insights, can be found here as part of the wingtiptoys project. See article for details:

https://github.com/Azure-Samples/active-directory-b2c-advanced-policies/tree/master/wingtipgamesb2c/src/WingTipUserJourneyPlayerWebApplication
