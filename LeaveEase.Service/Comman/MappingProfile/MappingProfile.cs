using AutoMapper;
using LeaveEase.Entity.Models;
using LeaveEase.Entity.ViewModel;
using LeaveEase.Service.Utills;

namespace LeaveEase.Service.Comman.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegisterViewModel, TblUser>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => PasswordUtills.HashPassword(src.Password)))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow)).ReverseMap();


            CreateMap<LeaveRequestViewModel, TblLeaveRequest>().ReverseMap();
            CreateMap<LeaveApproveViewModel, TblLeaveRequest>().ReverseMap();
            CreateMap<PermissionViewModel, TblPermission>().ReverseMap();
            CreateMap<TblRole, RoleViewModel>().ReverseMap();
        }
    }
}
