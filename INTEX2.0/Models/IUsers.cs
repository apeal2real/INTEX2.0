using Microsoft.EntityFrameworkCore;

namespace INTEX2._0.Models
{
    public interface IUsers
    {
        List<AspNetRole> AspNetRoles { get; }

        List<AspNetRoleClaim> AspNetRoleClaims { get; }

        List<AspNetUser> AspNetUsers { get; }

        List<AspNetUserClaim> AspNetUserClaims { get; }

        List<AspNetUserLogin> AspNetUserLogins { get; }

        List<AspNetUserToken> AspNetUserTokens { get; }
        List<AspNetUserRole> AspNetUserRoles { get; }
        public void RemoveUser(AspNetUser user);
        public void UpdateUser(AspNetUser user);
        public void AddUser(AspNetUser user);
        public void UpdateUserRole(AspNetUserRole userRole);
        public void AddUserRole(AspNetUserRole userRole);
    }
}
