using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreeSqlDemo.Bussiness.DTO.Role;
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

            CreateMap<Role, CurrentRole>();

            CreateMap<RoleInput, Role>();

            CreateMap<Role, RoleVO>()
                .ForMember(ro => ro.User,
                    opt => opt.MapFrom(r => from ur in r.UserRoles select ur.User));

            CreateMap<User, UserVO>()
                .ForMember(x => x.Email, opt => opt.MapFrom(u => u.UserInfo.Email))
                .ForMember(x => x.Address, opt => opt.MapFrom(u => u.UserInfo.Address))
                .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(u => u.UserInfo.PhoneNumber))
                .ForMember(x=>x.Roles,opt=>opt.MapFrom(u=> from ur in u.UserRoles select  ur.Role));
        }
    }
}
