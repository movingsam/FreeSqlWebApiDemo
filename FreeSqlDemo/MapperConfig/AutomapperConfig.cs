using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreeSqlDemo.Bussiness.DTO.Terant;
using FreeSqlDemo.Bussiness.DTO.User;
using FreeSqlDemo.Bussiness.Entities;
using FreeSqlDemo.Domain.Entities;
using FreeSqlDemo.Infrastructure.DomainBase;
using FreeSqlDemo.Infrastructure.Entity;

namespace FreeSqlDemo.MapperConfig
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<User, CurrentUser>()
                .ForMember(cu => cu.PhoneNumber, opt => opt.MapFrom(ue => ue.UserInfo.PhoneNumber))
                .ForMember(cu => cu.Roles, opt => opt.MapFrom(ue => from ur in ue.UserRoles select ur.Role));
            //.ForMember(cu => cu.Terant, opt => opt.MapFrom(ue => ue.Terant));
            CreateMap<UserInput, User>()
                .AfterMap((ui, u) =>
                {
                    u.UserInfo = new UserInfo();
                    u.UserInfo.Address = ui.Address;
                    u.UserInfo.Email = ui.Email;
                    u.UserInfo.PhoneNumber = ui.PhoneNumber;
                });

            //CreateMap<User,UserInput>()
            //    .ForMember(u=>u.Email,opt=>opt.MapFrom(ui=>ui.UserInfo))
            CreateMap<Terant, TerantInput>();
            CreateMap<TerantInput, Terant>();

            CreateMap<Role, RoleVO>();
        }
    }
}
