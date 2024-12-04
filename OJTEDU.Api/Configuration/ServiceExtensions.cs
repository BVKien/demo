using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Application.ApplicationServices.Services;
using OJTEDU.Application.Profiles;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using OJTEDU.Infrastructure.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OJTEDU.Api.Configuration
{
    public static class ServiceExtensions
    {
        public static void ConfigureDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<OJTEDU_DB_V1Context>(options => options.UseSqlServer(connectionString));
        }

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Program));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            // example: services.AddScoped<IUserRepository, UserRepository>();


            // Repositories 
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<INewsFaqRepository, NewsFaqRepository>();
            services.AddScoped<IPolicyRepository, PolicyRepository>();
            services.AddScoped<IBannerRepository, BannerRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICvRepository, CvRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAppllicationRepository, AppllicationRepository>();
            services.AddScoped<IWorkingReportRepository, WorkingReportRepository>();
            services.AddScoped<IAttendanceReportRepository, AttendanceReportRepository>();
            services.AddScoped<ICompanyProposalRepository, CompanyProposalRepository>();
            services.AddScoped<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<IDistrictRepository, DistrictRepository>();
            services.AddScoped<IWardRepository, WardRepository>();
            services.AddScoped<IMajorRepository, MajorRepository>();
            services.AddScoped<IGroupChatRepository, GroupChatRepository>();
            services.AddScoped<IMessageGroupRepository, MessageGroupRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<ISupportRequestRepository, SupportRequestRepository>();
            services.AddScoped<IInternshipRepository, InternshipRepository>();
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ISemesterRepository, SemesterRepository>();
            services.AddScoped<IUserGuideRepository, UserGuideRepository>();
            services.AddScoped<IInternshipProcessRepository, InternshipProcessRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IGoogleJsonWebSignatureValidator, GoogleJsonWebSignatureValidator>();

            // Services 
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IDocumentService, DocumentService>();
            services.AddScoped<INewsFaqService, NewsFaqService>();
            services.AddScoped<IPolicyService, PolicyService>();
            services.AddScoped<IBannerService, BannerService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICvService, CvService>();
            services.AddScoped<IAppllicationService, AppllicationService>();
            services.AddScoped<IWorkingReportService, WorkingReportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAttendanceReportService, AttendanceReportService>();
            services.AddScoped<ICompanyProposalService, CompanyProposalService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IMajorService, MajorService>();
            services.AddScoped<IGroupChatService, GroupChatService>();
            services.AddScoped<IMessageGroupService, MessageGroupService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<ISupportRequestService, SupportRequestService>();
            services.AddScoped<IInternshipService, InternshipService>();
            services.AddScoped<IContractService, ContractService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<ISemesterService, SemesterService>();
            services.AddScoped<IUserGuideService, UserGuideService>();
            services.AddScoped<IInternshipProcessService, InternshipProcessService>();
            services.AddHostedService<AutoAttendanceReportService>();
        }

        public static void ConfigureCors(this IServiceCollection services, string origin)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins(origin)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials()
                           .WithExposedHeaders("Content-Disposition");
                });
            });
        }

        //AllowSpecificOrigin
        //public static void ConfigureCors(this IServiceCollection services)
        //{
        //    services.AddCors(options =>
        //    {
        //        options.AddPolicy("AllowSpecificOrigin", builder =>
        //        {
        //            builder.WithOrigins("https://www.ojtedu.site")
        //                   .AllowAnyMethod()
        //                   .AllowAnyHeader()
        //                   .AllowCredentials()
        //                   .WithExposedHeaders("Content-Disposition");
        //        });
        //    });
        //}

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

                // Đăng ký khả năng upload file
                c.OperationFilter<SwaggerFileOperationFilter>();

                // Cấu hình xác thực cookie cho Swagger
                c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Cookie,
                    Description = "Cookie authentication",
                    Name = "authToken",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "cookie"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "cookieAuth"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }

        public static void ConfigureAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "OJTEDU"; // Tên của cookie
                options.Cookie.HttpOnly = true; // Ngăn chặn JavaScript truy cập cookie
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Cookie chỉ được gửi qua HTTPS
                options.Cookie.SameSite = SameSiteMode.None; // Ngăn chặn CSRF | SameSiteMode.Strict;
                options.Cookie.MaxAge = TimeSpan.FromHours(3); // Hết hạn sau 3 tiếng
                options.SlidingExpiration = true; // Làm mới thời gian hết hạn mỗi lần truy cập
            });
        }

        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();
        }


    }

    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.MethodInfo
                .GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile));

            if (fileParameters.Any())
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["file"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}
