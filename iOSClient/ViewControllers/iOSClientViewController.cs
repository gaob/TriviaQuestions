using System;
using System.Drawing;
using System.Net.Http;
using Microsoft.WindowsAzure.MobileServices;
using MonoTouch.CoreGraphics;
using MonoTouch.CoreImage;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using SQLite;
using System.Diagnostics;

namespace iOSClient
{
    public partial class iOSClientViewController : UIViewController
    {
		List<QuestionItem> Questions = new List<QuestionItem> ();
		private string SessionID = string.Empty;
		private string PlayerID = string.Empty;
		private int triviaQCount = 10;

		private MobileServiceHelper client;

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

        #region View lifecycle

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

			try
			{
	            // Perform any additional setup after loading the view, typically from a nib.
	            client = MobileServiceHelper.DefaultService;

				//Check if session exists.
				SQLiteHelper.Initialize ();
				var table = SQLiteHelper.db.Table<SessionItem> ();
				Debug.Assert (table.Count() < 2);

				if (table.Count() == 1) {
					PlayerID = table.First ().playerid;
					SessionID = table.First ().sessionid;

					TEmail.Text = PlayerID;

					var resultJson = await client.ServiceClient.InvokeApiAsync ("playerprogress", HttpMethod.Get, 
						new Dictionary<string, string>{{"playerid", PlayerID}, {"gamesessionid", SessionID}});

					//Check if need to use resultJson
				} else {
					TtriviaQCount.Text = triviaQCount.ToString();
				}
			}
			catch (Exception ex)
			{
				// Display the exception message for the demo
				StatusLabel.Text = ex.Message;
				StatusLabel.BackgroundColor = UIColor.Red;
			}

			CallAPIGetButton.TouchUpInside += CallAPIGetButton_TouchUpInside;
			CallAPIPostButton.TouchUpInside += CallAPIPostButton_TouchUpInside;
        }

		async partial void Bstart_TouchUpInside (UIButton sender)
		{
			// Create the json to send using an anonymous type 
			JToken payload;

			JArray JQuestions = new JArray();
			JToken temp;
			SessionQuestionItem tempSQ;
			bool first_q = true;

			try
			{
				var resultQuestions = await client.ServiceClient.InvokeApiAsync ("triviaquestions/" + triviaQCount.ToString(), HttpMethod.Get, null);
				// Verfiy that a result was returned
				if (resultQuestions.HasValues)
				{
					Debug.Assert(Questions.Count == 0);

					foreach (var item in resultQuestions)
					{
						if (item is JObject) {
							Questions.Add(new QuestionItem(item as JObject));
						} else {
							throw new Exception("Unexpected type in resultQuestions");
						}
					}

					// Set the text block with the result
					OutputLabel.Text = Questions.Count.ToString();
				}
				else
				{
					throw new Exception("Nothing returned!");
				}

				if (SessionID == string.Empty) {
					foreach (QuestionItem item in Questions) {
						temp = item.ToJToken();
						JQuestions.Add(temp);
					}

					payload = JObject.FromObject( new { playerid = TEmail.Text,
											            triviaIds = JQuestions });

					var resultJson = await client.ServiceClient.InvokeApiAsync("startgamesession", payload);

					Debug.Assert(resultJson.Value<string>("playerid") == TEmail.Text);
					SessionID = resultJson.Value<string>("gamesessionid");

					SQLiteHelper.db.Insert(new SessionItem(SessionID, resultJson.Value<string>("playerid")));
					foreach (QuestionItem item in Questions) {
						tempSQ = new SessionQuestionItem();
						tempSQ.sessionid = SessionID;
						tempSQ.questionid = item.id;
						tempSQ.proposedAnswer = first_q ? "!" : "?";
						SQLiteHelper.db.Insert(tempSQ);

						first_q = false;
					}
				} else {
					throw new NotImplementedException();
				}

				QuestionViewController aViewController = this.Storyboard.InstantiateViewController("QuestionViewController") as QuestionViewController;
				if (aViewController != null) {
					this.NavigationController.PushViewController(aViewController, true);
				} else {
					StatusLabel.Text = "Start Game Board Error!";
				}
			}
			catch (Exception ex)
			{
				// Display the exception message for the demo
				StatusLabel.Text = ex.Message;
				StatusLabel.BackgroundColor = UIColor.Red;
			}
		}

		partial void BViewHighScore_TouchUpInside (UIButton sender)
		{
			if (TEmail.Text.Length == 0) {
				TEmail.Text = "Please enter your Email";
			}

			HighScoresViewController.PlayerID = TEmail.Text;
			HighScoresViewController aViewController = this.Storyboard.InstantiateViewController("HighScoresViewController") as HighScoresViewController;
			if (aViewController != null) {
				this.NavigationController.PushViewController(aViewController, true);
			} else {
				StatusLabel.Text = "Start Game Board Error!";
			}
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
                var resultJson = await client.ServiceClient.InvokeApiAsync("triviaquestions", HttpMethod.Get, null);

                // Understanding color in iOS http://www.iosing.com/2011/11/uicolor-understanding-colour-in-ios/
                // A dark green: http://www.colorpicker.com/
                StatusLabel.BackgroundColor = UIColor.FromRGB(9, 125, 2);

                StatusLabel.Text = "Request completed!";

                // Verfiy that a result was returned
                if (resultJson.HasValues)
                {
					foreach (var item in resultJson)
					{
						if (item is JObject) {
							Questions.Add(new QuestionItem(item as JObject));
						} else {
							throw new Exception("Unexpected type in resultJson");
						}
					}

					// Extract the value from the result
					string messageResult = Questions.Count.ToString();

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