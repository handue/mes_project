using System;
using System.Collections.Generic;

namespace OracleMES.Infrastructure.Entities;

public partial class Downtime
{
    public decimal Downtimeid { get; set; }

    public decimal Machineid { get; set; }

    public decimal? Orderid { get; set; }

    public string? Starttime { get; set; }

    public string? Endtime { get; set; }

    public decimal? Duration { get; set; }

    public string Reason { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Reportedby { get; set; }
}
