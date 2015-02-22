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
		UIButton CallAPIGetButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton CallAPIPostButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel LEmail { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel OutputLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel StatusLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField TEmail { get; set; }

		[Action ("Bstart_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void Bstart_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (Bstart != null) {
				Bstart.Dispose ();
				Bstart = null;
			}
			if (CallAPIGetButton != null) {
				CallAPIGetButton.Dispose ();
				CallAPIGetButton = null;
			}
			if (CallAPIPostButton != null) {
				CallAPIPostButton.Dispose ();
				CallAPIPostButton = null;
			}
			if (LEmail != null) {
				LEmail.Dispose ();
				LEmail = null;
			}
			if (OutputLabel != null) {
				OutputLabel.Dispose ();
				OutputLabel = null;
			}
			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}
			if (TEmail != null) {
				TEmail.Dispose ();
				TEmail = null;
			}
		}
	}
}
