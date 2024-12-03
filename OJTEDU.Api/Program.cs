using Microsoft.AspNetCore.DataProtection.Repositories;
﻿using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using OJTEDU.Api.Configuration;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = false; // Đảm bảo rằng filter này được bật
});

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});
var logger = loggerFactory.CreateLogger<Program>();

// Log initial information
logger.LogInformation("Application starting...");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
logger.LogInformation("Configuring DbContext with connection string.");
builder.Services.ConfigureDbContext(connectionString);

// Configure AutoMapper
logger.LogInformation("Configuring AutoMapper.");
builder.Services.ConfigureAutoMapper();

// Configure Services and Repository
logger.LogInformation("Configuring Repositories.");
builder.Services.ConfigureRepositories();

// Configure CORS for ReactJS default port
logger.LogInformation("Configuring CORS for ReactJS on port 3000.");
builder.Services.ConfigureCors();

// Configure Http Context Accessor
logger.LogInformation("Configuring HttpContextAccessor.");
builder.Services.AddHttpContextAccessor();

// Configure swagger
logger.LogInformation("Configuring Swagger.");
builder.Services.ConfigureSwagger();

// Configure Authentication
logger.LogInformation("Configuring Authentication.");
builder.Services.ConfigureAuthentication();

// Configure Authorization
logger.LogInformation("Configuring Authorization.");
builder.Services.ConfigureAuthorization();

// Configure IHttpClientFactory to call API AI
logger.LogInformation("Configuring IHttpClientFactory.");
builder.Services.AddHttpClient();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Thiết lập LicenseContext cho EPPlus
ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Hoặc LicenseContext.Commercial nếu bạn có giấy phép thương mại
logger.LogInformation("EPPlus LicenseContext set to NonCommercial.");

var app = builder.Build();

// Log when application is built
logger.LogInformation("Application built, configuring middleware...");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    c.DisplayRequestDuration();
    c.EnableFilter();
    c.EnableDeepLinking();
});

// Enable HTTPS redirection
logger.LogInformation("Enabling HTTPS redirection.");
app.UseHttpsRedirection();

// Forward headers for proxies
logger.LogInformation("Configuring forwarded headers for proxies.");
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

// Serve static files
logger.LogInformation("Configuring static files.");
app.UseStaticFiles();

app.UseRouting();

// Enable CORS
logger.LogInformation("Enabling CORS.");
app.UseCors("AllowSpecificOrigin");

// Enable authentication and authorization
logger.LogInformation("Enabling authentication and authorization.");
app.UseAuthentication();
app.UseAuthorization();

// Map controllers to endpoints
logger.LogInformation("Mapping controllers to endpoints.");
app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Run the application
logger.LogInformation("Running application...");
app.Run();


//using Microsoft.AspNetCore.HttpOverrides;
//using Microsoft.Extensions.FileProviders;
//using OfficeOpenXml;
//using OJTEDU.Api.Configuration;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
//{
//    options.SuppressModelStateInvalidFilter = false; // Đảm bảo rằng filter này được bật
//});

//var loggerFactory = LoggerFactory.Create(builder =>
//{
//    builder.AddConsole();
//    builder.AddDebug();
//});
//var logger = loggerFactory.CreateLogger<Program>();

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// Configure DbContext with SQL Server
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.ConfigureDbContext(connectionString);

//// Configure AutoMapper
//builder.Services.ConfigureAutoMapper();

//// Configure Services and Repository
//builder.Services.ConfigureRepositories();

//// Configure CORS for ReactJS default port
//builder.Services.ConfigureCors("https://localhost:3000");

//// Configure Http Context Accessor
//builder.Services.AddHttpContextAccessor();

//// Configure swagger
//builder.Services.ConfigureSwagger();

//// Configure Authentication
//builder.Services.ConfigureAuthentication();

//// Configure Authorization
//builder.Services.ConfigureAuthorization();

//// Configure IHttpClientFactory to call API AI
//builder.Services.AddHttpClient();

//// Thiết lập LicenseContext cho EPPlus
//ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Hoặc LicenseContext.Commercial nếu bạn có giấy phép thương mại

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//// For development environment
////if (app.Environment.IsDevelopment())
////{
////    app.UseSwagger();
////    app.UseSwaggerUI();
////    app.UseSwaggerUI(c =>
////    {
////        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
////        c.DisplayRequestDuration();
////        c.EnableFilter();
////        c.EnableDeepLinking();
////    });
////}

//// For production environment
////app.UseSwagger();
////app.UseSwaggerUI();
////app.UseSwaggerUI(c =>
////{
////    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
////    c.DisplayRequestDuration();
////    c.EnableFilter();
////    c.EnableDeepLinking();
////});

////app.UseHttpsRedirection();

////app.UseAuthentication();
////app.UseRouting();
////app.UseAuthorization();

////app.UseCors();

////app.MapControllers();

////app.UseStaticFiles();

////app.UseEndpoints(endpoints =>
////{
////    endpoints.MapControllers();
////});

////app.Run();

////app.UseSwagger();
////app.UseSwaggerUI(c =>
////{
////    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
////    c.DisplayRequestDuration();
////    c.EnableFilter();
////    c.EnableDeepLinking();
////});

//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
//    c.DisplayRequestDuration();
//    c.EnableFilter();
//    c.EnableDeepLinking();
//});

//app.UseHttpsRedirection();

//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//    ForwardedHeaders = ForwardedHeaders.XForwardedProto
//});

//app.UseStaticFiles();
//app.UseCors("AllowSpecificOrigin");

//app.UseAuthentication();
//app.UseRouting();
//app.UseAuthorization();

//app.MapControllers(); // Map các Controller

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

//app.Run();


/*
 using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using OJTEDU.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = false; // Đảm bảo rằng filter này được bật
}); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.ConfigureDbContext(connectionString);

// Configure AutoMapper
builder.Services.ConfigureAutoMapper();

// Configure Services and Repository
builder.Services.ConfigureRepositories();

// Configure CORS for ReactJS default port
builder.Services.ConfigureCors("https://localhost:3000");

// Configure Http Context Accessor
builder.Services.AddHttpContextAccessor();

// Configure swagger
builder.Services.ConfigureSwagger();

// Configure Authentication
builder.Services.ConfigureAuthentication();

// Configure Authorization
builder.Services.ConfigureAuthorization();

// Configure IHttpClientFactory to call API AI
builder.Services.AddHttpClient();

// Thiết lập LicenseContext cho EPPlus
ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Hoặc LicenseContext.Commercial nếu bạn có giấy phép thương mại

var app = builder.Build();

// Configure the HTTP request pipeline.
// For development environment
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
//        c.DisplayRequestDuration();
//        c.EnableFilter();
//        c.EnableDeepLinking();
//    });
//}

// For production environment
//app.UseSwagger();
//app.UseSwaggerUI();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
//    c.DisplayRequestDuration();
//    c.EnableFilter();
//    c.EnableDeepLinking();
//});

//app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseRouting();
//app.UseAuthorization();

//app.UseCors();

//app.MapControllers();

//app.UseStaticFiles();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

//app.Run();

//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
//    c.DisplayRequestDuration();
//    c.EnableFilter();
//    c.EnableDeepLinking();
//});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    c.DisplayRequestDuration();
    c.EnableFilter();
    c.EnableDeepLinking();
});

app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

app.UseStaticFiles();
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllers(); // Map các Controller

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

 */