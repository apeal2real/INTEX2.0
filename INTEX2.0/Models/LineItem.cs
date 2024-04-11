using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace INTEX2._0.Models;

public partial class LineItem
{
    [ForeignKey("Order")]
    public int TransactionId { get; set; }
    public virtual Order Order { get; set; }
    
    [ForeignKey("Products")]
    public int ProductId { get; set; }
    public virtual Products Products { get; set; }

    public int? Qty { get; set; }

    public int? Rating { get; set; }
}
