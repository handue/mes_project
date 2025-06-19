using System;
using System.Collections.Generic;

namespace OracleMES.Infrastructure.Entities;

public partial class Defect
{
    public decimal Defectid { get; set; }

    public decimal Checkid { get; set; }

    public string Defecttype { get; set; } = null!;

    public decimal? Severity { get; set; }

    public decimal? Quantity { get; set; }

    public string? Location { get; set; }

    public string? Rootcause { get; set; }

    public string? Actiontaken { get; set; }
}
