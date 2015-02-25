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
		public string proposedAnswer { get; set; }
	}
}
