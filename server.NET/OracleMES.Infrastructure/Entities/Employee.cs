using System;
using System.Collections.Generic;

namespace OracleMES.Infrastructure.Entities;

public partial class Employee
{
    public decimal Employeeid { get; set; }

    public string Name { get; set; } = null!;

    public string? Role { get; set; }

    public decimal? Shiftid { get; set; }

    public decimal Hourlyrate { get; set; }

    public string? Skills { get; set; }

    public string? Hiredate { get; set; }
}
