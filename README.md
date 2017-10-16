# Xamarians.FacebookSdk

Cross platform library to allow users to login through facebook account in the app and also retrieves the profile for that particular user.

First install package from nuget using following command -

## Install-Package Xamarians.FacebookSdk

You can integrate FacebookSdk in Xamarin Form application using following code:

Shared Code -

Add this to call the facebook sdk functions
```c#
       private string GenerateFilePath()
        {
            return Path.Combine(MediaService.Instance.GetPublicDirectoryPath(), MediaService.Instance.GenerateUniqueFileName("jpg"));
        }

        private async void FbSignInClicked(object sender, EventArgs e)
        {
            var result = await DependencyService.Get<IFacebookLogin>().SignIn();
            if(result.Status == Xamarians.FacebookLogin.Platforms.FBStatus.Success)
            {
#if __ANDROID__
                var userDetails = JsonConvert.DeserializeObject<FbLoginResult>(result.JsonData);
                await DisplayAlert("Success", "Welcome" + userDetails.Name, "Ok");
#endif
#if __IOS__
                await DisplayAlert("Success", "Welcome" + result.Name, "Ok");
#endif

            }
            else
            {
                await DisplayAlert("Error", result.Message, "Ok");
            }

        }

        private async void FbSignOutClicked(object sender, EventArgs e)
        {
            var result = await DependencyService.Get<IFacebookLogin>().SignOut();
            if (result.Status == Xamarians.FacebookLogin.Platforms.FBStatus.Success)
            {
                await DisplayAlert("Success", result.Message, "Ok");
            }
            else
            {
                await DisplayAlert("Error", result.Message, "Ok");
            }
        }

        private async void FbImageShareClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IFacebookLogin>().ShareImageOnFacebook("Hi, This is demo text", "image-filePath");
        }

        private void FbTextShareClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IFacebookLogin>().ShareTextOnFacebook("Hi, This is a demo text");
        }

        private void FbLinkShareClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IFacebookLogin>().ShareLinkOnFacebook("your Link", "Hi, this is demo text","http://www.google.com");
        }
        
```	     
iOS - in AppDelegate file write below code -

```c#
 Xamarians.FacebookLogin.iOS.DS.FacebookLogin.Init();
```

iOS - Add following permissions in the info.plist

```
	<key>CFBundleVersion</key>
	<string>1.0</string>
	<key>FacebookAppID</key>
	<string>your-facebook-id</string>
	<key>FacebookDisplayName</key>
	<string>your-app-name</string>
	<key>LSApplicationQueriesSchemes</key>
	<array>
		<string>fbapi</string>
		<string>fb-messenger-api</string>
		<string>fbauth2</string>
		<string>fbshareextension</string>
		<string>fb</string>
	</array>
	<key>CFBundleURLTypes</key>
	<array>
		<dict>
			<key>CFBundleURLSchemes</key>
			<array>
				<string>fb[facebookid]</string>
			</array>
		</dict>
	</array>
	<key>NSAppTransportSecurity</key>
	<dict>
		<key>NSAllowsArbitraryLoads</key>
		<true/>
	</dict>
  ```
  
  Android - in Mainactivity file write the below code
  
  ```
  Xamarians.FacebookLogin.Droid.DS.FacebookLogin.Init("your-facebook-app-id");
  ```
  Add the following permissions in Android Manifest file
```
  <uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
  <uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
  <uses-permission android:name="android.permission.INTERNET" />
  
```
Add the following in the Application tag in Android Manifest file

```
<meta-data android:name="com.facebook.sdk.ApplicationId" android:value="your-facebook-app-id" />
<activity android:theme="@android:style/Theme.Translucent.NoTitleBar" android:label="Facebook Login" android:name="com.facebook.FacebookActivity" />
<provider android:authorities="com.facebook.app.FacebookContentProvider[your-facebook-app-id]" android:name="com.facebook.FacebookContentProvider" android:exported="true" />
    
```
        
