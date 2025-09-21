using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using tourism_minya.Infrastructure.Common.Mappings;
using tourism_minya.Infrastructure.Entities;
using tourism_minya.Infrastructure.Interfaces;
using tourism_minya.Infrastructure.Persistence;
using tourism_minya.Infrastructure.Services;
using Tourism_minya.Application.Interfaces;
using Tourism_minya.Application.Settings;

namespace Tourism_minya {
public class Program
{
    public static async Task Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);


    var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
    builder.Services.Configure<JwtSettings>(jwtSettingsSection);
    var jwtSettings = jwtSettingsSection.Get<JwtSettings>();


    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("tourism_minya.Infrastructure")
        )
    );
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
    })


   .AddEntityFrameworkStores<ApplicationDbContext>()
   .AddDefaultTokenProviders();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "JwtBearer";
        options.DefaultChallengeScheme = "JwtBearer";
    })
    .AddJwtBearer("JwtBearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            RoleClaimType = ClaimTypes.Role
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Admin", policy =>
            policy.RequireRole("Admin"));


        options.AddPolicy("AdminOrMember", policy =>
            policy.RequireRole("Admin", "Member"));
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularClient",
            builder =>
            {
                builder.WithOrigins("http://localhost:4200")
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
    });






    builder.Services.AddAutoMapper(typeof(MappingProfile));


    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITourismType, TourismTypeService>();
    builder.Services.AddScoped<ICenter, CenterService>();
    builder.Services.AddScoped<IRoleService, RoleService>();


            builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Tourism_Minya System API",
            Version = "v1",
            Description = "?? Tourism_Minya System API - Manage users, Places ",
            Contact = new OpenApiContact
            {
                Name = "Basma Khalaf",
                Email = "basmakhalaf974@gmail.com"
            }
        });


        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });


        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });


    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();


    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = { "Admin", "Member" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }




    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseCors("AllowAngularClient");
    app.UseAuthorization();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.MapControllers();

    try
    {
        app.Run();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Startup failed: " + ex.Message);
        throw;
    }
}
    }
}