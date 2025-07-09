using System;
using System.Collections.Generic;

namespace AwlrAziz.Models;

public partial class Device
{
    public Guid StationId { get; set; }

    public string DeviceId { get; set; } = null!;

    public string BrandCode { get; set; } = null!;

    public string? NoGsm { get; set; }

    public DateOnly? InstalledDate { get; set; }

    public double? Calibration { get; set; }

    // public virtual AwlrLastReading? AwlrLastReading { get; set; }

    // public virtual AwlrSetting? AwlrSetting { get; set; }

    // public virtual Brand BrandCodeNavigation { get; set; } = null!;

    // public virtual Station Station { get; set; } = null!;
}