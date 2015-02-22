using System;
using SQLite;
using System.IO;

namespace iOSClient
{
	public class SQLiteHelper
	{
		public static string dbPath = Path.Combine (
			Environment.GetFolderPath (Environment.SpecialFolder.Personal),
			"ormdemo.db3");

		public static SQLiteConnection db = new SQLiteConnection (SQLiteHelper.dbPath);

		public SQLiteHelper ()
		{
		}

		public static void Initialize()
		{
			SQLiteHelper.CheckTable<SessionItem> (db);
			SQLiteHelper.CheckTable<SessionQuestionItem> (db);
		}

		public static bool TableExists<T> (SQLiteConnection connection)
		{    
			const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
			var cmd = connection.CreateCommand (cmdText, typeof(T).Name);
			return cmd.ExecuteScalar<string> () != null;
		}

		public static void CheckTable<T> (SQLiteConnection connection)
		{
			if (!TableExists<T> (connection)) {
				Console.WriteLine ("Creating database, if it doesn't already exist");
				connection.CreateTable<T> ();
			}
		}
	}
}
