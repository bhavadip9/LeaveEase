
using LeaveEase.Entity.Models;

namespace LeaveEase.Repository.Interfaces
{
    public interface IHomeRepository
    {
        string GetRoleName(int? RoleID);
        List<T> GetEntities<T>() where T : class;
      
        Task<TblUser> AddRegistration(TblUser userData);

       
    }
}
