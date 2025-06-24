using System;
using System.Collections.Generic;

namespace OracleMES.Core.Entities;

public partial class Oeemetric
{
    public decimal Metricid { get; set; }

    public decimal Machineid { get; set; }

    public string? Date { get; set; }

    public decimal? Availability { get; set; }

    public decimal? Performance { get; set; }

    public decimal? Quality { get; set; }

    public decimal? Oee { get; set; }

    public decimal? Plannedproductiontime { get; set; }

    public decimal? Actualproductiontime { get; set; }

    public decimal? Downtime { get; set; }
}
