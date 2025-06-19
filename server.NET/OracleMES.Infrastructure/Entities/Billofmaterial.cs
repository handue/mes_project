using System;
using System.Collections.Generic;

namespace OracleMES.Infrastructure.Entities;

public partial class Billofmaterial
{
    public decimal Bomid { get; set; }

    public decimal Productid { get; set; }

    public decimal Componentid { get; set; }

    public decimal Quantity { get; set; }

    public decimal? Scrapfactor { get; set; }
}
