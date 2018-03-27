using System;
using Foundation;

namespace InAuth.iOS.Debug
{
	// @interface MME : NSObject
	[BaseType(typeof(NSObject))]
	interface MME
	{
		// -(instancetype)initWithAccountId:(NSString *)accountId applicationId:(NSString *)appId permIdFlagOn:(BOOL)yesOrNo andJSONKeys:(NSData *)jsonKeys;
		[Export("initWithAccountId:applicationId:permIdFlagOn:andJSONKeys:")]
		IntPtr Constructor(string accountId, string appId, bool yesOrNo, NSData jsonKeys);

		// -(void)setPermIdFlagOn:(BOOL)yesOrNo;
		[Export("setPermIdFlagOn:")]
		void SetPermIdFlagOn(bool yesOrNo);

		// -(OpaqueObjectRef)generateRegistrationPayload:(NSError **)error;
		[Export("generateRegistrationPayload:")]
		NSData GenerateRegistrationPayload(out NSError error);

		// -(OpaqueObjectRef)generateRegistrationPayloadWithCustomLog:(NSDictionary *)customLog onError:(NSError **)outError;
		[Export("generateRegistrationPayloadWithCustomLog:onError:")]
		NSData GenerateRegistrationPayloadWithCustomLog(NSDictionary customLog, out NSError outError);

		// -(OpaqueObjectRef)unRegister;
		[Export("unRegister")]
		NSData UnRegister { get; }

		// -(OpaqueObjectRef)generateListRequestPayload:(MMEListSet)selection onError:(NSError **)error;
		[Export("generateListRequestPayload:onError:")]
		NSData GenerateListRequestPayload(ushort selection, out NSError error);

		// -(OpaqueObjectRef)generateListRequestPayload:(MMEListType)type version:(NSString *)version onError:(NSError **)error;
		[Export("generateListRequestPayload:version:onError:")]
		NSData GenerateListRequestPayload(MMEListType type, string version, out NSError error);

		// -(NSString *)listVersion:(MMEListType)type onError:(NSError **)error;
		[Export("listVersion:onError:")]
		string ListVersion(MMEListType type, out NSError error);

		// -(OpaqueObjectRef)generateLogPayload:(MMELogSet)logChoices withCustomLog:(NSDictionary *)customLog transactionId:(NSString *)transId onError:(NSError **)error;
		[Export("generateLogPayload:withCustomLog:transactionId:onError:")]
		NSData GenerateLogPayload(uint logChoices, NSDictionary customLog, string transId, out NSError error);

		// -(OpaqueObjectRef)generateLogPayload:(MMELogSet)logChoices onError:(NSError **)error;
		[Export("generateLogPayload:onError:")]
		NSData GenerateLogPayload(uint logChoices, out NSError error);

		// -(OpaqueObjectRef)generateCustomLogPayload:(NSDictionary *)customLog onError:(NSError **)error;
		[Export("generateCustomLogPayload:onError:")]
		NSData GenerateCustomLogPayload(NSDictionary customLog, out NSError error);

		// -(NSDictionary *)handlePayload:(OpaqueObjectRef)data onError:(NSError **)error;
		[Export("handlePayload:onError:")]
		NSDictionary HandlePayload(NSData data, out NSError error);

		// -(StorageType)whiteBoxCreateItem:(NSData *)item withName:(NSString *)name onError:(NSError **)error;
		[Export("whiteBoxCreateItem:withName:onError:")]
		byte WhiteBoxCreateItem(NSData item, string name, out NSError error);

		// -(StorageType)whiteBoxCreateItem:(NSData *)item withName:(NSString *)name andPolicy:(MMEWhiteBoxPolicySet)policySet onError:(NSError **)error;
		[Export("whiteBoxCreateItem:withName:andPolicy:onError:")]
		byte WhiteBoxCreateItem(NSData item, string name, ushort policySet, out NSError error);

		// -(NSData *)whiteBoxReadItem:(NSString *)name onError:(NSError **)error;
		[Export("whiteBoxReadItem:onError:")]
		NSData WhiteBoxReadItem(string name, out NSError error);

		// -(NSData *)whiteBoxReadItem:(NSString *)name andPolicy:(MMEWhiteBoxPolicySet)policySet onError:(NSError **)error;
		[Export("whiteBoxReadItem:andPolicy:onError:")]
		NSData WhiteBoxReadItem(string name, ushort policySet, out NSError error);

		// -(StorageType)whiteBoxUpdateItem:(NSData *)item withName:(NSString *)name onError:(NSError **)error;
		[Export("whiteBoxUpdateItem:withName:onError:")]
		byte WhiteBoxUpdateItem(NSData item, string name, out NSError error);

		// -(StorageType)whiteBoxUpdateItem:(NSData *)item withName:(NSString *)name andPolicy:(MMEWhiteBoxPolicySet)policySet onError:(NSError **)error;
		[Export("whiteBoxUpdateItem:withName:andPolicy:onError:")]
		byte WhiteBoxUpdateItem(NSData item, string name, ushort policySet, out NSError error);

		// -(void)whiteBoxDestroyItem:(NSString *)name onError:(NSError **)error;
		[Export("whiteBoxDestroyItem:onError:")]
		void WhiteBoxDestroyItem(string name, out NSError error);

		// -(void)whiteBoxDestroyItem:(NSString *)name andPolicy:(MMEWhiteBoxPolicySet)policySet onError:(NSError **)error;
		[Export("whiteBoxDestroyItem:andPolicy:onError:")]
		void WhiteBoxDestroyItem(string name, ushort policySet, out NSError error);

		// -(BOOL)certificateInstall:(OpaqueObjectRef)certificatePayload withDomain:(NSString *)domain onError:(NSError **)error;
		[Export("certificateInstall:withDomain:onError:")]
		bool CertificateInstall(NSData certificatePayload, string domain, out NSError error);

		// -(BOOL)certificateUninstall:(NSString *)domain onError:(NSError **)error;
		[Export("certificateUninstall:onError:")]
		bool CertificateUninstall(string domain, out NSError error);

		// -(BOOL)certificateCompare:(SecTrustRef)trust withDomain:(NSString *)domain onError:(NSError **)error;
		//[Export("certificateCompare:withDomain:onError:")]
		//unsafe bool CertificateCompare(SecTrustRef* trust, string domain, out NSError error);

		// -(void)bindBrowserGeneratePayload:(void (^)(OpaqueObjectRef, NSError *))complete;
		[Export("bindBrowserGeneratePayload:")]
		void BindBrowserGeneratePayload(Action<NSData, NSError> complete);

		// -(BOOL)bindBrowserOpen:(NSURL *)url onError:(NSError **)outError;
		[Export("bindBrowserOpen:onError:")]
		bool BindBrowserOpen(NSUrl url, out NSError outError);

		// -(InAuthState)rootDetectionStateOnError:(NSError **)outError;
		[Export("rootDetectionStateOnError:")]
		ushort RootDetectionStateOnError(out NSError outError);

		// -(InAuthState)malwareDetectionStateOnError:(NSError **)outError;
		[Export("malwareDetectionStateOnError:")]
		ushort MalwareDetectionStateOnError(out NSError outError);

		// -(NSArray *)malwareDetectionList;
		[Export("malwareDetectionList")]
		NSObject[] MalwareDetectionList { get; }
	}
}
