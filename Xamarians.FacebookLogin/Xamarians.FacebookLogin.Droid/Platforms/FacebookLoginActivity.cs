using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarians.FacebookLogin.Platforms;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xamarians.FacebookLogin.Droid.Platforms
{
    [Activity(Label = "FacebookLoginActivity")]
    public class FacebookLoginActivity : Activity, GraphRequest.ICallback
    {
        static TaskCompletionSource<FbLoginResult> fbActivityComplete;
        static TaskCompletionSource<FbLoginResult> logOut;
        ICallbackManager callbackManager;

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            string title = Intent.GetStringExtra("Title");
            string description = Intent.GetStringExtra("Description");
            string imageUrl = Intent.GetStringExtra("ImageUrl");
            string permissions = Intent.GetStringExtra("Permissions");
            if (string.IsNullOrWhiteSpace(permissions))
                permissions = "email";
            //Permissions
            base.OnCreate(savedInstanceState);

            FacebookSdk.ApplicationId = Xamarians.FacebookLogin.Droid.DS.FacebookLogin.FacebookAppId;
            //FacebookSdk.ApplicationName = Interveo.App.FacebookAppName;
            FacebookSdk.SdkInitialize(ApplicationContext);
            callbackManager = CallbackManagerFactory.Create();

            var fbLoginCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = loginResult =>
                {
                    PullUserProfileAsync();
                    //ActivityCompleted(FBStatus.Success, "");
                },
                HandleCancel = () =>
                {
                    Toast.MakeText(ApplicationContext, "Cancelled", ToastLength.Long).Show();
                    ActivityCompleted(FBStatus.Cancelled, "User has cancelled.");
                },
                HandleError = loginError =>
                {
                    LoginManager.Instance.LogOut();
                    Toast.MakeText(ApplicationContext, "Error:" + loginError.Message, ToastLength.Long).Show();
                    ActivityCompleted(FBStatus.Error, loginError.Message);
                }
            };
            LoginManager.Instance.RegisterCallback(callbackManager, fbLoginCallback);
            if (permissions.Contains("publish"))
                LoginManager.Instance.LogInWithPublishPermissions(this, permissions.Split(','));
            else
                LoginManager.Instance.LogInWithReadPermissions(this, permissions.Split(','));
        }


        protected override void OnActivityResult(int requestCode, [Android.Runtime.GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        private void PullUserProfileAsync()
        {
            var parameters = new Android.OS.Bundle();
            parameters.PutString("fields", "id,name,email,gender");
            GraphRequest request = new GraphRequest(AccessToken.CurrentAccessToken, "me", parameters, HttpMethod.Get, this);
            request.ExecuteAsync();
        }

        // Implement GraphRequest.ICallback
        void GraphRequest.ICallback.OnCompleted(GraphResponse response)
        {
            if (response.Error != null)
            {
                ActivityCompleted(FBStatus.Error, response.Error.ErrorMessage, null);
            }
            else
            {
                ActivityCompleted(FBStatus.Success, "Authentication Completed.", response.RawResponse);
            }
        }

        private void ActivityCompleted(FBStatus status, string message, string jsonData = null)
        {
            Finish();
            if (fbActivityComplete != null)
            {
                if (AccessToken.CurrentAccessToken == null)
                {
                    fbActivityComplete.SetResult(new FbLoginResult
                    {
                        Status = status,
                        Message = message
                    });
                }
                else
                {
                    var userInfo = JsonConvert.DeserializeObject<FbLoginResult>(jsonData);
                    fbActivityComplete.SetResult(new FbLoginResult
                    {
                        ApplicationId = AccessToken.CurrentAccessToken.ApplicationId,
                        UserId = AccessToken.CurrentAccessToken.UserId,
                        AccessToken = AccessToken.CurrentAccessToken.Token,
                        Status = status,
                        Message = message,
                        Name = userInfo.Name,
                        Email = userInfo.Email,
                    });
                }

            }
        }

        public static void OnLoginCompleted(TaskCompletionSource<FbLoginResult> completed)
        {
            fbActivityComplete = completed;
        }

        public static void SignOut(TaskCompletionSource<FbLoginResult> logout)
        {
            logOut = logout;

            try
            { 
                LoginManager.Instance.LogOut();
                logOut.SetResult(new FbLoginResult()
                {
                    Status = FBStatus.Success,
                    Message = "Successfully Logged Out"
                });
            }
            catch(Exception ex)
            {
                logOut.SetResult(new FbLoginResult()
                {
                    Status = FBStatus.Error,
                    Message = "You are not Logged In"
                });
            }
        }
    }

}