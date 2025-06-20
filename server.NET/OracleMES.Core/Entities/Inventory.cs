using System;
using System.Collections.Generic;

namespace OracleMES.Infrastructure.Entities;

public partial class Inventory
{
    public decimal Itemid { get; set; }

    public string Name { get; set; } = null!;

    public string? Category { get; set; }

    public decimal Quantity { get; set; }

    public decimal Reorderlevel { get; set; }

    public decimal? Supplierid { get; set; }

    public decimal Leadtime { get; set; }

    public decimal Cost { get; set; }

    public string? Lotnumber { get; set; }

    public string? Location { get; set; }

    public string? Lastreceiveddate { get; set; }
}
