using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreeSqlDemo.Bussiness.DTO.Role;
using FreeSqlDemo.Bussiness.DTO.Terant;
using FreeSqlDemo.Bussiness.DTO.User;
using FreeSqlDemo.Bussiness.Entities;
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
            CreateMap<UserInput, User>()//automapper只支持第一层属性映射 所以需要使用AfterMap来做次层级的映射
                .AfterMap((ui, u) =>
                {
                    u.UserInfo = new UserInfo()
                    {
                        Address = ui.Address,
                        Email = ui.Email,
                        PhoneNumber = ui.PhoneNumber
                    };
                });
            CreateMap<UserUpdateInput, User>();


            CreateMap<User, UserUpdateInput>();


            CreateMap<Terant, TerantInput>();
            CreateMap<TerantInput, Terant>();

            CreateMap<Role, CurrentRole>();

            CreateMap<RoleInput, Role>();
            CreateMap<RoleUpdateInput, Role>();
            CreateMap<Role, RoleUpdateInput>();


            CreateMap<Role, RoleVO>()
                .ForMember(ro => ro.User,
                    opt => opt.MapFrom(r => from ur in r.UserRoles select ur.User));

            CreateMap<User, UserVO>()
                .ForMember(x => x.Email, opt => opt.MapFrom(u => u.UserInfo.Email))
                .ForMember(x => x.Address, opt => opt.MapFrom(u => u.UserInfo.Address))
                .ForMember(x => x.PhoneNumber, opt => opt.MapFrom(u => u.UserInfo.PhoneNumber))
                .ForMember(x => x.Roles, opt => opt.MapFrom(u => from ur in u.UserRoles select ur.Role));
        }
    }
}
