using System;
using SQLite;

namespace iOSClient
{
	[Table("SessionQuestionItem")]
	public class SessionQuestionItem
	{
		[PrimaryKey, AutoIncrement]
		public int id { get; set; }
		public string sessionid { get; set; }
		public string questionid { get; set; }
		public string proposedAnswer { get; set; }

		public SessionQuestionItem ()
		{
		}
	}
}
