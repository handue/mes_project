using System;
using System.Collections.Generic;

namespace OracleMES.Core.Entities;

public partial class Materialconsumption
{
    public decimal Consumptionid { get; set; }

    public decimal Orderid { get; set; }

    public decimal Itemid { get; set; }

    public decimal Plannedquantity { get; set; }

    public decimal? Actualquantity { get; set; }

    public decimal? Variancepercent { get; set; }

    public string? Consumptiondate { get; set; }

    public string? Lotnumber { get; set; }
}
