using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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

				var question = from q in SQLiteHelper.db.Table<SessionQuestionItem>()
						where (q.proposedAnswer == "!" && q.sessionid == SessionID)
					select q;

				Debug.Assert(question.Count() == 1);

				QuestionID = question.First().questionid;

				// Make the call to the hello resource asynchronously 
				var resultJson = await client.ServiceClient.InvokeApiAsync("triviaquestions/" + QuestionID, 
																			HttpMethod.Get, null);

				currQuestion = new QuestionItem(resultJson as JObject);

				TQText.Text = currQuestion.questionText;
				Bone.SetTitle(currQuestion.answerOne, UIControlState.Normal);
				Btwo.SetTitle(currQuestion.answerTwo, UIControlState.Normal);
				Bthree.SetTitle(currQuestion.answerThree, UIControlState.Normal);
				Bfour.SetTitle(currQuestion.answerFour, UIControlState.Normal);
			}
			catch (Exception ex)
			{
				// Display the exception message for the demo
				StatusLabel.Text = ex.Message;
				StatusLabel.BackgroundColor = UIColor.Red;
			}
		}
	}
}
