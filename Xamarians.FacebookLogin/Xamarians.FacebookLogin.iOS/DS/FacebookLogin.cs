using Facebook.CoreKit;
using Facebook.LoginKit;
using Foundation;
using System.Threading.Tasks;
using UIKit;
using Xamarians.FacebookLogin.Platforms;
using Xamarin.Forms;
using Xamarians.FacebookLogin.iOS.DS;
using System;
using Facebook.ShareKit;

[assembly: Dependency(typeof(Xamarians.FacebookLogin.iOS.DS.FacebookLogin))]
namespace Xamarians.FacebookLogin.iOS.DS
{
    public class FacebookLogin : Xamarians.FacebookLogin.IFacebookLogin
	{
        bool isLoggedIn = false;
		public static void Init()
		{
			
		}

		private UIViewController GetController()
		{
            var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (vc.PresentedViewController != null)
                vc = vc.PresentedViewController;
            return vc;
        }

		LoginManager manager;
		public async Task<FbLoginResult> SignIn()
		{
			var tcs = new TaskCompletionSource<FbLoginResult>();
			manager = new LoginManager();
			manager.Init();
			var result = await manager.LogInWithReadPermissionsAsync(new string[] { "email", "public_profile" }, GetController());
			if (!result.IsCancelled)
			{
				try
				{
					var request = new GraphRequest("/me?fields=id,name,email", null, result.Token.TokenString, null, "GET");
					request.Start((connection, res, error) =>
					{
						var userInfo = res as NSDictionary;
						var id = userInfo["id"].ToString();
						var name = userInfo["name"].ToString();
						var email = userInfo["email"].ToString();
						tcs.SetResult(new FbLoginResult
						{
							AccessToken = result.Token.ToString(),
							UserId = id,
							Name = name,
							Email = email,
                            Status = FBStatus.Success
						});
					});
                    isLoggedIn = true;
				}
				catch { }
               
			}
			else if (result.IsCancelled)
			{
                isLoggedIn = false;
                tcs.SetResult(new FbLoginResult(){Status = FBStatus.Cancelled, Message = "Cancelled"});
			}

			return await tcs.Task;
		}

		public Task<FbLoginResult> SignOut()
		{
			var tcs = new TaskCompletionSource<FbLoginResult>();
			if (manager != null)
			{
                if (isLoggedIn)
                {
                    manager.LogOut();
                    isLoggedIn = false;
                    tcs.SetResult(new FbLoginResult()
                    {
                        Status = FBStatus.Success,
                        Message = "Successfully Logged Out"
                    });
                }
                else
                {
					tcs.SetResult(new FbLoginResult()
					{
						Status = FBStatus.Error,
						Message = "You are not Logged In"
					});
                }
			}
			else
			{
				tcs.SetResult(new FbLoginResult()
				{
					Status = FBStatus.Error,
					Message = "You are not Logged In"
				});
			}
			return tcs.Task;
		}

        public void ShareLinkOnFacebook(string text, string description, string link)
        {
			ShareLinkContent link1 = new ShareLinkContent();
			link1.SetContentUrl(new NSUrl(link));
			var shareDelegate = new FbDelegate();

			var dialog = new ShareDialog();
            dialog.Mode = ShareDialogMode.FeedBrowser;
            dialog.SetDelegate(shareDelegate);
			dialog.SetShareContent(link1);
			dialog.FromViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
			bool isInstalled = UIApplication.SharedApplication.CanOpenUrl(new NSUrl(urlString: "fb://"));
			if (isInstalled)
			{
				dialog.Mode = ShareDialogMode.Native;
				dialog.Show();
			}
			else
			{
				ShareAPI.Share(link1, shareDelegate);
			}
        }

        public void ShareTextOnFacebook(string text)
        {
			ShareLinkContent link = new ShareLinkContent();
			var shareDelegate = new FbDelegate();

			var dialog = new ShareDialog();
			dialog.Mode = ShareDialogMode.Native;
			dialog.SetDelegate(shareDelegate);
			dialog.SetShareContent(link);
			dialog.FromViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
			bool isInstalled = UIApplication.SharedApplication.CanOpenUrl(new NSUrl(urlString: "fb://"));
			if (isInstalled)
			{
				dialog.Show();
			}
            else
            {
                new UIAlertView("Error", "Cannot share text when app is not installed", null, "Ok", null).Show();    
            }
        }

        public void ShareImageOnFacebook(string caption, string imagePath)
        {
            SharePhoto tt = SharePhoto.From(new UIImage(imagePath), true);
            tt.Caption = caption;
      		SharePhotoContent content = new SharePhotoContent();
			content.Photos = new SharePhoto[] { tt };
			content.SetContentUrl(new NSUrl(imagePath));

			var shareDelegate = new FbDelegate();

			var dialog = new ShareDialog();
            dialog.Mode = ShareDialogMode.FeedBrowser;
			dialog.SetDelegate(shareDelegate);
			dialog.SetShareContent(content);
			dialog.FromViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            bool isInstalled = UIApplication.SharedApplication.CanOpenUrl(new NSUrl(urlString: "fb://"));
            if (isInstalled)
			{
                dialog.Mode = ShareDialogMode.Native;
                dialog.Show();
			}
			else
			{
                ShareAPI.Share(content, shareDelegate);
			}
			
        }
    }
}
