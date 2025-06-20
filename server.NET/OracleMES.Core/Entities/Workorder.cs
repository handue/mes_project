using System;
using System.Collections.Generic;

namespace OracleMES.Infrastructure.Entities;

public partial class Workorder
{
    public decimal Orderid { get; set; }

    public decimal Productid { get; set; }

    public decimal Workcenterid { get; set; }

    public decimal Machineid { get; set; }

    public decimal Employeeid { get; set; }

    public decimal Quantity { get; set; }

    public string? Plannedstarttime { get; set; }

    public string? Plannedendtime { get; set; }

    public string? Actualstarttime { get; set; }

    public string? Actualendtime { get; set; }

    public string? Status { get; set; }

    public decimal Priority { get; set; }

    public decimal Leadtime { get; set; }

    public string? Lotnumber { get; set; }

    public decimal? Actualproduction { get; set; }

    public decimal? Scrap { get; set; }

    public decimal? Setuptimeactual { get; set; }
}
