using System.Threading.Tasks;
using Android.Content;
using Xamarians.FacebookLogin.Platforms;
using Xamarians.FacebookLogin.Droid.Platforms;
using Xamarin.Forms;
using Xamarians.FacebookLogin.Droid.DS;
using System;
using Android.Content.PM;
using Android.Widget;

[assembly: Dependency(typeof(FacebookLogin))]
namespace Xamarians.FacebookLogin.Droid.DS
{
    public class FacebookLogin : IFacebookLogin
    {
        public static string FacebookAppId { get; set; }
        public static void Init(string facebookAppId)
        {
            FacebookAppId = facebookAppId;
        }
        private bool CanOpenApp()
        {
            try
            {
                ApplicationInfo info = Xamarin.Forms.Forms.Context.PackageManager.
                        GetApplicationInfo("com.facebook.katana", 0);
                return true;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                return false;
            }
        }

        public void ShareImageOnFacebook(string caption, string imagePath)
        {
            var dbIntent = new Intent(Xamarin.Forms.Forms.Context, typeof(FacebookShareActivity));
            dbIntent.PutExtra("Title", caption);
            dbIntent.PutExtra("ImagePath", imagePath);
            Xamarin.Forms.Forms.Context.StartActivity(dbIntent);
        }

        public void ShareLinkOnFacebook(string title, string description, string link)
        {
            var dbIntent = new Intent(Xamarin.Forms.Forms.Context, typeof(FacebookShareActivity));
            dbIntent.PutExtra("Title", title);
            dbIntent.PutExtra("Description", description);
            dbIntent.PutExtra("Link", link);
            Xamarin.Forms.Forms.Context.StartActivity(dbIntent);
        }

        public void ShareTextOnFacebook(string text)
        {
            var isAppInstalled = CanOpenApp();
            if(!isAppInstalled)
            {
                Toast.MakeText(Xamarin.Forms.Forms.Context, "Cannot share text when app is not installed", ToastLength.Long).Show();
                return;
            }
            var dbIntent = new Intent(Xamarin.Forms.Forms.Context, typeof(FacebookShareActivity));
            dbIntent.PutExtra("Title", "Joybird");
            dbIntent.PutExtra("Description", text);
            Xamarin.Forms.Forms.Context.StartActivity(dbIntent);
        }

        public Task<FbLoginResult> SignIn()
        {
            var tcs = new TaskCompletionSource<FbLoginResult>();
            FacebookLoginActivity.OnLoginCompleted(tcs);
            var fbIntent = new Intent(Xamarin.Forms.Forms.Context, typeof(FacebookLoginActivity));
            fbIntent.PutExtra("Permissions", "email");
            Xamarin.Forms.Forms.Context.StartActivity(fbIntent);
            return tcs.Task;
        }

        public Task<FbLoginResult> SignOut()
        {
            var tcs = new TaskCompletionSource<FbLoginResult>();
            FacebookLoginActivity.SignOut(tcs);
            return tcs.Task;
        }
    }
}