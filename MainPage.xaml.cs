using Microsoft.Maui.Controls.PlatformConfiguration;

namespace DispatcherDemo;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    #region These work as expected

    private async void OnAwaitedTaskWithDispatcherButtonClicked(object sender, EventArgs e)
    {
        Console.WriteLine("***********************************************************************");
        Console.WriteLine("OnAwaitedTaskWithDispatcherButtonClicked");
        Console.WriteLine("***********************************************************************");
        Console.WriteLine("Dispatch should NOT yet be required.");
        Console.WriteLine($"IsDispatchRequired: {Dispatcher.IsDispatchRequired}");

        await Task.Run(async () =>
        {
            Console.WriteLine(
                "Running in a non-awaited Task - Dispatch SHOULD be required.");
            Console.WriteLine($"IsDispatchRequired: {Dispatcher.IsDispatchRequired}");

            Console.WriteLine(
                "Attempting to push a new page using Dispatcher...  No exceptions should be thrown and navigation should complete.");

            await Dispatcher.DispatchAsync(async () => { await navigateToNewPage(); });
        });
    }

    private async void OnMainThreadNoDispatcherButtonClicked(object sender, EventArgs e)
    {
        Console.WriteLine("****************************************************************************");
        Console.WriteLine("OnMainThreadNoDispatcherButtonClicked");
        Console.WriteLine("****************************************************************************");

        Console.WriteLine("Attempting to push a new page on the main thread WITHOUT Tasks/Dispatcher... No exceptions should be thrown and navigation should complete.");
        await navigateToNewPage();
    }

    #endregion

    #region These do not

    private async void OnAwaitedTaskNoDispatcherButtonClicked(object sender, EventArgs e)
    {
        Console.WriteLine("***********************************************************************");
        Console.WriteLine("OnAwaitedTaskNoDispatcherButtonClicked");
        Console.WriteLine("***********************************************************************");
        Console.WriteLine("Dispatch should NOT yet be required.");
        Console.WriteLine($"IsDispatchRequired: {Dispatcher.IsDispatchRequired}");

        await Task.Run(async () =>
        {
            Console.WriteLine(
                "Running in a non-awaited Task - Dispatch SHOULD be required.");
            Console.WriteLine($"IsDispatchRequired: {Dispatcher.IsDispatchRequired}");

            Console.WriteLine("Attempting to push a new page WITHOUT Dispatcher...  An AndroidRuntimeException should be thrown in Android and a UIKitThreadAccessException in iOS. Navigation is messed up going forward...");

#if ANDROID
            try
            {
                await navigateToNewPage();
            }
            catch (Android.Util.AndroidRuntimeException ex)
            {
                Console.WriteLine("AndroidRuntimeException WAS thrown in Android.");
                await Dispatcher.DispatchAsync(async () =>
                {
                    await DisplayAlert(ex.GetType().Name, ex.Message, "Thanks.");
                });
            }
#elif IOS
            try
            {
                await navigateToNewPage();
            }
            catch (UIKit.UIKitThreadAccessException ex)
            {
                Console.WriteLine("UIKitThreadAccessException WAS thrown in iOS.");
                await Dispatcher.DispatchAsync(async () =>
                {
                    await DisplayAlert(ex.GetType().Name, ex.Message, "Thanks.");
                });
            }
#endif
        });
    }

    private void OnNonAwaitedTaskNoDispatcherButtonClicked(object sender, EventArgs e)
    {
        Console.WriteLine("***********************************************************************");
        Console.WriteLine("OnNonAwaitedTaskNoDispatcherButtonClicked");
        Console.WriteLine("***********************************************************************");

        Console.WriteLine("Dispatch should NOT yet be required.");
        Console.WriteLine($"IsDispatchRequired: {Dispatcher.IsDispatchRequired}");

        _ = Task.Run(async () =>
        {
            Console.WriteLine(
                "Running in a non-awaited Task - Dispatch should be required.");
            Console.WriteLine($"IsDispatchRequired: {Dispatcher.IsDispatchRequired}");
        
            Console.WriteLine("Attempting to push a new page WITHOUT Dispatcher...  No exceptions seem to throw in Android, but a UIKitThreadAccessException is thrown in iOS. Navigation is messed up going forward...");

#if ANDROID
            try
            {
                await navigateToNewPage();
            }
            catch (Android.Util.AndroidRuntimeException ex)
            {
                Console.WriteLine("AndroidRuntimeException WAS thrown in Android.");
                await Dispatcher.DispatchAsync(async () =>
                {
                    await DisplayAlert(ex.GetType().Name, ex.Message, "Thanks.");
                });
            }
#elif IOS
            try
            {
                await navigateToNewPage();
            }
            catch (UIKit.UIKitThreadAccessException ex)
            {
                Console.WriteLine("UIKitThreadAccessException WAS thrown in iOS.");
                await Dispatcher.DispatchAsync(async () =>
                {
                    await DisplayAlert(ex.GetType().Name, ex.Message, "Thanks.");
                });
            }
#endif
        });
    }

    #endregion

    private async Task navigateToNewPage()
    {
        await Shell.Current.GoToAsync("/TestPage");
    }
}

