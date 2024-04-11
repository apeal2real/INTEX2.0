using System.ComponentModel.DataAnnotations.Schema;

namespace INTEX2._0.Models
{
    public class AspNetUserRole
    {
        [ForeignKey("RoleId")]
        public string RoleId { get; set; }

        public virtual AspNetRole Role { get; set; }


        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public virtual AspNetUser User { get; set; }
    }
}
