using System;
using System.Collections.Generic;

namespace INTEX2._0.Models;

public partial class Customer
{
    public int? CustomerIdPk { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? BirthDate { get; set; }

    public string? CountryOfResidence { get; set; }

    public string? Gender { get; set; }

    public double? Age { get; set; }
}
