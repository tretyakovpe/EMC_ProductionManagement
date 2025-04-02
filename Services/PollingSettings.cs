namespace ProductionManagement.Services;

public class PollingSettings
{
    public int LinesFetchIntervalInSeconds { get; set; }
    public int PlcPollingIntervalInMilliseconds { get; set; }
}