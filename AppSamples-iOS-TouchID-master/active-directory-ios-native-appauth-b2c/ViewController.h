//
//  ViewController.h
//  active-directory-ios-native-appauth
//
//  Created by Saeed Akhter and Gerardo Saca on 3/1/17.
//  Copyright © 2017 Microsoft. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "Games.h"
#import "BKPasscodeViewController.h"

@interface ViewController : UIViewController <BKPasscodeViewControllerDelegate>{
    
    Games* allGames;
    NSString* saveToken;
    
}


@end

