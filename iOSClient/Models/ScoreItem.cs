
using System;
using Newtonsoft.Json.Linq;

namespace iOSClient
{
	/// <summary>
	/// Client structure to store a score.
	/// </summary>
	public class ScoreItem
	{
		public int score { get; set;}
		public DateTime occurred { get; set;}

		/// <summary>
		/// Constructor from a JObject.
		/// </summary>
		/// <param name="theObject">The object.</param>
		public ScoreItem (JObject theObject)
		{
			score = theObject.Value<int> ("score");
			occurred = theObject.Value<DateTime> ("occurred");
		}

		/// <summary>
		/// Used to display the score in the HighScoresTable.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="iOSClient.ScoreItem"/>.</returns>
		public override string ToString ()
		{
			return score.ToString()+"\t\t\t\t\t\t\t"+occurred.ToShortDateString();
		}
	}
}
