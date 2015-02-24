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
	[Register ("QuestionViewController")]
	partial class QuestionViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Bcover { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Bfour { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Bone { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton BQuit { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Bthree { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton Btwo { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel StatusLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel TQText { get; set; }

		[Action ("Bcover_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void Bcover_TouchUpInside (UIButton sender);

		[Action ("Bfour_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void Bfour_TouchUpInside (UIButton sender);

		[Action ("Bone_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void Bone_TouchUpInside (UIButton sender);

		[Action ("BQuit_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void BQuit_TouchUpInside (UIButton sender);

		[Action ("Bthree_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void Bthree_TouchUpInside (UIButton sender);

		[Action ("Btwo_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void Btwo_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (Bcover != null) {
				Bcover.Dispose ();
				Bcover = null;
			}
			if (Bfour != null) {
				Bfour.Dispose ();
				Bfour = null;
			}
			if (Bone != null) {
				Bone.Dispose ();
				Bone = null;
			}
			if (BQuit != null) {
				BQuit.Dispose ();
				BQuit = null;
			}
			if (Bthree != null) {
				Bthree.Dispose ();
				Bthree = null;
			}
			if (Btwo != null) {
				Btwo.Dispose ();
				Btwo = null;
			}
			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}
			if (TQText != null) {
				TQText.Dispose ();
				TQText = null;
			}
		}
	}
}
