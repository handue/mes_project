using System;
using System.Collections.Generic;

namespace OracleMES.Core.Entities;

public partial class Shift
{
    public decimal Shiftid { get; set; }

    public string Name { get; set; } = null!;

    public string Starttime { get; set; } = null!;

    public string Endtime { get; set; } = null!;

    public decimal Capacity { get; set; }

    public decimal? Isweekend { get; set; }
}
