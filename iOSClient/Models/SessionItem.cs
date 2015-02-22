using System;
using SQLite;

namespace iOSClient
{
	[Table("SessionItem")]
	public class SessionItem
	{
		[PrimaryKey, Column("_id")]
		public string Id { get; set; }
		public string playerid { get; set; }

		public SessionItem (string theID, string thePlayerID)
		{
			Id = theID;
			playerid = thePlayerID;
		}

		public SessionItem()
		{
		}
	}
}
