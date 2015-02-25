using System;
using SQLite;

namespace iOSClient
{
	/// <summary>
	/// Session question item in the local store.
	/// </summary>
	[Table("SessionQuestionItem")]
	public class SessionQuestionItem
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		public string sessionid { get; set; }
		public string questionid { get; set; }
		// "1", "2", "3", "4" indicates the answer, "?" means this question hasn't been answered.
		// local store could also have a value of "!", it means "?" and it's the last unanswered question from game session.
		// There will only be exact 1 "!" proposedAnswer in the local store for each session.
		public string proposedAnswer { get; set; }
	}
}
