//
//  GameTableViewController.m
//  active-directory-ios-native-appauth-b2c
//
//  Created by John Lyons on 10/4/17.
//  Copyright © 2017 Microsoft. All rights reserved.
//

#import "GameTableViewController.h"

@interface GameTableViewController ()

@end

@implementation GameTableViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    
    CGRect r=[[UIScreen mainScreen] applicationFrame];
    
    
    UIView* contentView=[[UIView alloc] initWithFrame:r];
    contentView.backgroundColor=[UIColor whiteColor];
    self.view=contentView;
    
    myTableView=[[UITableView alloc] initWithFrame:r style:UITableViewStylePlain];
    myTableView.separatorStyle=UITableViewCellSeparatorStyleNone;
    myTableView.delegate=self;
    myTableView.dataSource=self;
    [self.view addSubview:myTableView];
    
   
    UIView* YourHeaderView=[[UIView alloc] initWithFrame:CGRectMake(0, 0, r.size.width, 40)];
    YourHeaderView.backgroundColor=[UIColor whiteColor];
    [self.view addSubview:YourHeaderView ];
    
    
    
    allGames=[Games sharedInstance];
    
}
-(void)checkTimer{
    if (allGames.gamesList.count==0){
        [NSTimer scheduledTimerWithTimeInterval:0.1 target:self selector:@selector(checkTimer) userInfo:nil repeats:NO];
    } else {
        
        [myTableView reloadData];
    }
    
}
- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - Table view data source

-(CGFloat)tableView:(UITableView *)tableView heightForRowAtIndexPath:(NSIndexPath *)indexPath{
    return 300.0f;
    
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {

    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {

    return allGames.gamesList.count;
}

- (UITableViewCell *) getCellContentView:(NSString *)cellIdentifier {
    
    float width=myTableView.frame.size.width;
    UILabel *lblTemp;
    
    UITableViewCell *cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:cellIdentifier];
    /*
    lblTemp = [[UILabel alloc] initWithFrame:CGRectMake(2,2,width-4,98)];
    lblTemp.backgroundColor= [UIColor colorWithRed:0.95f green:0.95f blue:0.95f alpha:0.95f];
    lblTemp.tag=99;
    [cell.contentView addSubview:lblTemp];
    */
    
    UIImageView *picTemp = [[UIImageView alloc] init];
    picTemp.frame= CGRectMake(0,4,width,200);
    picTemp.contentMode =UIViewContentModeScaleAspectFit;
    picTemp.clipsToBounds=YES;
    picTemp.tag=4;
    [cell.contentView addSubview:picTemp];
    
    lblTemp = [[UILabel alloc] initWithFrame:CGRectMake(5, 208, width-10, 22)];
    lblTemp.font=[UIFont fontWithName:@"HelveticaNeue-Light" size:18];
    lblTemp.tag = 1;
    lblTemp.adjustsFontSizeToFitWidth=YES;
    lblTemp.textColor = [UIColor blackColor];
    lblTemp.backgroundColor= [UIColor clearColor];
    lblTemp.textAlignment=NSTextAlignmentCenter;
    [cell.contentView addSubview:lblTemp];
    
    
   
    
    
    
    
    
    lblTemp = [[UILabel alloc] initWithFrame:CGRectMake((width-140)/2,235,140,40)];
    lblTemp.backgroundColor=  [UIColor colorWithRed:0.13 green:0.30 blue:0.45 alpha:1.0];;
    lblTemp.tag=199;
    lblTemp.layer.cornerRadius = 7;
    lblTemp.layer.masksToBounds = YES;
    [cell.contentView addSubview:lblTemp];
    
    //Initialize Label with tag 2.
    lblTemp = [[UILabel alloc] initWithFrame:CGRectMake(5+(width-140)/2, 240, 130, 30)];
    lblTemp.tag = 2;
    lblTemp.font=[UIFont fontWithName:@"HelveticaNeue-Light" size:16];
    lblTemp.textColor=[UIColor whiteColor];
    //lblTemp.numberOfLines = 2;
    //lblTemp.lineBreakMode = NSLineBreakByWordWrapping;
    lblTemp.textAlignment=NSTextAlignmentCenter;
    lblTemp.backgroundColor= [UIColor clearColor];
    [cell.contentView addSubview:lblTemp];
    
    
    /*
    lblTemp = [[UILabel alloc] initWithFrame:CGRectMake(0,300,width,2)];
    lblTemp.backgroundColor= [UIColor darkGrayColor];
    lblTemp.tag=199;
    [cell.contentView addSubview:lblTemp];*/
    
    return cell;
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    static NSString *CellIdentifier = @"Cell";
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier];
    
    if (cell == nil) {
        cell = [self getCellContentView:CellIdentifier];
        
    }
    
    
    UILabel *lblTemp1 = (UILabel *)[cell viewWithTag:1];
    UILabel *lblTemp2 = (UILabel *)[cell viewWithTag:2];
   // UILabel *lblTemp99 = (UILabel *)[cell viewWithTag:99];
    //UILabel *lblTemp199 = (UILabel *)[cell viewWithTag:199];
    UIImageView *picTemp = (UIImageView *)[cell viewWithTag:4];

    
    NSDictionary* one=[allGames.gamesList objectAtIndex:indexPath.row];
    
    lblTemp1.text=[one objectForKey:@"title"];
    lblTemp2.text=@"$4.00";
    if ([one   objectForKey:@"standardPrice"]){
        float price=[[one objectForKey:@"standardPrice"] floatValue];
        lblTemp2.text=[@"" stringByAppendingFormat:@"$%1.2f",price];
    }
    NSString* imageUrl=[one objectForKey:@"imageSource"];
    
    [NSURLConnection sendAsynchronousRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:imageUrl]] queue:[NSOperationQueue mainQueue] completionHandler:^(NSURLResponse *response, NSData *data, NSError *error) {
        picTemp.image = [UIImage imageWithData:data];
        
    }];
    
    //picTemp.image=[UIImage imageWithCo   imageWithContentsOfURL:theURL];
    
   // NSLog(@"one %@",one);
    
    
    return cell;
}


/*
// Override to support conditional editing of the table view.
- (BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath {
    // Return NO if you do not want the specified item to be editable.
    return YES;
}
*/

/*
// Override to support editing the table view.
- (void)tableView:(UITableView *)tableView commitEditingStyle:(UITableViewCellEditingStyle)editingStyle forRowAtIndexPath:(NSIndexPath *)indexPath {
    if (editingStyle == UITableViewCellEditingStyleDelete) {
        // Delete the row from the data source
        [tableView deleteRowsAtIndexPaths:@[indexPath] withRowAnimation:UITableViewRowAnimationFade];
    } else if (editingStyle == UITableViewCellEditingStyleInsert) {
        // Create a new instance of the appropriate class, insert it into the array, and add a new row to the table view
    }   
}
*/

/*
// Override to support rearranging the table view.
- (void)tableView:(UITableView *)tableView moveRowAtIndexPath:(NSIndexPath *)fromIndexPath toIndexPath:(NSIndexPath *)toIndexPath {
}
*/

/*
// Override to support conditional rearranging of the table view.
- (BOOL)tableView:(UITableView *)tableView canMoveRowAtIndexPath:(NSIndexPath *)indexPath {
    // Return NO if you do not want the item to be re-orderable.
    return YES;
}
*/

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

@end
