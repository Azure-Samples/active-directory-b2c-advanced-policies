//
//  Games.h
//  active-directory-ios-native-appauth-b2c
//
//  Created by John Lyons on 10/4/17.
//  Copyright © 2017 Microsoft. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>


@interface Games : NSObject{
    
    NSArray* gamesList;
    
    
}
@property (readwrite, retain) NSArray* gamesList;
+ (Games *) sharedInstance;

-(void)loadGames:(NSString*)token;

@end
