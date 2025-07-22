using System;
using System.Collections.Generic;

namespace AwlrAziz.ViewModels;

public partial class VMAwlrLastReading
{
    public Guid StationId { get; set; }
    public Guid Id { get; set; }
    public string DeviceId { get; set; } = null!;

    public DateTime ReadingAt { get; set; }
    public decimal WaterLevel { get; set; }
    public decimal? ChangeValue { get; set; }

    public string? ChangeStatus { get; set; }

    public string? WarningStatus { get; set; }

    // public virtual Device Device { get; set; } = null!;
}
