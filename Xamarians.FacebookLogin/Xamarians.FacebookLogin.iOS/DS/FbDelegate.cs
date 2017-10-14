using System;
using Facebook.ShareKit;
using Foundation;
using UIKit;

namespace Xamarians.FacebookLogin.iOS.DS
{
    public class FbDelegate : NSObject, ISharingDelegate
    {
        UIAlertView alert;
        public FbDelegate()
        {
            
        }


        public void DidCancel(ISharing sharer)
        {
           ShowPopUp("", "User has cancelled", null, "Ok", null);
        }

        public void DidComplete(ISharing sharer, NSDictionary results)
        {
            ShowPopUp("Success", "Your post has successfully posted on facebook", null, "Ok", null);
        }

        public void DidFail(ISharing sharer, NSError error)
        {
            ShowPopUp("Success", error.Description, null, "Ok", null);
        }

        public void ShowPopUp(string title, string message, IUIAlertViewDelegate del, string cancelButtonTitle, string[] otherButton)
        {
            alert = new UIAlertView(title, message, del, cancelButtonTitle, otherButton);
            alert.Show();
        }

    }
}
