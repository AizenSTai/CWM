using Microsis.CWM.Dto.Basic;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Microsis.CWM.AppService
{
    public interface IHelperAppService
    {
        ClaimDto GetClaim();
    }

    public class HelperAppService : IHelperAppService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;


        public HelperAppService(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public ClaimDto GetClaim()
        {
            string? secretKey = _configuration.GetSection("JWT").GetSection("SecretKey").Value;

            ClaimDto claim = new ClaimDto();
            var context = _contextAccessor.HttpContext;

            var lst = context.User.Claims.ToList();
            var principal = GetPrincipal(secretKey, GetToken(context));

            if (principal == null)
                return null;

            ClaimsIdentity identity = null;
            identity = (ClaimsIdentity)principal.Identity;

            if (!identity.Claims.Any())
                return null;

            lst = identity.Claims.ToList();
            claim.Username = lst.Find(x => x.Type == ClaimTypes.UserData).Value;
            claim.UserKey = lst.Find(x => x.Type == ClaimTypes.Hash).Value;
            long lUserId = 0;
            long.TryParse(lst.Find(x => x.Type == ClaimTypes.Name).Value, out lUserId);
            claim.UserId = lUserId;
            long lTariff = 0;
            long.TryParse(lst.Find(x => x.Type.Equals("TID", StringComparison.OrdinalIgnoreCase)).Value, out lTariff);
            claim.TariffId = lTariff;
            return claim;
        }

        #region ______ Private _______
        private ClaimsPrincipal GetPrincipal(string secretKey, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return null;

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Encoding.ASCII.GetBytes(secretKey);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                    parameters, out securityToken);
                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string GetToken(HttpContext context)
        {
            Microsoft.Extensions.Primitives.StringValues authTokens;

            context.Request.Headers.TryGetValue("Authorization", out authTokens);

            if (string.IsNullOrEmpty(authTokens))
                return String.Empty;

            var _token = authTokens.FirstOrDefault().Replace("Bearer ", String.Empty);

            return _token;

        }
        #endregion

    }
}
