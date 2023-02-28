using AutoMapper;
using SharedWarehousingCore.Dtos.IdentityDTOs;
using SharedWarehousingCore.Model.IdentityModel;

namespace SharedWarehousingCore.Extensions;

public class IdentityDtosAutoMapperProfile : Profile
{
    public IdentityDtosAutoMapperProfile()
    {
        CreateMap<AppUser, AppUserDto>();
    }
}