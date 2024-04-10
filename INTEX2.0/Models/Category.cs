using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace INTEX2._0.Models;

public partial class Category
{
    [Key]
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }
}
