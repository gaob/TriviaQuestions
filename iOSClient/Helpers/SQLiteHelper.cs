using System;
using SQLite;
using System.IO;

namespace iOSClient
{
	/// <summary>
	/// Encapsulate the SQLite into a helper class for local storage.
	/// </summary>
	public class SQLiteHelper
	{
		// The local path where the db is stored.
		public static string dbPath = Path.Combine (
			Environment.GetFolderPath (Environment.SpecialFolder.Personal),
			"ormdemo.db3");

		public static SQLiteConnection db = new SQLiteConnection (SQLiteHelper.dbPath);

		public SQLiteHelper ()
		{
		}

		/// <summary>
		/// Initialize this instance to make sure tables exist.
		/// </summary>
		public static void Initialize()
		{
			SQLiteHelper.CheckTable<SessionItem> (db);
			SQLiteHelper.CheckTable<SessionQuestionItem> (db);
		}

		/// <summary>
		/// Check if a table exists.
		/// </summary>
		/// <returns><c>true</c>, if exists was tabled, <c>false</c> otherwise.</returns>
		/// <param name="connection">Connection.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static bool TableExists<T> (SQLiteConnection connection)
		{    
			const string cmdText = "SELECT name FROM sqlite_master WHERE type='table' AND name=?";
			var cmd = connection.CreateCommand (cmdText, typeof(T).Name);
			return cmd.ExecuteScalar<string> () != null;
		}

		/// <summary>
		/// Create a table if it doesn't exist.
		/// </summary>
		/// <param name="connection">Connection.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void CheckTable<T> (SQLiteConnection connection)
		{
			if (!TableExists<T> (connection)) {
				Console.WriteLine ("Creating database, if it doesn't already exist");
				connection.CreateTable<T> ();
			}
		}
	}
}
