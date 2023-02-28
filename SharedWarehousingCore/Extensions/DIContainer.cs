using Microsoft.Extensions.DependencyInjection;
using SharedWarehousingCore.Services.AccountServices;
using SharedWarehousingCore.Services.TokenServices;

namespace SharedWarehousingCore.Extensions;

    public static class DIContainer
    {
        public static IServiceCollection AddDIContainer(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAccountService, AccountService>();
            return services;
        }
    }

