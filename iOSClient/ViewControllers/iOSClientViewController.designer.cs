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
	[Register ("iOSClientViewController")]
	partial class iOSClientViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Bstart { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton BViewHighScore { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel LEmail { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel LtriviaQCount { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel StatusLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField TEmail { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField TtriviaQCount { get; set; }

		[Action ("Bstart_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void Bstart_TouchUpInside (UIButton sender);

		[Action ("BViewHighScore_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void BViewHighScore_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (Bstart != null) {
				Bstart.Dispose ();
				Bstart = null;
			}
			if (BViewHighScore != null) {
				BViewHighScore.Dispose ();
				BViewHighScore = null;
			}
			if (LEmail != null) {
				LEmail.Dispose ();
				LEmail = null;
			}
			if (LtriviaQCount != null) {
				LtriviaQCount.Dispose ();
				LtriviaQCount = null;
			}
			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}
			if (TEmail != null) {
				TEmail.Dispose ();
				TEmail = null;
			}
			if (TtriviaQCount != null) {
				TtriviaQCount.Dispose ();
				TtriviaQCount = null;
			}
		}
	}
}
