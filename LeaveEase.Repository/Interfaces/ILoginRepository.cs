

using LeaveEase.Entity.Models;

namespace LeaveEase.Repository.Interfaces
{
    public interface ILoginRepository
    {
        public TblUser GetUserByEmail(string email);
        bool UserExist(string mail);
        bool UpdatePassword(TblUser user);
    }
}
