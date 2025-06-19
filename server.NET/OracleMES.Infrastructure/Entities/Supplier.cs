using System;
using System.Collections.Generic;

namespace OracleMES.Infrastructure.Entities;

public partial class Supplier
{
    public decimal Supplierid { get; set; }

    public string Name { get; set; } = null!;

    public decimal Leadtime { get; set; }

    public decimal? Reliabilityscore { get; set; }

    public string? Contactinfo { get; set; }
}
