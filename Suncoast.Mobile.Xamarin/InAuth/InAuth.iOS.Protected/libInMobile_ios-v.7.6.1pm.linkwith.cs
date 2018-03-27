// WARNING: This feature is deprecated. Use the "Native References" folder instead.
// Right-click on the "Native References" folder, select "Add Native Reference",
// and then select the static library or framework that you'd like to bind.
//
// Once you've added your static library or framework, right-click the library or
// framework and select "Properties" to change the LinkWith values.

using ObjCRuntime;

[assembly: LinkWith("libInMobile_ios-v.7.6.1pm.a", SmartLink = true, ForceLoad = true,
	LinkerFlags = "-ObjC -framework CoreTelephony -framework MobileCoreServices -framework CoreMotion -framework CoreFoundation -framework AddressBook -framework AudioToolbox -framework AVFoundation -framework CFNetwork -framework CoreGraphics -framework CoreLocation -framework Foundation -framework MediaPlayer -framework SystemConfiguration -framework Security -framework UIKit -lstdc++ -lstdc++.6.0.9 -lz -LResources -ljansson -lcryptopp")]