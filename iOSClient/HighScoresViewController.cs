using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;
using System.Net.Http;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace iOSClient
{
	partial class HighScoresViewController : UITableViewController
	{
		private MobileServiceHelper client;
		public static string PlayerID = string.Empty;
		private List<ScoreItem> SItems = new List<ScoreItem>();

		public HighScoresViewController (IntPtr handle) : base (handle)
		{
		}

		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			try
			{
				client = MobileServiceHelper.DefaultService;

				Debug.Assert(PlayerID.Length > 0);

				var resultJson = await client.ServiceClient.InvokeApiAsync ("highscore", HttpMethod.Get, new Dictionary<string, string>{{"playerid", PlayerID}});

				foreach (var item in resultJson) {
					if (item is JObject) {
						SItems.Add(new ScoreItem(item as JObject));
					} else {
						throw new Exception("Unexpected type in resultJson");
					}
				}
					
				string[] tableItems = new string[SItems.Count];

				for (int i=0;i<tableItems.Length;i++) {
					tableItems[i] = SItems[i].ToString();
				}
				HighScoresTable.Source = new TableSource(tableItems);
				HighScoresTable.ReloadData();
			}
			catch (Exception ex)
			{
				Title = ex.Message;
			}
		}

		public class TableSource : UITableViewSource {
			string[] tableItems;
			string cellIdentifier = "TableCell";
			public TableSource (string[] items)
			{
				tableItems = items;
			}
			public override int RowsInSection (UITableView tableview, int section)
			{
				return tableItems.Length;
			}
			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
				// if there are no cells to reuse, create a new one
				if (cell == null)
					cell = new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier);
				cell.TextLabel.Text = tableItems[indexPath.Row];
				return cell;
			}
		}
	}
}
