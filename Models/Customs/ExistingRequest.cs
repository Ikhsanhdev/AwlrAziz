using System;
using System.Collections.Generic;

namespace AwlrAziz.Models.Customs;

public class ExistingRequest
{
    public string id { get; set; }

    public double? volt { get; set; } = 12;

    public double? tma { get; set; }

    public double? rain { get; set; }

    public double? hmd { get; set; }

    public double? pr { get; set; }

    public double? rf { get; set; }

    public double? sr { get; set; }

    public double? tmp { get; set; }

    public double? wd { get; set; }

    public double? ws { get; set; }

    public double? evp { get; set; }

    public double? ph { get; set; }

    public double? orp { get; set; }

    public double? tbd { get; set; }

    public double? flow_rate { get; set; }
    
    public string reading_at { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}
