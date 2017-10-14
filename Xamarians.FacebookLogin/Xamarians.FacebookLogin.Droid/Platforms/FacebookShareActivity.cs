using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Xamarin.Facebook;
using Xamarin.Facebook.Share.Model;
using Xamarin.Facebook.Share.Widget;
using Xamarin.Facebook.Login;
using Xamarians.FacebookLogin.Platforms;
using Xamarin.Facebook.Share;

namespace Xamarians.FacebookLogin.Droid.Platforms
{
    [Activity(Label = "Facebook")]
    public class FacebookShareActivity : Activity
    {
        static Action<FBShareResult> fbShareResultComplete;

        ICallbackManager callbackManager;
        FacebookCallback<SharerResult> fbShareCallback;

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            string title = Intent.GetStringExtra("Title");
            string description = Intent.GetStringExtra("Description");
            string imageUrl = Intent.GetStringExtra("ImageUrl");
            string localImagePath = Intent.GetStringExtra("ImagePath");
            string localVideoPath = Intent.GetStringExtra("VideoPath");
            string link = Intent.GetStringExtra("Link");

            base.OnCreate(savedInstanceState);

            FacebookSdk.ApplicationId = Droid.DS.FacebookLogin.FacebookAppId;
            FacebookSdk.ApplicationName = "";
            FacebookSdk.SdkInitialize(ApplicationContext);


            callbackManager = CallbackManagerFactory.Create();
            fbShareCallback = new FacebookCallback<SharerResult>
            {
                HandleSuccess = loginResult =>
                {
                    Toast.MakeText(ApplicationContext, "Your post has been shared successfully.", ToastLength.Long).Show();
                    ShareCompleted(ShareStatus.Success, "Your post has been shared successfully.");
                },
                HandleCancel = () =>
                {
                    Toast.MakeText(ApplicationContext, "Cancelled", ToastLength.Long).Show();
                    ShareCompleted(ShareStatus.Cancelled, "User has cancelled.");
                },
                HandleError = loginError =>
                {
                    Toast.MakeText(ApplicationContext, "Error " + loginError.Message, ToastLength.Long).Show();
                    ShareCompleted(ShareStatus.Error, loginError.Message);
                }
            };
            ShareContent shareContent = null;
            if (!string.IsNullOrWhiteSpace(localImagePath))
            {
                SharePhoto sharePhoto = (SharePhoto)new SharePhoto.Builder()
                            .SetBitmap(Android.Graphics.BitmapFactory.DecodeFile(localImagePath))
                            .SetCaption(title)
                            .Build();
                SharePhotoContent content = new SharePhotoContent.Builder()
                .AddPhoto(sharePhoto)
                .Build();

                shareContent = content; // new ShareMediaContent.Builder().AddMedium(sharePhoto).Build();
            }

            else if (!string.IsNullOrWhiteSpace(localVideoPath))
            {
                Android.Net.Uri videoFileUri = Android.Net.Uri.FromFile(new Java.IO.File(localVideoPath));
                ShareVideo shareVideo = (ShareVideo)new ShareVideo.Builder()
                           .SetLocalUrl(videoFileUri)
                           .Build();
                ShareVideoContent content = new ShareVideoContent.Builder()
                        .SetVideo(shareVideo)
                        .Build();
                shareContent = content;

            }
            else
            {
                var contentBuilder = new ShareLinkContent.Builder();
                contentBuilder.SetContentTitle(title);
                if (!string.IsNullOrWhiteSpace(description))
                    contentBuilder.SetContentDescription(description);
                if (!string.IsNullOrWhiteSpace(imageUrl))
                    contentBuilder.SetImageUrl(Android.Net.Uri.Parse(imageUrl));
                if(!string.IsNullOrWhiteSpace(link))
                contentBuilder.SetContentUrl(Android.Net.Uri.Parse(link));
                shareContent = contentBuilder.Build();
            }
            if (ShareDialog.CanShow(shareContent.Class))
            {
                var shareDialog = new ShareDialog(this);
                shareDialog.RegisterCallback(callbackManager, fbShareCallback);
                shareDialog.Show(shareContent, ShareDialog.Mode.Automatic);
                return;
            }
            else
            {
                var FBLoginCallback = new FacebookCallback<LoginResult>
                {
                    HandleSuccess = loginResult =>
                    {
                        ShareApi.Share(shareContent, fbShareCallback);
                    },
                    HandleCancel = () =>
                    {
                        Toast.MakeText(ApplicationContext, "Cancelled", ToastLength.Long).Show();
                        ShareCompleted(ShareStatus.Cancelled, "User has cancelled.");
                    },
                    HandleError = loginError =>
                    {
                        LoginManager.Instance.LogOut();
                        Toast.MakeText(ApplicationContext, "Error " + loginError.Message, ToastLength.Long).Show();
                        ShareCompleted(ShareStatus.Error, loginError.Message);
                    }
                };
                LoginManager.Instance.RegisterCallback(callbackManager, FBLoginCallback);
                LoginManager.Instance.LogInWithPublishPermissions(this, new System.Collections.Generic.List<string>() { "publish_actions" });
            }

        }

        protected override void OnActivityResult(int requestCode, [Android.Runtime.GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        private void ShareCompleted(ShareStatus status, string message)
        {
            Finish();
            if (fbShareResultComplete != null)
            {
                fbShareResultComplete.Invoke(new FBShareResult { Status = status, Message = message });
            }
        }

        public static void OnShareCompleted(Action<FBShareResult> completed)
        {
            fbShareResultComplete = completed;
        }

    }
}