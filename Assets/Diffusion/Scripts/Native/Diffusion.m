//
//  Diffusion.m
//  DiffusionDevelopment
//
//  Created by Christopher Baltzer on 2013-09-12.
//  Copyright (c) 2013 Chris Baltzer. All rights reserved.
//

#import "Diffusion.h"
#import <Social/Social.h>

void UnitySendMessage(const char * obj, const char * func, const char * param);

void _share(const char * message, const char * url, const char * filePath, int platformsToHide) {
    NSString *m = [NSString stringWithUTF8String:message];
    NSString *u = [NSString stringWithUTF8String:url];
    NSString *f = [NSString stringWithUTF8String:filePath];
    
    [[Diffusion sharedInstance] setActivityMask:platformsToHide];
    [[Diffusion sharedInstance] share:m withURL:u andFile:f];
}

void _postToTwitter(const char * message, const char * url, const char * filePath) {
    NSString *m = [NSString stringWithUTF8String:message];
    NSString *u = [NSString stringWithUTF8String:url];
    NSString *f = [NSString stringWithUTF8String:filePath];
    
    [[Diffusion sharedInstance] postToTwitter:m withURL:u andImage:f];
}

void _postToFacebook(const char * message, const char * url, const char * filePath) {
    NSString *m = [NSString stringWithUTF8String:message];
    NSString *u = [NSString stringWithUTF8String:url];
    NSString *f = [NSString stringWithUTF8String:filePath];
    
    [[Diffusion sharedInstance] postToFacebook:m withURL:u andImage:f];
}

void _prewarm() {
    [[Diffusion sharedInstance] prewarm];
}

void _addCustomActivity(const char * activity) {
    [[Diffusion sharedInstance] addCustomActivity:[NSString stringWithUTF8String:activity]];
}

bool _isFacebookConnected() {
    return [[Diffusion sharedInstance] isFacebookConnected];
}

bool _isTwitterConnected() {
    return [[Diffusion sharedInstance] isTwitterConnected];
}


@implementation Diffusion

static Diffusion *sharedInstance = nil;
+(Diffusion*)sharedInstance {
	if( !sharedInstance )
		sharedInstance = [[Diffusion alloc] init];
    
	return sharedInstance;
}

-(id)init {
    if (self = [super init]) {
        self.systemActivities = @[
                               UIActivityTypePostToFacebook,
                               UIActivityTypePostToTwitter,
                               UIActivityTypePostToWeibo,
                               UIActivityTypeMessage,
                               UIActivityTypeMail,
                               UIActivityTypePrint,
                               UIActivityTypeCopyToPasteboard,
                               UIActivityTypeAssignToContact,
                               UIActivityTypeSaveToCameraRoll,
                               @"com.apple.UIKit.activity.AddToReadingList",
                               @"com.apple.UIKit.activity.PostToFlickr",
                               @"com.apple.UIKit.activity.PostToVimeo",
                               @"com.apple.UIKit.activity.TencentWeibo",
                               @"com.apple.UIKit.activity.AirDrop"
                               ];
        
        self.customActivities = [NSMutableArray arrayWithCapacity:0];
    }
    
    return self;
}

-(void)prewarm {
    NSLog(@"Diffusion initialized");
}

-(void)addCustomActivity:(NSString*)className {
    id customActivity = [[NSClassFromString(className) alloc] init];
    if (customActivity == nil) {
        NSLog(@"No class named %@ could be found! Double check for typos!", className);
    } else {
        // Don't add duplicates!
        if (![self.customActivities containsObject:customActivity]) {
            [self.customActivities addObject:customActivity];
        }
    }
}


#pragma mark - Accounts

-(BOOL)isFacebookConnected {
    return [SLComposeViewController isAvailableForServiceType:SLServiceTypeFacebook];
}

-(BOOL)isTwitterConnected {
    return [SLComposeViewController isAvailableForServiceType:SLServiceTypeTwitter];
}

-(void)postToTwitter:(NSString*)message withURL:(NSString*)url andImage:(NSString*)file {
    SLComposeViewController *twitter = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeTwitter];
    
    [twitter setCompletionHandler:^(SLComposeViewControllerResult result) {
        
        BOOL completed = NO;
        if (result == SLComposeViewControllerResultDone) {
            completed = YES;
        }
        
        [self shareCallback:UIActivityTypePostToTwitter didComplete:completed];
    }];
    
    [self share:message withURL:url andImage:file withController:twitter];
}

-(void)postToFacebook:(NSString*)message withURL:(NSString*)url andImage:(NSString*)file {
    SLComposeViewController *fb = [SLComposeViewController composeViewControllerForServiceType:SLServiceTypeFacebook];
    
    [fb setCompletionHandler:^(SLComposeViewControllerResult result) {
        
        BOOL completed = NO;
        if (result == SLComposeViewControllerResultDone) {
            completed = YES;
        }
        
        [self shareCallback:UIActivityTypePostToFacebook didComplete:completed];
    }];
    
    [self share:message withURL:url andImage:file withController:fb];
}

-(void)share:(NSString*)message withURL:(NSString *)url andImage:(NSString*)path withController:(SLComposeViewController*)controller
{
    [controller setInitialText:message];
    
    if (![url isEqualToString:@""]) {
        NSURL *nsurl = [NSURL URLWithString:url];
        [controller addURL:nsurl];
    }
    
    UIImage *image = [self imageAtPath:path];
    if (image) {
        [controller addImage:image];
    }
    
    UIWindow *window = [UIApplication sharedApplication].keyWindow;
    [window.rootViewController presentViewController:controller animated:YES completion:nil];
}


#pragma mark - UIActivity

-(void)share:(NSString*)message withURL:(NSString*)url andFile:(NSString*)file {
    // Setup the data to pass!
    NSMutableArray *data = [NSMutableArray array];
    
    DiffusionShareText *shareMessage = [[DiffusionShareText alloc] init];
    shareMessage.shareText = message;
    
    [data addObject:shareMessage];
    
    if (![url isEqualToString:@""]) {
        NSURL *nsurl = [NSURL URLWithString:url];
        [data addObject:nsurl];
    }
    
    if (![file isEqualToString:@""]) {

        NSURL *fileurl;
        if ([[file substringToIndex:4] isEqualToString:@"file"]) {
            fileurl = [NSURL URLWithString:file];
        } else {
            fileurl = [NSURL fileURLWithPath:file];
        }
        
        NSData *urldata = [NSData dataWithContentsOfURL:fileurl];
        UIImage *image = [UIImage imageWithData:urldata];
        if (image != nil) {
            [data addObject:image];
        } else {
            [data addObject:fileurl];
        }
    }

    NSArray *custom;
    if (self.customActivities.count > 0) {
        custom = [NSArray arrayWithArray:self.customActivities];
    } else {
        custom = nil;
    }
    
    UIActivityViewController *activityVC = [[UIActivityViewController alloc] initWithActivityItems:data applicationActivities:custom];
    
    
    activityVC.excludedActivityTypes = [self activiesToHide];
    
    
    // set the callback to unity
    [activityVC setCompletionHandler:^(NSString *activityType, BOOL completed) {
        [self shareCallback:activityType didComplete:completed];
    }];
    
    // Present the share sheet
    UIWindow *window = [UIApplication sharedApplication].keyWindow;
    
    if ([activityVC respondsToSelector:@selector(popoverPresentationController)])
    {
        // iOS 8+
        UIPopoverPresentationController *presentationController = [activityVC popoverPresentationController];
        CGRect screen = [[UIScreen mainScreen] applicationFrame];
        
        UIView *tmp = [[UIView alloc] initWithFrame:screen];
        presentationController.sourceView = tmp;
    }


    [window.rootViewController presentViewController:activityVC animated:YES completion:nil];
}





#pragma mark - Callbacks

-(void)shareCallback:(NSString*)type didComplete:(BOOL)completed {
    if (!completed) {
        [self sendMessageToGameObject:@"Diffusion" withMethod:@"Cancelled" andMessage:@""];
    } else {
        NSString *returnType = [NSString stringWithFormat:@"%d", [self activityTypeToUnityInt:type]];
        [self sendMessageToGameObject:@"Diffusion" withMethod:@"Completed" andMessage:returnType];
    }
}


#pragma mark - Utils

-(UIImage*)imageAtPath:(NSString*)file {
    if (![file isEqualToString:@""]) {
        
        NSURL *fileurl;
        if ([[file substringToIndex:4] isEqualToString:@"file"]) {
            fileurl = [NSURL URLWithString:file];
        } else {
            fileurl = [NSURL fileURLWithPath:file];
        }
        
        NSData *urldata = [NSData dataWithContentsOfURL:fileurl];
        return [UIImage imageWithData:urldata];
    }
    return nil;
}

-(void)sendMessageToGameObject:(NSString*)gameObject withMethod:(NSString*)method andMessage:(NSString*)message {
    NSLog(@"Unity message to %@: %@ (%@)", gameObject, method, message);
    #ifdef UNITY_VERSION
        UnitySendMessage([gameObject UTF8String], [method UTF8String], [message UTF8String]);
    #endif
    
}


-(NSArray*)activiesToHide {
    NSMutableArray *hide = [NSMutableArray arrayWithCapacity:2];
    
    for (int i = 1; i <= self.systemActivities.count; i++) { // enum starts at 2 on unity side
        if (self.activityMask & (1 << i)) {
            [hide addObject:[self.systemActivities objectAtIndex:(i-1)]];
        }
    }
    return hide;
}


-(int)activityTypeToUnityInt:(NSString*)activity {
    int index = (int)[self.systemActivities indexOfObject:activity];
    index++; // unity starts at 1
    return (1 << index);
}

@end



@implementation DiffusionShareText

- (id)activityViewControllerPlaceholderItem:(UIActivityViewController *)activityViewController {
    return self.shareText;
}

- (id)activityViewController:(UIActivityViewController *)activityViewController itemForActivityType:(NSString *)activityType {
    if ([activityType isEqualToString:UIActivityTypePostToFacebook]) {
       return @"";
    } else {
        return self.shareText;
    }
}

- (void)dealloc {
    _shareText = nil;
}


@end
