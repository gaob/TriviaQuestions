using System;
using Newtonsoft.Json.Linq;

namespace iOSClient
{
	/// <summary>
	/// Client structure to store a question.
	/// </summary>
	public class QuestionItem
	{
		public string id { get; set;}
		public string questionText { get; set; }

		public string answerOne { get; set; }
		public string answerTwo { get; set; }
		public string answerThree { get; set; }
		public string answerFour { get; set; }

		/// <summary>
		/// Constructor from a JObject.
		/// </summary>
		/// <param name="theObject">The object.</param>
		public QuestionItem(JObject theObject)
		{
			id = theObject.Value<string> ("id");
			questionText = theObject.Value<string> ("questionText");
			answerOne = theObject.Value<string> ("answerOne");
			answerTwo = theObject.Value<string> ("answerTwo");
			answerThree = theObject.Value<string> ("answerThree");
			answerFour = theObject.Value<string> ("answerFour");
		}

		/// <summary>
		/// Return a simple JToken
		/// </summary>
		/// <returns>The J token.</returns>
		public JToken ToJToken()
		{
			return JObject.FromObject (new { id = id});
		}
	}
}
