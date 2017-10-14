using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarians.FacebookLogin;
using Xamarin.Forms;
using System.IO;
using Xamarians.Media;
#if __ANDROID__
using Newtonsoft.Json;
#endif
using Xamarians.FacebookLogin.Platforms;

namespace Sample
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

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
            string filePath = GenerateFilePath();
            var result = await MediaService.Instance.TakePhotoAsync(new CameraOption()
            {
                FilePath = filePath,
                MaxWidth = 300,
                MaxHeight = 300
            });
            await Task.Delay(1000);
            DependencyService.Get<IFacebookLogin>().ShareImageOnFacebook("Hi, This is demo text", result.FilePath);
        }

        private void FbTextShareClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IFacebookLogin>().ShareTextOnFacebook("Hi, This is a demo text");
        }

        private void FbLinkShareClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IFacebookLogin>().ShareLinkOnFacebook("Joybird Link", "Hi, this is demo text", "http://www.joybird.net");
        }
    }

    
}
