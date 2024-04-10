using System;
using System.Collections.Generic;

namespace INTEX2._0.Models;

public partial class LineItem
{
    public int TransactionId { get; set; }

    public int ProductId { get; set; }

    public int? Qty { get; set; }

    public int? Rating { get; set; }
}
