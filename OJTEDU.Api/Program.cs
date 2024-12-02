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

//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
//    c.DisplayRequestDuration();
//    c.EnableFilter();
//    c.EnableDeepLinking();
//});

//app.UseStaticFiles(); // Đặt sau Swagger nếu phục vụ file tĩnh

//app.UseHttpsRedirection();

//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{
//    ForwardedHeaders = ForwardedHeaders.XForwardedProto
//});

//app.UseCors();

//app.UseAuthentication();
//app.UseRouting();
//app.UseAuthorization();

//app.MapControllers(); // Map các Controller

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

//app.Run();


using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using OJTEDU.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// **Cấu hình Logging**
builder.Logging.ClearProviders(); // Xóa các logging provider mặc định
builder.Logging.AddConsole();    // Thêm logging vào Console
builder.Logging.AddDebug();      // Thêm Debug logging (nếu cần)

// **Thêm các dịch vụ vào container**
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = false; // Đảm bảo filter model validation được bật
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// **Cấu hình DbContext**
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.ConfigureDbContext(connectionString);

// **Cấu hình AutoMapper**
builder.Services.ConfigureAutoMapper();

// **Cấu hình Services và Repository**
builder.Services.ConfigureRepositories();

// **Cấu hình CORS**
builder.Services.ConfigureCors("https://localhost:3000");

// **Thêm HttpContext Accessor**
builder.Services.AddHttpContextAccessor();

// **Cấu hình Swagger**
builder.Services.ConfigureSwagger();

// **Cấu hình Authentication và Authorization**
builder.Services.ConfigureAuthentication();
builder.Services.ConfigureAuthorization();

// **Thêm IHttpClientFactory**
builder.Services.AddHttpClient();

// **Thiết lập LicenseContext cho EPPlus**
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// **Xây dựng ứng dụng**
var app = builder.Build();

// **Cấu hình Middleware**

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
        c.DisplayRequestDuration();
        c.EnableFilter();
        c.EnableDeepLinking();
    });
}

// **Middleware xử lý lỗi (nếu cần)**
app.UseExceptionHandler("/error"); // Endpoint để xử lý lỗi
app.UseHsts(); // Thêm HSTS cho bảo mật trong môi trường Production

// **Xử lý file tĩnh**
app.UseStaticFiles();

// **Cấu hình Forwarded Headers (nếu sử dụng reverse proxy như Nginx)**
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

// **Kích hoạt HTTPS Redirection**
app.UseHttpsRedirection();

// **Bật CORS**
app.UseCors();

// **Thêm Authentication và Authorization**
app.UseAuthentication();
app.UseAuthorization();

// **Middleware để ghi log mỗi Request/Response (tùy chọn)**
app.Use(async (context, next) =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next.Invoke();
    logger.LogInformation("Finished handling request.");
});

// **Map các controller**
app.MapControllers();

// **Chạy ứng dụng**
app.Run();
