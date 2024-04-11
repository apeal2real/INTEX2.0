using System;
using System.Collections.Generic;

namespace INTEX2._0.Models;

public partial class ProductRecommendation
{
    public int ItemId { get; set; }

    public int? RecommendedId1 { get; set; }

    public int? RecommendedId2 { get; set; }

    public int? RecommendedId3 { get; set; }
}
