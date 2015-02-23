
using System;
using Newtonsoft.Json.Linq;

namespace iOSClient
{
	public class ScoreItem
	{
		public int score { get; set;}
		public DateTime occurred { get; set;}

		public ScoreItem (JObject theObject)
		{
			score = theObject.Value<int> ("score");
			occurred = theObject.Value<DateTime> ("occurred");
		}

		public override string ToString ()
		{
			return score.ToString()+"\t\t\t\t\t\t\t"+occurred.ToShortDateString();
		}
	}
}
