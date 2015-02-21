using System;
using Newtonsoft.Json.Linq;

namespace iOSClient
{
	public class QuestionItem
	{
		public string id { get; set;}
		public string questionText { get; set; }

		public string answerOne { get; set; }
		public string answerTwo { get; set; }
		public string answerThree { get; set; }
		public string answerFour { get; set; }

		public QuestionItem(JObject theObject)
		{
			id = theObject.Value<string> ("id");
			questionText = theObject.Value<string> ("questionText");
			answerOne = theObject.Value<string> ("answerOne");
			answerTwo = theObject.Value<string> ("answerTwo");
			answerThree = theObject.Value<string> ("answerThree");
			answerFour = theObject.Value<string> ("answerFour");
		}
	}
}
