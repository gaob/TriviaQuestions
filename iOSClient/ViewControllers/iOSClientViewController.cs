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
		// The default triviaQCount is 10.
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

			UpdateStatus(string.Empty, UIColor.White, UIColor.White);

			try
			{
	            // Perform any additional setup after loading the view, typically from a nib.
	            client = MobileServiceHelper.DefaultService;

				//Check if session exists.
				SQLiteHelper.Initialize ();
				var table = SQLiteHelper.db.Table<SessionItem> ();
				Debug.Assert (table.Count() < 2);

				// If session exists, resuming the game.
				if (table.Count() == 1) {
					UpdateStatus("Resuming the previous game...", UIColor.White, UIColor.Orange);

					// Obtain the playerid and session id.
					PlayerID = table.First ().playerid;
					SessionID = table.First ().sessionid;

					TEmail.Text = PlayerID;

					// Get all questions from local store.
					var question = from q in SQLiteHelper.db.Table<SessionQuestionItem> ()
							where (q.sessionid == SessionID)
						select q;

					if (question.Count() == 0) throw new Exception("Invalid code path!");

					TtriviaQCount.Text = question.Count().ToString();

					// Call playerprogress.
					var resultJson = await client.ServiceClient.InvokeApiAsync ("playerprogress", HttpMethod.Get, 
						new Dictionary<string, string>{{"playerid", PlayerID}, {"gamesessionid", SessionID}});

					//Check consistency between local store and the mobile service.
					string temp_id = string.Empty;
					string temp_ans = string.Empty;
					SessionQuestionItem SQItem = null;

					foreach (var item in resultJson)
					{
						if (item is JObject) {
							temp_id = item.Value<string> ("id");
							temp_ans = item.Value<string> ("proposedAnswer");
							SQItem = question.Where(x => x.questionid == temp_id).FirstOrDefault();
							// unanswered proposedAnswer in the server is "?", in local store, it could be "?" or "!".
							if (temp_ans == "?") {
								if (SQItem.proposedAnswer != "?" && SQItem.proposedAnswer != "!") {
									throw new Exception("Inconsistency!");
								}
							}
							// check if answered proposedAnswer is consistent.
							else if (SQItem.proposedAnswer != temp_ans) {
								throw new Exception("Inconsistency!");
							}
						} else {
							throw new Exception("Unexpected type in resultQuestions");
						}
					}

					// Start the game.
					QuestionViewController aViewController = this.Storyboard.InstantiateViewController("QuestionViewController") as QuestionViewController;
					if (aViewController != null) {
						UpdateStatus(string.Empty, UIColor.White, UIColor.White);
						this.NavigationController.PushViewController(aViewController, true);
					} else {
						StatusLabel.Text = "Start Game Error!";
					}
				}
				// If session doesn't exist, just set the default triviaQCount.
				else {
					TtriviaQCount.Text = triviaQCount.ToString();
				}
			}
			catch (Exception ex)
			{
				// Display the exception message
				UpdateStatus (ex.Message, UIColor.White, UIColor.Red);
			}
        }

		/// <summary>
		/// Resets the session.
		/// </summary>
		public static void ResetSession()
		{
			SessionID = string.Empty;
			PlayerID = string.Empty;

			SQLiteHelper.Initialize ();
		}

		/// <summary>
		/// Start Game button action.
		/// </summary>
		/// <param name="sender">Sender.</param>
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

				if (TEmail.Text.Length == 0) {
					throw new Exception("Invalid Email!");
				}

				// If email format is invalid, it will display an error.
				new System.Net.Mail.MailAddress(TEmail.Text);

				Questions = new List<QuestionItem>();

				// Retrieve triviaQCount questions.
				var resultQuestions = await client.ServiceClient.InvokeApiAsync ("triviaquestions", HttpMethod.Get, 
					new Dictionary<string, string>{{"triviaQCount", triviaQCount.ToString()}});

				// Verfiy that a result was returned
				if (resultQuestions.HasValues)
				{
					Debug.Assert(Questions.Count == 0);

					// Construct the questions from JArray.
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

				// If we allow to start a game, which means we don't have a session currently.
				if (SessionID == string.Empty) {
					// Construct JArray from retrieve questions.
					foreach (QuestionItem item in Questions) {
						temp = item.ToJToken();
						JQuestions.Add(temp);
					}

					payload = JObject.FromObject( new { playerid = TEmail.Text,
											            triviaIds = JQuestions });

					// Call start game session.
					var resultJson = await client.ServiceClient.InvokeApiAsync("startgamesession", payload);

					Debug.Assert(resultJson.Value<string>("playerid") == TEmail.Text);
					SessionID = resultJson.Value<string>("gamesessionid");

					// Insert player session into local store.
					SQLiteHelper.db.Insert(new SessionItem(SessionID, resultJson.Value<string>("playerid")));
					// Insert session questions into local store, and set the first question as "!" to be displayed.
					foreach (QuestionItem item in Questions) {
						tempSQ = new SessionQuestionItem();
						tempSQ.sessionid = SessionID;
						tempSQ.questionid = item.id;
						tempSQ.proposedAnswer = first_q ? "!" : "?";
						SQLiteHelper.db.Insert(tempSQ);

						first_q = false;
					}
				} else {
					throw new Exception("Invalid code path");
				}

				// Start the game.
				QuestionViewController aViewController = this.Storyboard.InstantiateViewController("QuestionViewController") as QuestionViewController;
				if (aViewController != null) {
					UpdateStatus(string.Empty, UIColor.White, UIColor.White);
					this.NavigationController.PushViewController(aViewController, true);
				} else {
					StatusLabel.Text = "Start Game Error!";
				}
			}
			catch (Exception ex)
			{
				// Display the exception message
				UpdateStatus (ex.Message, UIColor.White, UIColor.Red);
			}
		}

		/// <summary>
		/// High score button action.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void BViewHighScore_TouchUpInside (UIButton sender)
		{
			if (TEmail.Text.Length == 0) {
				TEmail.Text = "Please enter your Email";
				return;
			}

			HighScoresViewController.PlayerID = TEmail.Text;

			// Display the high scores.
			HighScoresViewController aViewController = this.Storyboard.InstantiateViewController("HighScoresViewController") as HighScoresViewController;
			if (aViewController != null) {
				this.NavigationController.PushViewController(aViewController, true);
			} else {
				StatusLabel.Text = "High Score Board Error!";
			}
		}

		/// <summary>
		/// Updates the status.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="tColor">T color.</param>
		/// <param name="bColor">B color.</param>
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