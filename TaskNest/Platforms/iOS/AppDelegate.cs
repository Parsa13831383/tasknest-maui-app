using Foundation;
using Microsoft.Maui.ApplicationModel;
using UIKit;

namespace TaskNest;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
	{
		if (Uri.TryCreate(url.AbsoluteString, UriKind.Absolute, out var uri) && global::Microsoft.Maui.Controls.Application.Current is App app)
		{
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				await app.HandleIncomingUrlAsync(uri);
			});

			return true;
		}

		return base.OpenUrl(application, url, options);
	}
}
