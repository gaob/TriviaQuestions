using System;
using System.Drawing;
using System.Net.Http;
using Microsoft.WindowsAzure.MobileServices;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreImage;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json.Linq;

namespace iOSClient
{
    public partial class iOSClientViewController : UIViewController
    {
        public iOSClientViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        private MobileServiceHelper client;

        #region View lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            client = MobileServiceHelper.DefaultService;

            CallAPIGetButton.TouchUpInside += CallAPIGetButton_TouchUpInside;
            CallAPIPostButton.TouchUpInside += CallAPIPostButton_TouchUpInside;

        }

        async void CallAPIGetButton_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                StatusLabel.Text = "GET Request Made, waiting for response...";
                StatusLabel.TextColor = UIColor.White;
                StatusLabel.BackgroundColor = UIColor.Blue;

                // Let the user know something is happening


                // Make the call to the hello resource asynchronously 
                var resultJson = await client.ServiceClient.InvokeApiAsync("hello", HttpMethod.Get, null);

                // Understanding color in iOS http://www.iosing.com/2011/11/uicolor-understanding-colour-in-ios/
                // A dark green: http://www.colorpicker.com/
                StatusLabel.BackgroundColor = UIColor.FromRGB(9, 125, 2);

                StatusLabel.Text = "Request completed!";

                // Verfiy that a result was returned
                if (resultJson.HasValues)
                {
                    // Extract the value from the result
                    string messageResult = resultJson.Value<string>("message");

                    // Set the text block with the result
                    OutputLabel.Text = messageResult;
                }
                else
                {
                    StatusLabel.TextColor = UIColor.Black;
                    StatusLabel.BackgroundColor = UIColor.Orange;
                    OutputLabel.Text = "Nothing returned!";
                }


            }
            catch (Exception ex)
            {
                // Display the exception message for the demo
                OutputLabel.Text = "";
                StatusLabel.Text = ex.Message;
                StatusLabel.BackgroundColor = UIColor.Red;

            }

            finally
            {
                // Let the user know the operation has completed
            }
        }

        async void CallAPIPostButton_TouchUpInside(object sender, EventArgs e)
        {
            try
            {
                StatusLabel.Text = "POST Request Made, waiting for response...";
                StatusLabel.TextColor = UIColor.White;
                StatusLabel.BackgroundColor = UIColor.Blue;

                // Let the user know something is happening


                // Create the json to send using an anonymous type 
                JToken payload = JObject.FromObject(new { msg = "hello joe" });
                // Make the call to the hello resource asynchronously using POST verb
                var resultJson = await client.ServiceClient.InvokeApiAsync("hello", payload);

                // Understanding color in iOS http://www.iosing.com/2011/11/uicolor-understanding-colour-in-ios/
                // A dark green: http://www.colorpicker.com/
                StatusLabel.BackgroundColor = UIColor.FromRGB(9, 125, 2);

                StatusLabel.Text = "Request completed!";

                // Verfiy that a result was returned
                if (resultJson.HasValues)
                {
                    // Extract the value from the result
                    string messageResult = resultJson.Value<string>("message");

                    // Set the text block with the result
                    OutputLabel.Text = messageResult;
                }
                else
                {
                    StatusLabel.TextColor = UIColor.Black;
                    StatusLabel.BackgroundColor = UIColor.Orange;
                    OutputLabel.Text = "Nothing returned!";
                }


            }
            catch (Exception ex)
            {
                // Display the exception message for the demo
                OutputLabel.Text = "";
                StatusLabel.Text = ex.Message;
                StatusLabel.BackgroundColor = UIColor.Red;

            }

            finally
            {
                // Let the user know the operaion has completed
            }
        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        #endregion
    }
}