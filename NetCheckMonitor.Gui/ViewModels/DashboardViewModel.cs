using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using NetCheckMonitor.Core.Models;
using NetCheckMonitor.Core.Models.SystemInfo;
using NetCheckMonitor.Core.Services;
using NetCheckMonitor.Core.Services.Interfaces;

namespace NetCheckMonitor.Gui.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ISystemInfoService systemInfoService;

    [ObservableProperty]
    private string host = "8.8.8.8";

    [ObservableProperty]
    private int interval = 1000;

    [ObservableProperty]
    private int timeout = 3000;

    private SystemInfo? systemInformation;

    public SystemInfo? SystemInformation
    {
        get => systemInformation;
        private set => SetProperty(ref systemInformation, value);
    }

    private string? systemInfoError;

    public string? SystemInfoError
    {
        get => systemInfoError;
        private set => SetProperty(ref systemInfoError, value);
    }

    private bool isLoadingSystemInfo;

    public bool IsLoadingSystemInfo
    {
        get => isLoadingSystemInfo;
        private set => SetProperty(ref isLoadingSystemInfo, value);
    }

    public ObservableCollection<MonitorResult> Results { get; } = new();

    public DashboardViewModel()
    {
        systemInfoService = SystemInfoServiceFactory.Create();

        Results.Add(new MonitorResult
        {
            Timestamp = DateTime.Now,
            Host = "8.8.8.8",
            Status = "Success",
            RoundTripTime = 18
        });

        _ = LoadSystemInfoAsync();
    }

    private async Task LoadSystemInfoAsync()
    {
        try
        {
            IsLoadingSystemInfo = true;
            SystemInfoError = null;

            SystemInformation =
                await systemInfoService.GetSystemInfoAsync();
        }
        catch (Exception exception)
        {
            SystemInfoError = exception.Message;
        }
        finally
        {
            IsLoadingSystemInfo = false;
        }
    }
}