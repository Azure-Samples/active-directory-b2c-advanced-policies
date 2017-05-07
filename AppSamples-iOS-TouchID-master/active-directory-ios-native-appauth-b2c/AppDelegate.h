//
//  AppDelegate.h
//  active-directory-ios-native-appauth
//
//  Created by Saeed Akhter and Gerardo Saca on 3/1/17.
//  Copyright © 2017 Microsoft. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "AppAuth.h"

@interface AppDelegate : UIResponder <UIApplicationDelegate>

@property (nonnull, strong, nonatomic) UIWindow *window;
@property (nonatomic, strong, nullable) id<OIDAuthorizationFlowSession> currentAuthorizationFlow;


@end

