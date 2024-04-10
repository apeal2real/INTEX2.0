using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace INTEX2._0.Models;

public partial class ProductsCategory
{
    [ForeignKey("Category")]
    public int CategoryId { get; set; }

    public virtual Category Category { get; set; }

    
    [ForeignKey("Products")]
    public int ProductId { get; set; }
    public virtual Products Products { get; set; }
}
