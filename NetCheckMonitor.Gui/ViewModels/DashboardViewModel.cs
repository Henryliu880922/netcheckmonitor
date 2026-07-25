using CommunityToolkit.Mvvm.ComponentModel;

namespace NetCheckMonitor.Gui.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private string host = "8.8.8.8";

    [ObservableProperty]
    private int interval = 1000;

    [ObservableProperty]
    private int timeout = 3000;
}