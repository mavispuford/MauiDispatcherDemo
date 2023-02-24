using System.Diagnostics;
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
        Debug.WriteLine("***********************************************************************");
        Debug.WriteLine("* OnAwaitedTaskWithDispatcherButtonClicked");
        Debug.WriteLine("***********************************************************************");
        Debug.WriteLine("* Dispatch should NOT yet be required.");
        Debug.WriteLine($"* IsDispatchRequired: {Dispatcher.IsDispatchRequired}");

        await Task.Run(async () =>
        {
            Debug.WriteLine(
                "* Running in a non-awaited Task - Dispatch SHOULD be required.");
            Debug.WriteLine($"* IsDispatchRequired: {Dispatcher.IsDispatchRequired}");

            Debug.WriteLine(
                "* Attempting to push a new page using Dispatcher...  No exceptions should be thrown and navigation should complete.");

            await Dispatcher.DispatchAsync(async () => { await navigateToNewPage(); });
        });
    }

    private async void OnMainThreadNoDispatcherButtonClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("****************************************************************************");
        Debug.WriteLine("* OnMainThreadNoDispatcherButtonClicked");
        Debug.WriteLine("****************************************************************************");

        Debug.WriteLine("* Attempting to push a new page on the main thread WITHOUT Tasks/Dispatcher... No exceptions should be thrown and navigation should complete.");
        await navigateToNewPage();
    }

    #endregion

    #region These do not

    private async void OnAwaitedTaskNoDispatcherButtonClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("***********************************************************************");
        Debug.WriteLine("* OnAwaitedTaskNoDispatcherButtonClicked");
        Debug.WriteLine("***********************************************************************");
        Debug.WriteLine("* Dispatch should NOT yet be required.");
        Debug.WriteLine($"* IsDispatchRequired: {Dispatcher.IsDispatchRequired}");

        await Task.Run(async () =>
        {
            Debug.WriteLine(
                "* Running in a non-awaited Task - Dispatch SHOULD be required.");
            Debug.WriteLine($"* IsDispatchRequired: {Dispatcher.IsDispatchRequired}");

            Debug.WriteLine("* Attempting to push a new page WITHOUT Dispatcher...  An AndroidRuntimeException should be thrown in Android and a UIKitThreadAccessException in iOS. Navigation is messed up going forward...");

            await navigateToNewPage();
        });
    }

    private void OnNonAwaitedTaskNoDispatcherButtonClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("***********************************************************************");
        Debug.WriteLine("* OnNonAwaitedTaskNoDispatcherButtonClicked");
        Debug.WriteLine("***********************************************************************");

        Debug.WriteLine("* Dispatch should NOT yet be required.");
        Debug.WriteLine($"* IsDispatchRequired: {Dispatcher.IsDispatchRequired}");

        _ = Task.Run(async () =>
        {
            Debug.WriteLine(
                "* Running in a non-awaited Task - Dispatch should be required.");
            Debug.WriteLine($"* IsDispatchRequired: {Dispatcher.IsDispatchRequired}");
        
            Debug.WriteLine("* Attempting to push a new page WITHOUT Dispatcher...  No exceptions seem to throw in Android, but a UIKitThreadAccessException is thrown in iOS. Navigation is messed up going forward...");

            await navigateToNewPage();
        });
    }

    #endregion

    private async Task navigateToNewPage()
    {
        try
        {
            await Shell.Current.GoToAsync("/TestPage");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"* {ex.GetType().Name} was thrown in {DeviceInfo.Current.Platform}.");
            await Dispatcher.DispatchAsync(async () =>
            {
                await DisplayAlert(ex.GetType().Name, ex.Message, "Thanks.");
            });
        }
        
    }
}

