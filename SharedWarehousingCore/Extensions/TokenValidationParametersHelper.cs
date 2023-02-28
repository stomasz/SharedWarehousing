using Microsoft.IdentityModel.Tokens;

namespace SharedWarehousingCore.Extensions
{
    /* It's a helper class that returns a new instance of `TokenValidationParameters` class */
    public static class TokenValidationParametersHelper
    {
        
        /// <summary>
        /// It creates a new instance of the TokenValidationParameters class, sets its properties and returns it
        /// </summary>
        /// <param name="SymmetricSecurityKey">This is the key that will be used to sign the token.</param>
        public static TokenValidationParameters GetTokenValidationParameters(SymmetricSecurityKey key)
            => new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false, // todo tsos: ogarnać przed produkcja
                ValidateAudience = false,
                //RequireExpirationTime = true,
            };
        
    }
}