
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Roma_Shop_Server.Models.DB;
using Roma_Shop_Server.Services.AccountService;
using Roma_Shop_Server.Services.AccountService.Interfaces;
using Roma_Shop_Server.Services.Middlewares;
using Roma_Shop_Server.Services.OrderService;
using Roma_Shop_Server.Services.ProductService;
using Roma_Shop_Server.Services.ReviewService;
using Roma_Shop_Server.Services.TokenService;
using Roma_Shop_Server.Services.TokenService.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace Roma_Shop_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string? connection = builder.Configuration.GetSection("AppSettings:DefaultConnection").Value;
            if (connection == null) throw new Exception("Connection string is null. Check appsettings.json to solve this.");
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connection));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "bearer {token}",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
            builder.Services.AddHttpContextAccessor();


            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Security:AccessKey").Value))
                    };
                });

            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            builder.Services.AddSingleton<ITokenService, TokenService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ProductRepository, ProductRepository>();
            builder.Services.AddScoped<ReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<OrderRepository, OrderRepository>();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.MapControllers();
            app.UseMiddleware<JwtUserIdMiddleware>();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000"));
            app.Run();
        }
    }
}
