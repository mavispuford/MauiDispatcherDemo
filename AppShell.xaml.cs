namespace DispatcherDemo;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("TestPage", typeof(TestPage));
	}
}
