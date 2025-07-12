
using LeaveEase.Entity.Models;
using LeaveEase.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveEase.Repository.Implementation
{
    public class LoginRepository:ILoginRepository
    {
        private readonly LeaveEaseDbContext _context;

        public LoginRepository(LeaveEaseDbContext context)
        {
            _context = context;
        }


        public TblUser GetUserByEmail(string email)
        {
            try
            {
                return _context.TblUsers.Include(c=>c.RoleNavigation).FirstOrDefault(m => m.Email.ToLower() == email.ToLower() &&  m.IsActive)!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return new TblUser();
            }
        }

        public bool UserExist(string mail)
        {
            try
            {
                var isExist = _context.TblUsers.FirstOrDefault(c => c.Email.ToLower() == mail.ToLower());
                if (isExist == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }
        }

        public bool UpdatePassword(TblUser user)
        {
            try
            {
                 _context.Update(user);
                 _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}\n{ex.InnerException?.Message}");
                return false;
            }

        }

    }
}
