using AutoMapper;

namespace SharedWarehousingCore.Extensions;

public static class AutoMapperConfig
{
    public static IMapper Initialize()
    {
        return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DomainDtosAutoMapperProfile>();
                cfg.AddProfile<IdentityDtosAutoMapperProfile>();
            })
            .CreateMapper();
    }
}