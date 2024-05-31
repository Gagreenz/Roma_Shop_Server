using Roma_Shop_Server.Data;
using System.IdentityModel.Tokens.Jwt;

namespace Roma_Shop_Server.Services.Middlewares
{
    public class JwtUserIdMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtUserIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

                var id = jwtToken.Claims.First(c => c.Type == ArnegJWTClaims.UserId).Value;

                context.Items[ArnegJWTClaims.UserId] = id;
            }

            await _next(context);
        }
    }
}
