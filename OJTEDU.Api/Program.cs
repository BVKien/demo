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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    c.DisplayRequestDuration();
    c.EnableFilter();
    c.EnableDeepLinking();
});

app.UseStaticFiles(); // Đặt sau Swagger nếu phục vụ file tĩnh

app.UseHttpsRedirection();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

app.UseCors();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllers(); // Map các Controller

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
