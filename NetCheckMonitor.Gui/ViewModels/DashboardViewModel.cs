using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using NetCheckMonitor.Core.Models;

namespace NetCheckMonitor.Gui.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private string host = "8.8.8.8";

    [ObservableProperty]
    private int interval = 1000;

    [ObservableProperty]
    private int timeout = 3000;

    public ObservableCollection<MonitorResult> Results { get; } = new();

    public DashboardViewModel()
    {
        Results.Add(new MonitorResult
        {
            Timestamp = DateTime.Now,
            Host = "8.8.8.8",
            Status = "Success",
            RoundTripTime = 18
        });
    }
}