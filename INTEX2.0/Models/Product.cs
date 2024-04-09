using System;
using System.Collections.Generic;

namespace INTEX2._0.Models;

public partial class Product
{
    public int? ProductIdPk { get; set; }

    public string? Name { get; set; }

    public int? Year { get; set; }

    public int? NumParts { get; set; }

    public int? Price { get; set; }

    public string? ImgLink { get; set; }

    public string? PrimaryColor { get; set; }

    public string? SecondaryColor { get; set; }

    public string? ShortDescription { get; set; }

    public string? Description { get; set; }
}
