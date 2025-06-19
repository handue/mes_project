using System;
using System.Collections.Generic;

namespace OracleMES.Infrastructure.Entities;

public partial class Qualitycontrol
{
    public decimal Checkid { get; set; }

    public decimal Orderid { get; set; }

    public string? Date { get; set; }

    public string? Result { get; set; }

    public string? Comments { get; set; }

    public decimal? Defectrate { get; set; }

    public decimal? Reworkrate { get; set; }

    public decimal? Yieldrate { get; set; }

    public decimal? Inspectorid { get; set; }
}
