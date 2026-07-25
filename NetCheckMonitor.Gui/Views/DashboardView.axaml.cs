using Avalonia.Controls;
using NetCheckMonitor.Gui.ViewModels;

namespace NetCheckMonitor.Gui.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();

        DataContext = new DashboardViewModel();
    }
}