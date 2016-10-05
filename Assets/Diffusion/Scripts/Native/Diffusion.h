//
//  Diffusion.h
//  DiffusionDevelopment
//
//  Created by Christopher Baltzer on 2013-09-12.
//  Copyright (c) 2013 Chris Baltzer. All rights reserved.
//

#import <Foundation/Foundation.h>

void _share(const char *, const char *, const char *, int);
void _postToTwitter(const char *, const char *, const char *);
void _postToFacebook(const char *, const char *, const char *);
void _prewarm();
void _addCustomActivity(const char *);
bool _isFacebookConnected();
bool _isTwitterConnected();

@interface Diffusion : NSObject

@property (nonatomic, assign) int activityMask;
@property (nonatomic, strong) NSArray* systemActivities;
@property (nonatomic, strong) NSMutableArray* customActivities;
@property (nonatomic, strong) NSMutableArray* shareData;

+(Diffusion*)sharedInstance;
-(void)prewarm;
-(void)addCustomActivity:(NSString*)activity;

-(BOOL)isFacebookConnected;
-(BOOL)isTwitterConnected;


-(void)postToTwitter:(NSString*)message withURL:(NSString*)url andImage:(NSString*)file;

-(void)postToFacebook:(NSString*)message withURL:(NSString*)url andImage:(NSString*)file;

-(void)share:(NSString*)message withURL:(NSString*)url andFile:(NSString*)file;

-(void)shareCallback:(NSString*)type didComplete:(BOOL)completed;



-(void)sendMessageToGameObject:(NSString*)gameObject withMethod:(NSString*)method andMessage:(NSString*)message;
-(NSArray*)activiesToHide;
-(int)activityTypeToUnityInt:(NSString*)activity;
@end


@interface DiffusionShareText : NSObject <UIActivityItemSource>

@property (nonatomic, copy) NSString* shareText;

@end