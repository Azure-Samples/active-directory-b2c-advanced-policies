//
//  ViewController.m
//  active-directory-ios-native-appauth
//
//  Created by Saeed Akhter and Gerardo Saca on 3/1/17.
//  Copyright © 2017 Microsoft. All rights reserved.
//

#import "ViewController.h"
#import "AppAuth.h"
#import "AppDelegate.h"
#import "GameTableViewController.h"

// Update the following for your AAD B2C tenant
static NSString *const kTenantName = @"b2ctechready.onmicrosoft.com";
static NSString *const kSignupOrSigninPolicy = @"b2c_1a_sign_up_sign_in_games";
static NSString *const kEditProfilePolicy = @"b2c_1_edit_profile";
static NSString *const kClientId = @"ac08f359-c273-4c01-9a43-3c47ec2f142b";
static NSString *const kRedirectUri = @"com.onmicrosoft.b2ctechready.wingtipgamesb2c://oauth/redirect";


// DO NOT CHANGE - This is the format of OIDC Token and Authorization enpoints for AAD B2C
static NSString *const kEndpoint = @"https://login.microsoftonline.com/te/%1$@/%2$@/oauth2/v2.0/%3$@";

@interface ViewController ()
@property (nonatomic, strong, nullable) OIDAuthState *authState;
@property (weak, nonatomic) IBOutlet UIButton *signInButton;
@property (weak, nonatomic) IBOutlet UIButton *editProfileButton;
@property (weak, nonatomic) IBOutlet UIActivityIndicatorView *spinnerView;
@property (weak, nonatomic) IBOutlet UILabel *MessLabel;

@end

@implementation ViewController
- (IBAction)didSignIn:(id)sender {
    
    NSLog(@"Signing in");
    
    [_spinnerView startAnimating];
    _spinnerView.hidden=false;
    _signInButton.enabled = NO;
    _editProfileButton.enabled = NO;
    
    NSURL *authorizationEndpoint = [NSURL URLWithString:[NSString stringWithFormat:kEndpoint, kTenantName, kSignupOrSigninPolicy, @"authorize"]];
    NSURL *tokenEndpoint = [NSURL URLWithString:[NSString stringWithFormat:kEndpoint, kTenantName, kSignupOrSigninPolicy, @"token"]];
    
  //  NSLog(@"Authorize endpoint: %@", authorizationEndpoint);
  //  NSLog(@"Token endpoint: %@", tokenEndpoint);
    
    OIDServiceConfiguration *configuration = [[OIDServiceConfiguration alloc] initWithAuthorizationEndpoint:authorizationEndpoint tokenEndpoint:tokenEndpoint];
    
    OIDAuthorizationRequest *request = [[OIDAuthorizationRequest alloc] initWithConfiguration:configuration clientId:kClientId scopes:@[OIDScopeOpenID, OIDScopeProfile,@"https://b2ctechready.onmicrosoft.com/wingtipb2c/Games.Read",@"offline_access"] redirectURL:[NSURL URLWithString:kRedirectUri] responseType:@"code" additionalParameters:nil];
    
    AppDelegate *appDelegate = (AppDelegate *)[UIApplication sharedApplication].delegate;
    
    
    appDelegate.currentAuthorizationFlow = [OIDAuthState authStateByPresentingAuthorizationRequest:request presentingViewController:self callback:^(OIDAuthState *_Nullable authState, NSError *_Nullable error) {
        
        if (authState) {
            
            [[NSUserDefaults standardUserDefaults] setObject:authState.lastTokenResponse.idToken forKey:@"idToken"];
            [[NSUserDefaults standardUserDefaults] setObject:authState.lastTokenResponse.accessToken forKey:@"accessToken"];
            [[NSUserDefaults standardUserDefaults] setObject:authState.lastTokenResponse.refreshToken forKey:@"refreshToken"];
            
            
            
            [self setAuthState:authState];
            
            [self ShowGames:authState.lastTokenResponse.accessToken];
            
        } else {
            NSLog(@"Authorization error: %@", [error localizedDescription]);
            [self setAuthState:nil];
        }
    }];
}
-(void)ShowGames:(NSString*)accessToken{
    
    NSString* aPasscode=[[NSUserDefaults standardUserDefaults] objectForKey:@"aPasscode"];
    
    
    if (aPasscode==nil){
        saveToken=accessToken;
        
        BKPasscodeViewController *viewController = [[BKPasscodeViewController alloc] initWithNibName:nil bundle:nil];
        viewController.delegate = self;
        viewController.type = BKPasscodeViewControllerNewPasscodeType;
        
        viewController.passcodeStyle = BKPasscodeInputViewNumericPasscodeStyle;
        
        // To supports Touch ID feature, set BKTouchIDManager instance to view controller.
        // It only supports iOS 8 or greater.
        viewController.touchIDManager = [[BKTouchIDManager alloc] initWithKeychainServiceName:@"<# your keychain service name #>"];
        viewController.touchIDManager.promptText = @"Scan fingerprint to authenticate";   // You can set prompt text.
        
        UINavigationController *navController = [[UINavigationController alloc] initWithRootViewController:viewController];
        [self presentViewController:navController animated:YES completion:nil];
        
    } else {
    
        _editProfileButton.enabled = YES;
        
        allGames=[Games sharedInstance];
        
        [allGames loadGames:accessToken];
        
        
        [NSTimer scheduledTimerWithTimeInterval:0.1 target:self selector:@selector(checkTimer) userInfo:nil repeats:NO];
        
    }
    
    
}

-(void)checkTimer{
    if (allGames.gamesList.count==0){
        [NSTimer scheduledTimerWithTimeInterval:0.1 target:self selector:@selector(checkTimer) userInfo:nil repeats:NO];
    } else {
        
        GameTableViewController    *targetViewController = [[GameTableViewController alloc] init];
        [[self navigationController] pushViewController:targetViewController animated:YES];
        
        [_spinnerView stopAnimating];
    }
}
- (IBAction)didEditProfile:(id)sender {
    
    
    NSURL *authorizationEndpoint = [NSURL URLWithString:[NSString stringWithFormat:kEndpoint, kTenantName, kEditProfilePolicy, @"authorize"]];
    NSURL *tokenEndpoint = [NSURL URLWithString:[NSString stringWithFormat:kEndpoint, kTenantName, kEditProfilePolicy, @"token"]];
    
    
    OIDServiceConfiguration *configuration = [[OIDServiceConfiguration alloc] initWithAuthorizationEndpoint:authorizationEndpoint tokenEndpoint:tokenEndpoint];
    
    OIDAuthorizationRequest *request = [[OIDAuthorizationRequest alloc] initWithConfiguration:configuration clientId:kClientId scopes:@[OIDScopeOpenID, OIDScopeProfile] redirectURL:[NSURL URLWithString:kRedirectUri] responseType:OIDResponseTypeCode additionalParameters:nil];
    
    
    AppDelegate *appDelegate = (AppDelegate *)[UIApplication sharedApplication].delegate;
    appDelegate.currentAuthorizationFlow = [OIDAuthState authStateByPresentingAuthorizationRequest:request presentingViewController:self callback:^(OIDAuthState *_Nullable authState, NSError *_Nullable error) {
        
        if (authState) {
           // NSLog(@"Got authorization tokens. Access token: %@", authState.lastTokenResponse.accessToken);
            [self setAuthState:authState];
        } else {
            NSLog(@"Authorization error: %@", [error localizedDescription]);
            [self setAuthState:nil];
        }
    }];
}

- (void)viewDidLoad {
    [_spinnerView stopAnimating];
    _spinnerView.hidden=true;
    [super viewDidLoad];
    _editProfileButton.enabled = NO;
    
    
    NSString* aPasscode=[[NSUserDefaults standardUserDefaults] objectForKey:@"aPasscode"];
    
    if (aPasscode==nil){
        
    } else {
        
        BKPasscodeViewController *viewController = [[BKPasscodeViewController alloc] initWithNibName:nil bundle:nil];
        viewController.delegate = self;
        viewController.type = BKPasscodeViewControllerCheckPasscodeType;   // for authentication
        viewController.passcodeStyle = BKPasscodeInputViewNumericPasscodeStyle;
        
        // To supports Touch ID feature, set BKTouchIDManager instance to view controller.
        // It only supports iOS 8 or greater.
        viewController.touchIDManager = [[BKTouchIDManager alloc] initWithKeychainServiceName:@"<# your keychain service name #>"];
        viewController.touchIDManager.promptText = @"Scan fingerprint to authenticate";   // You can set prompt text.
        
        // Show Touch ID user interface
        [viewController startTouchIDAuthenticationIfPossible:^(BOOL prompted) {
            
            // If Touch ID is unavailable or disabled, present passcode view controller for manual input.
            if (NO == prompted) {
                UINavigationController *navController = [[UINavigationController alloc] initWithRootViewController:viewController];
                [self presentViewController:navController animated:YES completion:nil];
            }
        }];
    }
    
}


- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

- (void)passcodeViewController:(BKPasscodeViewController *)aViewController didFinishWithPasscode:(NSString *)aPasscode{
    NSLog(@"aPasscode %@",aPasscode);
    
    [[NSUserDefaults standardUserDefaults] setObject:aPasscode forKey:@"aPasscode"];
    
    [aViewController dismissViewControllerAnimated:YES completion:nil];
    
    [self ShowGames:saveToken];
    
}


- (void)passcodeViewControllerDidFailAttempt:(BKPasscodeViewController *)aViewController{
    
    NSLog(@"passcodeViewControllerDidFailAttempt");
    [aViewController dismissViewControllerAnimated:YES completion:nil];
    
    _MessLabel.text=@"login error";
    
    _signInButton.enabled = NO;
    _editProfileButton.enabled = NO;
}

- (void)passcodeViewController:(BKPasscodeViewController *)aViewController authenticatePasscode:(NSString *)aPasscode resultHandler:(void(^)(BOOL succeed))aResultHandler{
    
    
    _MessLabel.text=@"loading games";
    
    _signInButton.hidden = true;
    _editProfileButton.hidden = true;
    
    
    [_spinnerView startAnimating];
    _spinnerView.hidden=false;
    
    
    NSString* refreshToken=[[NSUserDefaults standardUserDefaults] objectForKey:@"refreshToken"];
    NSLog(@"authenticatePasscode %@",refreshToken);
    if (refreshToken!=nil){
        
        NSURL *authorizationEndpoint = [NSURL URLWithString:[NSString stringWithFormat:kEndpoint, kTenantName, kSignupOrSigninPolicy, @"authorize"]];
        NSURL *tokenEndpoint = [NSURL URLWithString:[NSString stringWithFormat:kEndpoint, kTenantName, kSignupOrSigninPolicy, @"token"]];
        
        OIDServiceConfiguration *configuration = [[OIDServiceConfiguration alloc] initWithAuthorizationEndpoint:authorizationEndpoint tokenEndpoint:tokenEndpoint];
        
        
        OIDTokenRequest* newTokenRequest=[[OIDTokenRequest alloc] initWithConfiguration:configuration grantType:@"refresh_token" authorizationCode:nil redirectURL:[NSURL URLWithString:kRedirectUri] clientID:kClientId clientSecret:nil scope:@"openid profile https://b2ctechready.onmicrosoft.com/wingtipb2c/Games.Read offline_access" refreshToken:refreshToken codeVerifier:nil additionalParameters:nil];
        
        
        [OIDAuthorizationService  performTokenRequest:newTokenRequest  callback:^(OIDTokenResponse *_Nullable tokenResponse,  NSError *_Nullable tokenError) {
             
             if (tokenError){
                 NSLog(@"tokenError %@",tokenError);
             } else {
                
                 
                 [self ShowGames:tokenResponse.accessToken];
             }
            

         }];
        
        
        
    } else {
        
        [aViewController dismissViewControllerAnimated:YES completion:nil];
    }
    
    
}

@end
