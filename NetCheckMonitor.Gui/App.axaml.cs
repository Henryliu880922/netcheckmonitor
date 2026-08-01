using System;
using System.Globalization;
using Avalonia;
using System.Resources;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Lang.Avalonia;
using Lang.Avalonia.Resx;
using NetCheckMonitor.Gui.Views;

namespace NetCheckMonitor.Gui;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var resourceManager = new ResourceManager(

            "NetCheckMonitor.Gui.I18n.Resources",

            typeof(App).Assembly);

        I18nManager.Instance.Register(

            new ResxLangPlugin(resourceManager),

            new CultureInfo("zh-Hant"),

            out string? error);
        var rm = new ResourceManager("NetCheckMonitor.Gui.I18n.Resources", typeof(App).Assembly);

        if (!string.IsNullOrWhiteSpace(error))
        {
            Console.Error.WriteLine(
                $"Localization initialization failed: {error}");
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is
            IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}