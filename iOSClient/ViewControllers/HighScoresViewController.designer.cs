// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;

namespace iOSClient
{
	[Register ("HighScoresViewController")]
	partial class HighScoresViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView HighScoresTable { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (HighScoresTable != null) {
				HighScoresTable.Dispose ();
				HighScoresTable = null;
			}
		}
	}
}
