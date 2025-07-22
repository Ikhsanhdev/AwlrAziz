using System;
using System.Collections.Generic;

namespace AwlrAziz.Models;

public partial class AwlrSetting
{
    public Guid StationId { get; set; }
    public string DeviceId { get; set; }
    public string? UnitDisplay { get; set; }
    public string? UnitSensor { get; set; }
    public double? KonstantaA { get; set; }
    public double? KonstantaB { get; set; }
    public decimal? Siaga1 { get; set; }
    public decimal? Siaga2 { get; set; }
    public decimal? Siaga3 { get; set; }
    public double? PeilschaalBasisValue { get; set; }
    public double? PeilschaalBasisElevation { get; set; }
}