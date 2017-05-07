//
//  GameTableViewController.h
//  active-directory-ios-native-appauth-b2c
//
//  Created by John Lyons on 10/4/17.
//  Copyright © 2017 Microsoft. All rights reserved.
//

#import <UIKit/UIKit.h>

#import "Games.h"
@interface GameTableViewController : UIViewController<UITableViewDelegate,UITableViewDataSource>{
    
    UITableView* myTableView;
    Games* allGames;
}

@end
