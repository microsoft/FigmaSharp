using FigmaSharp.Maui;

namespace Rendering;

public partial class App : Application
{
    public App()
    {
        FigmaApplication.Init(Environment.GetEnvironmentVariable("TOKEN"));
        InitializeComponent();
        MainPage = new AppShell();
    }
}

