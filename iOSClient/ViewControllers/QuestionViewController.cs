using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace iOSClient
{
	partial class QuestionViewController : UIViewController
	{
		private MobileServiceHelper client;
		private string PlayerID = string.Empty;
		private string SessionID = string.Empty;
		private string QuestionID = string.Empty;
		private QuestionItem currQuestion = null;
		private bool Finished = false;
		private Timer timer;

		public QuestionViewController (IntPtr handle) : base (handle)
		{
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Hides back button, quit game is handled by the quit game button.
			this.NavigationItem.SetHidesBackButton (true, false);

			try
			{
				// Perform any additional setup after loading the view, typically from a nib.
				client = MobileServiceHelper.DefaultService;

				// Prepare the timer for displaying the next question.
				timer = new Timer();
				timer.Interval = 2000;
				timer.Elapsed += OnTimedEvent;

				//Check if session exists.
				var table = SQLiteHelper.db.Table<SessionItem> ();
				Debug.Assert (table.Count() == 1);

				PlayerID = table.First ().playerid;
				SessionID = table.First ().sessionid;

				await DisplayNextQuestion ();
			}
			catch (Exception ex)
			{
				// Display the exception message
				UpdateStatus (ex.Message, UIColor.White, UIColor.Red);
			}

			// Display the quit game button.
			BQuit.Hidden = false;
			BQuit.SetTitleColor(UIColor.Blue, UIControlState.Normal);
		}

		/// <summary>
		/// Timer triggerred action.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
		{
			timer.Stop ();

			//Update visual representation here
			InvokeOnMainThread(async () => {
				StatusLabel.Text = "Timer triggered";

				// Get unanswered questions.
				var question = from q in SQLiteHelper.db.Table<SessionQuestionItem>()
						where (q.proposedAnswer == "?" && q.sessionid == SessionID)
					select q;

				// No unanswered questions, end game.
				if (question.Count() == 0) {
					await EndGameSession(PlayerID, SessionID);
				}
				// Otherwise, display the next question.
				else {
					var question2update = question.FirstOrDefault();
					question2update.proposedAnswer = "!";
					SQLiteHelper.db.Update(question2update);
					await DisplayNextQuestion();

					Bcover.Hidden = true;
				}
			});
		}

		/// <summary>
		/// Displaies the next question.
		/// </summary>
		/// <returns>The next question.</returns>
		async Task DisplayNextQuestion ()
		{
			UpdateStatus ("Retrieve question...", UIColor.White, UIColor.Orange);

			// Get the question set to be displayed.
			var question = from q in SQLiteHelper.db.Table<SessionQuestionItem> ()
			where (q.proposedAnswer == "!" && q.sessionid == SessionID)
			select q;
			Debug.Assert (question.Count () == 1);
			QuestionID = question.First ().questionid;

			// Make the call to get the question details. 
			var resultJson = await client.ServiceClient.InvokeApiAsync ("triviaquestions/" + QuestionID, HttpMethod.Get, null);
			currQuestion = new QuestionItem (resultJson as JObject);
			TQText.Text = currQuestion.questionText;
			Bone.SetTitle (currQuestion.answerOne, UIControlState.Normal);
			Bone.SetTitleColor(UIColor.Blue, UIControlState.Normal);
			Btwo.SetTitle (currQuestion.answerTwo, UIControlState.Normal);
			Btwo.SetTitleColor(UIColor.Blue, UIControlState.Normal);
			Bthree.SetTitle (currQuestion.answerThree, UIControlState.Normal);
			Bthree.SetTitleColor(UIColor.Blue, UIControlState.Normal);
			Bfour.SetTitle (currQuestion.answerFour, UIControlState.Normal);
			Bfour.SetTitleColor(UIColor.Blue, UIControlState.Normal);

			UpdateStatus ("Please select your answer", UIColor.White, UIColor.Blue);
		}

		/// <summary>
		/// Answer 1 action.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void Bone_TouchUpInside (UIButton sender)
		{
			ProposeAnswer("1");
		}

		/// <summary>
		/// Answer 2 action.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void Btwo_TouchUpInside (UIButton sender)
		{
			ProposeAnswer("2");
		}

		/// <summary>
		/// Answer 3 action.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void Bthree_TouchUpInside (UIButton sender)
		{
			ProposeAnswer("3");
		}

		/// <summary>
		/// Answer 4 action.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void Bfour_TouchUpInside (UIButton sender)
		{
			ProposeAnswer("4");
		}

		/// <summary>
		/// Proposes the answer.
		/// </summary>
		/// <param name="theProposedAnswer">The proposed answer.</param>
		async void ProposeAnswer (string theProposedAnswer)
		{
			try
			{
				UpdateStatus("Checking Answer...", UIColor.White, UIColor.Blue);

				//Update internal proposedAnswer field.
				var question = from q in SQLiteHelper.db.Table<SessionQuestionItem>()
						where (q.proposedAnswer == "!" && q.sessionid == SessionID)
					select q;
				Debug.Assert(question.Count() == 1);
				var question2update = question.FirstOrDefault();
				question2update.proposedAnswer = theProposedAnswer;
				SQLiteHelper.db.Update(question2update);

				// Create the json to send using an anonymous type 
				JToken payload = JObject.FromObject(new { playerid = PlayerID,
														  gamesessionid = SessionID, id = QuestionID, proposedAnswer = theProposedAnswer});
				// Make the call to get the correctness.
				var resultJson = await client.ServiceClient.InvokeApiAsync("playerprogress", payload, new HttpMethod("PATCH"), null);

				if (resultJson.HasValues)
				{
					// Extract the value from the result
					string messageResult = resultJson.Value<string>("answerEvaluation");

					UIButton theButton = getButtonFrom(theProposedAnswer);

					// Green means correct selection, Red means incorrect selection.
					if (messageResult == "correct") {
						theButton.SetTitleColor(UIColor.Green, UIControlState.Normal);
					} else if (messageResult == "incorrect") {
						theButton.SetTitleColor(UIColor.Red, UIControlState.Normal);
					} else {
						throw new Exception("Invalid path!");
					}
						
					UpdateStatus("Click anywhere to the next question", UIColor.Blue, UIColor.Green);
					Bcover.Hidden = false;

					// Start the 2 second countdown to display the next question.
					timer.Enabled = true;
				}
				else
				{
					UpdateStatus("Nothing returned!", UIColor.Black, UIColor.Orange);
				}
			}
			catch (Exception ex)
			{
				// Display the exception message
				UpdateStatus (ex.Message, UIColor.White, UIColor.Red);
			}
		}

		/// <summary>
		/// Convert the proposed answer string to button name.
		/// </summary>
		/// <returns>The button from.</returns>
		/// <param name="theProposedAnswer">The proposed answer.</param>
		private UIButton getButtonFrom(string theProposedAnswer)
		{
			switch (theProposedAnswer) {
			case "1":
				return Bone;
			case "2":
				return Btwo;
			case "3":
				return Bthree;
			case "4":
				return Bfour;
			default:
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Bcover action, used to advance from the current question.
		/// </summary>
		/// <param name="sender">Sender.</param>
		async partial void Bcover_TouchUpInside (UIButton sender)
		{
			// get unanswered questions.
			var question = from q in SQLiteHelper.db.Table<SessionQuestionItem>()
					where (q.proposedAnswer == "?" && q.sessionid == SessionID)
				select q;

			// If score displayed, go back.
			if (Finished) {
				this.NavigationController.PopToRootViewController(true);
			}
			// No unanswered questions, end game.
			else if (question.Count() == 0) {
				await EndGameSession(PlayerID, SessionID);
			}
			// Otherwisse, display the next question.
			else {
				var question2update = question.FirstOrDefault();
				question2update.proposedAnswer = "!";
				SQLiteHelper.db.Update(question2update);
				await DisplayNextQuestion();

				Bcover.Hidden = true;
			}

			// If player click to advance to the next question, disable the timer.
			timer.Enabled = false;
		}

		/// <summary>
		/// Quit Game button action.
		/// </summary>
		/// <param name="sender">Sender.</param>
		async partial void BQuit_TouchUpInside (UIButton sender)
		{
			await EndGameSession(PlayerID, SessionID);
			this.NavigationController.PopToRootViewController(false);
		}

		/// <summary>
		/// Ends the game session.
		/// </summary>
		/// <returns>The game session.</returns>
		/// <param name="playerID">Player I.</param>
		/// <param name="sessionID">Session I.</param>
		async Task EndGameSession (string playerID, string sessionID)
		{
			try
			{
				UpdateStatus("Ending Game...", UIColor.White, UIColor.Orange);

				JToken payload = JObject.FromObject( new { playerid = playerID, gamesessionid = sessionID });

				// Call endgamesession.
				var resultJson = await client.ServiceClient.InvokeApiAsync("endgamesession", payload);

				if (resultJson.HasValues)
				{
					// Extract the value from the result
					int score = resultJson.Value<int>("score");

					//If returned score is -1, means the player quits the game before it ends.
					if (score != -1) {
						int highscorebeat = resultJson.Value<int>("highscorebeat");

						string highscorebeat_text = string.Empty;

						// Construct message based on highscorebeat.
						if (highscorebeat != -1) {
							if (highscorebeat == 0) {
								highscorebeat_text = "\nNew High Score!";
							} else if (highscorebeat == 1) {
								highscorebeat_text = "\nCongratulations you beat your highest score!";
							} else if (highscorebeat == 2) {
								highscorebeat_text = "\nCongratulations you beat your 2nd high score!";
							} else if (highscorebeat == 3) {
								highscorebeat_text = "\nCongratulations you beat your 3rd high score!";
							} else {
								highscorebeat_text = "\nCongratulations you beat your " + highscorebeat + "th high score!";
							}
						}

						TQText.Text = "Total Score: " + score.ToString() + highscorebeat_text;
					}

					// Clear the session data.
					SQLiteHelper.db.DropTable<SessionItem>();
					SQLiteHelper.db.DropTable<SessionQuestionItem>();
					iOSClientViewController.ResetSession();

					// Marked as finished.
					Finished = true;

					Bone.SetTitle (string.Empty, UIControlState.Normal);
					Btwo.SetTitle (string.Empty, UIControlState.Normal);
					Bthree.SetTitle (string.Empty, UIControlState.Normal);
					Bfour.SetTitle (string.Empty, UIControlState.Normal);
					BQuit.Hidden = true;
					Bcover.Hidden = false;
					UpdateStatus("Click anywhere to go back", UIColor.Blue, UIColor.Green);
				}
				else
				{
					UpdateStatus("Nothing returned!", UIColor.Black, UIColor.Orange);
				}
			}
			catch (Exception ex)
			{
				// Display the exception message
				UpdateStatus (ex.Message, UIColor.White, UIColor.Red);
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
	}
}
