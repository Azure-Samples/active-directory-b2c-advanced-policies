//
//  Games.m
//  active-directory-ios-native-appauth-b2c
//
//  Created by John Lyons on 10/4/17.
//  Copyright © 2017 Microsoft. All rights reserved.
//

#import "Games.h"

@implementation Games
static Games *_sharedInstance;
@synthesize gamesList;

- (id) init
{
    if (self = [super init])
    {
        
    }
    return self;
}
+ (Games *) sharedInstance
{
    if (!_sharedInstance)
    {
        _sharedInstance = [[Games alloc] init];
    }
    
    return _sharedInstance;
}


-(void)loadGames:(NSString*)token{
    
    //[[UIApplication sharedApplication] setNetworkActivityIndicatorVisible:YES];
    
   // NSString *token ; //GET THE TOKEN FROM THE KEYCHAIN
    
    NSString *authValue = [NSString stringWithFormat:@"Bearer %@",token];
    
    //Configure your session with common header fields like authorization etc
    NSURLSessionConfiguration *sessionConfiguration = [NSURLSessionConfiguration defaultSessionConfiguration];
    sessionConfiguration.HTTPAdditionalHeaders = @{@"Authorization": authValue};
    
    NSURLSession *session = [NSURLSession sessionWithConfiguration:sessionConfiguration];
    
    NSString *url=@"https://wingtipgamesb2c.azurewebsites.net/api/games/newrelease";
    NSURLRequest *request = [NSURLRequest requestWithURL:[NSURL URLWithString:url]];
    //NSLog(@"url %@",url);
    
    NSURLSessionDataTask *task = [session dataTaskWithRequest:request completionHandler:^(NSData *data, NSURLResponse *response, NSError *error) {
        [[UIApplication sharedApplication] setNetworkActivityIndicatorVisible:NO];
        if (!error) {
            NSHTTPURLResponse *httpResponse = (NSHTTPURLResponse *)response;
            if (httpResponse.statusCode == 200){
                NSArray *jsonData = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingMutableContainers|NSJSONReadingAllowFragments error:nil];
                
                //Process the data
                
                //NSLog(@"jsonData %@",jsonData);
                
                gamesList=jsonData;
            }
        }
        
    }];
    [task resume];
    
    
}

@end
