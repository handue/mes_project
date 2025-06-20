using System;
using System.Collections.Generic;

namespace OracleMES.Infrastructure.Entities;

public partial class Workcenter
{
    public decimal Workcenterid { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Capacity { get; set; }

    public string Capacityuom { get; set; } = null!;

    public decimal Costperhour { get; set; }

    public string? Location { get; set; }

    public decimal? Isactive { get; set; }
}
