This sample shows how to create a user journey which does *only* signin as opposed to the starterpack sample which provides both signup and signin options in the same experience.

A SignUp or SignIn journey from the starterpack can be edited to achieve this scenario:



Notes and changes:
Policy
    <UserJourney Id="SignUpOrSignIn">
      <OrchestrationSteps>
      
<!--NOTE: remove <OrchestrationStep Order="1" Type="CombinedSignInAndSignUp" ContentDefinitionReferenceId="api.signuporsignin">-->
               <!-- Yoel: add following line instead -->
               <OrchestrationStep Order="1" Type="ClaimsProviderSelection" ContentDefinitionReferenceId="api.idpselections">
           <ClaimsProviderSelections>
            <ClaimsProviderSelection TargetClaimsExchangeId="FacebookExchange" />

            <!--NOTE: remove <ClaimsProviderSelection ValidationClaimsExchangeId="LocalAccountSigninEmailExchange" /> -->
           <!-- NOTE: add following line instead -->
     <ClaimsProviderSelection TargetClaimsExchangeId="LocalAccountSigninEmailExchange" />
          </ClaimsProviderSelections>

          <!--NOTE: remove <ClaimsExchanges>
            <ClaimsExchange Id="LocalAccountSigninEmailExchange" TechnicalProfileReferenceId="SelfAsserted-LocalAccountSignin-Email" />
          </ClaimsExchanges>-->
        </OrchestrationStep>
        
       <!-- Check if the user has selected to sign in using one of the social providers -->
        <OrchestrationStep Order="2" Type="ClaimsExchange">
          <Preconditions>
            <Precondition Type="ClaimsExist" ExecuteActionsIf="true">
              <Value>objectId</Value>
              <Action>SkipThisOrchestrationStep</Action>
            </Precondition>
          </Preconditions>
          <ClaimsExchanges>
            <ClaimsExchange Id="FacebookExchange" TechnicalProfileReferenceId="Facebook-OAUTH" />
<!-- NOTE: remove <ClaimsExchange Id="SignUpWithLogonEmailExchange" TechnicalProfileReferenceId="LocalAccountSignUpWithLogonEmail" />-->
              <!-- NOTE: add following line instead -->
               <ClaimsExchange Id="LocalAccountSigninEmailExchange" TechnicalProfileReferenceId="SelfAsserted-LocalAccountSignin-Email" />
          </ClaimsExchanges>
        </OrchestrationStep>
