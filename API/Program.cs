﻿using Models.Shared.Hubs;
using AutoMapper;
using Bussines;
using Constantes;
using DBModel.DB;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using IBussines;
using IRepository;
using IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Service;
using SixLabors.ImageSharp;
using System.Text;
using System.Text.Json.Serialization;
using UtilMapper;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

builder.Services.AddCors(options =>
{
    if (env.IsDevelopment())
    {
        // Para desarrollo: más permisivo
        options.AddPolicy("AllowAll",
            builder => builder
                .SetIsOriginAllowed(_ => true) // Permite cualquier origen
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()); // Necesario para SignalR
    }
    else
    {
        // Para producción: más restrictivo
        options.AddPolicy("AllowAll",
            builder => builder
                .WithOrigins("https://tudominio.com") // Cambia por tu dominio
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    }
});

/* PARA IMPLEMENTAR NUESTROS PROTOCOLOS DE SEGURIDAD ==> JWT */
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            
            return Task.CompletedTask;
        }
    };
});


// Agregar otros servicios al contenedor.
builder.Services.AddControllers();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true; // Para debugging
});
// Configuración de Swagger/OpenAPI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SISTEMA DE LIBRERIA",
        Version = "v1",
        Description = "Documentación de los servicios para el sistema de Libreria Saber",
        Contact = new OpenApiContact
        {
            Name = "Piero Palacios",
            Url = new Uri("https://www.linkedin.com/in/franklinleonciofuamanaraujo/"),
        },
    });
});

builder.Services.AddAutoMapper(typeof(IStartup).Assembly, typeof(AutoMapperProfiles).Assembly);

// Registro de servicios
builder.Services.AddScoped<ILibroBussines, LibroBussines>();
builder.Services.AddScoped<IAzureStorage, AzureStorage>();
builder.Services.AddScoped<IApisPaypalServices, ApisPaypalServices>();

builder.Services.AddScoped<IKardexRepository, KardexRepository>();
builder.Services.AddScoped<IKardexBussines, KardexBussines>();
builder.Services.AddScoped<IVentaBussines, VentaBussines>();
builder.Services.AddScoped<IDetalleVentaBussines, DetalleVentaBussines>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPersonaBussines, PersonaBussines>();
builder.Services.AddScoped<ICajaBussines, CajaBussines>();
builder.Services.AddScoped<ICajaRepository, CajaRepository>();
builder.Services.AddScoped<IRetiroDeCajaBussines, RetiroDeCajaBussines>();
builder.Services.AddScoped<IPedidoProveedorBussines, PedidoProveedorBussines>();
builder.Services.AddScoped<IDetallePedidoProveedorBussines, DetallePedidoProveedorBussines>();

builder.Services.AddScoped<IPedidoProveedorRepository, PedidoProveedorRepository>();
builder.Services.AddScoped<IDetallePedidoProveedorRepository, DetallePedidoProveedorRepository>();
builder.Services.AddScoped<IFirebaseStorageService, FirebaseStorageService>();

builder.Services.AddScoped<INotificacionBussines, NotificacionBussines>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();
builder.Services.AddHostedService<StockNotificationService>();
builder.Services.AddScoped<ILibroAutorRepository, LibroAutorRepository>();
builder.Services.AddScoped<IAutorRepository, AutorRepository>();
builder.Services.AddScoped<IPrecioRepository, PrecioRepository>();










var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificacionHub>("/hubs/notificaciones"); 
});
app.MapControllers();

app.Run();
