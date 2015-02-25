using System;
using SQLite;

namespace iOSClient
{
	/// <summary>
	/// Session item in the local store.
	/// </summary>
	[Table("SessionItem")]
	public class SessionItem
	{
		[PrimaryKey]
		public string playerid { get; set; }
		public string sessionid { get; set; }

		public SessionItem (string theID, string thePlayerID)
		{
			playerid = thePlayerID;
			sessionid = theID;
		}

		public SessionItem()
		{
		}
	}
}
