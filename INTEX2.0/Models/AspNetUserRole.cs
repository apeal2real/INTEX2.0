using System.ComponentModel.DataAnnotations.Schema;

namespace INTEX2._0.Models
{
    public class AspNetUserRole
    {
        public string RoleId { get; set; }
        
        public string UserId { get; set; }
    }
}
