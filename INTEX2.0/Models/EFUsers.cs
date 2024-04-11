using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using INTEX2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace INTEX2._0.Models
{
    public class EFUsers : IUsers
    {
        private MfalabW24Context _context;

        public EFUsers(MfalabW24Context temp)
        {
            _context = temp;
        }

        public List<AspNetRole> AspNetRoles => _context.AspNetRoles.ToList();

        public List<AspNetRoleClaim> AspNetRoleClaims => _context.AspNetRoleClaims.ToList();

        public List<AspNetUser> AspNetUsers => _context.AspNetUsers.ToList();

        public List<AspNetUserClaim> AspNetUserClaims => _context.AspNetUserClaims.ToList();

        public List<AspNetUserLogin> AspNetUserLogins => _context.AspNetUserLogins.ToList();

        public List<AspNetUserToken> AspNetUserTokens => _context.AspNetUserTokens.ToList();
        public List<AspNetUserRole> AspNetUserRoles => _context.AspNetUserRoles.ToList();

        public void RemoveUser(AspNetUser user)
        {
            _context.AspNetUsers.Remove(user);
            _context.SaveChanges();
        }

        public void UpdateUser(AspNetUser user)
        {
            _context.AspNetUsers.Update(user);
            _context.SaveChanges();
        }
    }
}
