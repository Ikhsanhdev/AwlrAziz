using System;
using System.Collections.Generic;

namespace AwlrAziz.Models;

public partial class MvDevice
{
    public string? OrganizationCode { get; set; }

    public string? OrganizationName { get; set; }

    public string? SubDomain { get; set; }

    public string? SubDomainOld { get; set; }

    public string? BrandCode { get; set; }

    public string? BrandName { get; set; }

    public string? BrandUrl { get; set; }

    public bool? BrandIsBasicAuth { get; set; }

    public string? BrandUsername { get; set; }

    public string? BrandPassword { get; set; }

    public string? BrandJobName { get; set; }

    public Guid? StationId { get; set; }

    public string? StationName { get; set; }

    public string? StationType { get; set; }

    public string? TimeZone { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeviceId { get; set; }

    public string? NoGsm { get; set; }

    public DateOnly? InstalledDate { get; set; }

    public double? Calibration { get; set; }

    public string? UnitDisplay { get; set; }

    public string? UnitSensor { get; set; }

    public string? UnitDebit { get; set; }

    public double? PeilschaalBasisValue { get; set; }

    public double? PeilschaalBasisElevation { get; set; }

    public double? KonstantaA { get; set; }

    public double? KonstantaB { get; set; }

    public double? Siaga1 { get; set; }

    public double? Siaga2 { get; set; }

    public double? Siaga3 { get; set; }

    public double? HeightMercu { get; set; }

    public bool? IsHumidity { get; set; }

    public bool? IsRainfall { get; set; }

    public bool? IsPressure { get; set; }

    public bool? IsSolarRadiation { get; set; }

    public bool? IsTemperature { get; set; }

    public bool? IsWindDirection { get; set; }

    public bool? IsWindSpeed { get; set; }

    public bool? IsEvaporation { get; set; }
}
