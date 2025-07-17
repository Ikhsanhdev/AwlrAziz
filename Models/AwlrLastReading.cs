using System;
using System.Collections.Generic;

namespace AwlrAziz.Models;

public partial class AwlrLastReading
{
    public Guid StationId { get; set; }
    public Guid Id { get; set; }
    public string DeviceId { get; set; } = null!;

    public DateTime ReadingAt { get; set; }

    public double WaterLevel { get; set; }

    public double? ChangeValue { get; set; }

    public string? ChangeStatus { get; set; }

    public string? WarningStatus { get; set; }

    // public virtual Device Device { get; set; } = null!;
}
