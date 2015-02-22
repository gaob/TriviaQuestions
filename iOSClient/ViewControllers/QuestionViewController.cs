using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace iOSClient
{
	partial class QuestionViewController : UIViewController
	{
		private MobileServiceHelper client;
		private string PlayerID = string.Empty;
		private string SessionID = string.Empty;
		private string QuestionID = string.Empty;
		private QuestionItem currQuestion = null;

		public QuestionViewController (IntPtr handle) : base (handle)
		{
		}

		public async override void ViewDidLoad()
		{
			base.ViewDidLoad();

			try
			{
				// Perform any additional setup after loading the view, typically from a nib.
				client = MobileServiceHelper.DefaultService;

				//Check if session exists.
				var table = SQLiteHelper.db.Table<SessionItem> ();
				Debug.Assert (table.Count() == 1);

				PlayerID = table.First ().playerid;
				SessionID = table.First ().sessionid;

				await DisplayNextQuestion ();
			}
			catch (Exception ex)
			{
				// Display the exception message for the demo
				StatusLabel.Text = ex.Message;
				StatusLabel.BackgroundColor = UIColor.Red;
			}
		}

		async Task DisplayNextQuestion ()
		{
			var question = from q in SQLiteHelper.db.Table<SessionQuestionItem> ()
			where (q.proposedAnswer == "!" && q.sessionid == SessionID)
			select q;
			Debug.Assert (question.Count () == 1);
			QuestionID = question.First ().questionid;

			// Make the call to the hello resource asynchronously 
			var resultJson = await client.ServiceClient.InvokeApiAsync ("triviaquestions/" + QuestionID, HttpMethod.Get, null);
			currQuestion = new QuestionItem (resultJson as JObject);
			TQText.Text = currQuestion.questionText;
			Bone.SetTitle (currQuestion.answerOne, UIControlState.Normal);
			Btwo.SetTitle (currQuestion.answerTwo, UIControlState.Normal);
			Bthree.SetTitle (currQuestion.answerThree, UIControlState.Normal);
			Bfour.SetTitle (currQuestion.answerFour, UIControlState.Normal);
		}

		partial void Bone_TouchUpInside (UIButton sender)
		{
			ProposeAnswer("1");
		}

		partial void Btwo_TouchUpInside (UIButton sender)
		{
			ProposeAnswer("2");
		}

		partial void Bthree_TouchUpInside (UIButton sender)
		{
			ProposeAnswer("3");
		}

		partial void Bfour_TouchUpInside (UIButton sender)
		{
			ProposeAnswer("4");
		}

		async void ProposeAnswer (string theProposedAnswer)
		{
			try
			{
				StatusLabel.Text = "Checking Answer...";
				StatusLabel.TextColor = UIColor.White;
				StatusLabel.BackgroundColor = UIColor.Blue;

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
				// Make the call to the hello resource asynchronously using POST verb
				var resultJson = await client.ServiceClient.InvokeApiAsync("playerprogress", payload, new HttpMethod("PATCH"), null);

				if (resultJson.HasValues)
				{
					// Extract the value from the result
					string messageResult = resultJson.Value<string>("answerEvaluation");

					UIButton theButton = getButtonFrom(theProposedAnswer);

					if (messageResult == "correct") {
						theButton.SetTitleColor(UIColor.Green, UIControlState.Normal);
					} else if (messageResult == "incorrect") {
						theButton.SetTitleColor(UIColor.Red, UIControlState.Normal);
					} else {
						throw new Exception("Invalid path!");
					}

					StatusLabel.Text = "Click anywhere to the next question";
					Bcover.Hidden = false;
				}
				else
				{
					StatusLabel.TextColor = UIColor.Black;
					StatusLabel.BackgroundColor = UIColor.Orange;
					StatusLabel.Text = "Nothing returned!";
				}
			}
			catch (Exception ex)
			{
				StatusLabel.Text = ex.Message;
				StatusLabel.BackgroundColor = UIColor.Red;
			}
		}

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

		async partial void Bcover_TouchUpInside (UIButton sender)
		{
			var question = from q in SQLiteHelper.db.Table<SessionQuestionItem>()
					where (q.proposedAnswer == "?" && q.sessionid == SessionID)
				select q;

			if (question.Count() == 0) {
				//Go to highscore.
			} else {
				var question2update = question.FirstOrDefault();
				question2update.proposedAnswer = "!";
				SQLiteHelper.db.Update(question2update);
				StatusLabel.Text = "Displaying Next Question";
				await DisplayNextQuestion();
				StatusLabel.Text = "Done";

				Bcover.Hidden = true;
			}
		}
	}
}
