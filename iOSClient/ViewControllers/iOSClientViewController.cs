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
		private static string SessionID = string.Empty;
		private static string PlayerID = string.Empty;
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
				// Display the exception message
				UpdateStatus (ex.Message, UIColor.White, UIColor.Red);
			}
        }

		public static void ResetSession()
		{
			SessionID = string.Empty;
			PlayerID = string.Empty;

			SQLiteHelper.Initialize ();
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
				triviaQCount = int.Parse(TtriviaQCount.Text);

				if (triviaQCount <= 0 || triviaQCount > 30) {
					throw new Exception("Invalid triviaQCount!");
				}

				Questions = new List<QuestionItem>();

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
					StatusLabel.Text = "Start Game Error!";
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
				return;
			}

			HighScoresViewController.PlayerID = TEmail.Text;
			HighScoresViewController aViewController = this.Storyboard.InstantiateViewController("HighScoresViewController") as HighScoresViewController;
			if (aViewController != null) {
				this.NavigationController.PushViewController(aViewController, true);
			} else {
				StatusLabel.Text = "High Score Board Error!";
			}
		}

		void UpdateStatus (string text, UIColor tColor, UIColor bColor)
		{
			StatusLabel.Text = text;
			StatusLabel.TextColor = tColor;
			StatusLabel.BackgroundColor = bColor;
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