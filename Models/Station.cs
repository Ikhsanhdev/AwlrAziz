using System;
using System.Collections.Generic;

namespace AwlrAziz.Models;

public partial class Station
{
    public Guid Id { get; set; }
    public string? Photo { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public double? Elevation { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? Slug { get; set; }
    // public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}