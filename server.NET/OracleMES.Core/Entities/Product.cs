using System;
using System.Collections.Generic;

namespace OracleMES.Infrastructure.Entities;

public partial class Product
{
    public decimal Productid { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public decimal Cost { get; set; }

    public decimal? Standardprocesstime { get; set; }

    public decimal? Isactive { get; set; }
}
