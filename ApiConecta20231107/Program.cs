using API.Auxiliares;
using API.Servicios;
using API.Validador;
using Data;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Servicios;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<EmpresaValidador>();


var keyPlease = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"]));



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = keyPlease,        // new SymmetricSecurityKey(
                                    //Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"])),
        ClockSkew = TimeSpan.Zero

    });

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApiConectaContext>().AddDefaultTokenProviders();

builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
    opciones.AddPolicy("EsInterno", politica => politica.RequireClaim("esInterno"));

});

//builder.Services.AddDataProtection();
builder.Services.AddTransient<HashService>();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(policy=> policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
});



ApiConectaContext.ConnectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<ApiConectaContext>(options =>
{
    options.UseSqlServer(builder.Configuration
        .GetConnectionString("Default")); // for example if you're holding the connection string in the appsettings.json
});


#region Build de servicios
//SERVICIOS
builder.Services.AddScoped<IEmpleadosServicio, EmpleadosServicio>();
builder.Services.AddScoped<IHistorialEmpleadoServicio, HistorialEmpleadoServicio>();

//AUXILIARES
builder.Services.AddScoped<ICatalogAUX, CatalogAUX>();
builder.Services.AddScoped<IMetodosAUX, MetodosAUX>();
#endregion


// Construccion de la app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
