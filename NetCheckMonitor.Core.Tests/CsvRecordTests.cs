using NetCheckMonitor.Core.Report.Csv;

namespace NetCheckMonitor.Core.Tests;

public class CsvRecordTests
{
    [Fact]
    public void CsvRecord_CanStoreValues()
    {
        var record = new CsvRecord
        {
            Timestamp = new DateTime(2026, 7, 23),
            Type = "CHECK",
            Status = "ONLINE",
            LatencyMs = 15,
            Target = "https://example.com",
            Detail = "OK"
        };

        Assert.Equal("CHECK", record.Type);
        Assert.Equal(15, record.LatencyMs);
        Assert.Equal("OK", record.Detail);
    }
}