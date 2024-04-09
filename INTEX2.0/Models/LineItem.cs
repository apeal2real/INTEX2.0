using System;
using System.Collections.Generic;

namespace INTEX2._0.Models;

public partial class LineItem
{
    public int? TransactionIdPkFk { get; set; }

    public int? ProductIdPkFk { get; set; }

    public int? Qty { get; set; }

    public int? Rating { get; set; }
}
