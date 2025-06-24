using System;
using System.Collections.Generic;

namespace OracleMES.Core.Entities;

public partial class Machine
{
    public decimal Machineid { get; set; }

    public string Name { get; set; } = null!;

    public string? Type { get; set; }

    public decimal? Workcenterid { get; set; }

    public string? Status { get; set; }

    public decimal Nominalcapacity { get; set; }

    public string Capacityuom { get; set; } = null!;

    public decimal Setuptime { get; set; }

    public decimal Efficiencyfactor { get; set; }

    public decimal Maintenancefrequency { get; set; }

    public string? Lastmaintenancedate { get; set; }

    public string? Nextmaintenancedate { get; set; }

    public decimal Productchangeovertime { get; set; }

    public decimal Costperhour { get; set; }

    public string? Installationdate { get; set; }

    public string? Modelnumber { get; set; }
}
