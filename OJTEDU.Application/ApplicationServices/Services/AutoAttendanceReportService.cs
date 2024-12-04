using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OJTEDU.Application.ApplicationServices.Interfaces;
using OJTEDU.Domain.Entities;
using System;
using System.ComponentModel;

namespace OJTEDU.Application.ApplicationServices.Services
{
    public class AutoAttendanceReportService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        // private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // TimeSpan.FromHours(24); // for test
        public AutoAttendanceReportService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = GetVietnamTime();
                    var nextRun = now.Date.AddDays(1).AddHours(2);
                    var delay = nextRun - now;
                    Console.WriteLine($"Next execution scheduled at: {nextRun}");
                    await Task.Delay(delay, stoppingToken);

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var attendanceService = scope.ServiceProvider.GetRequiredService<IAttendanceReportService>();
                        var companyService = scope.ServiceProvider.GetRequiredService<ICompanyService>();

                        var userIds = await companyService.GetAllMentorsInfoAsync();

                        foreach (var data in userIds.Data)
                        {
                            var checkInTime = data.CheckInTime ?? new TimeSpan(8, 0, 0); // Default 8:00 AM
                            var checkOutTime = data.CheckOutTime ?? new TimeSpan(17, 0, 0); // Default 5:00 PM

                            try
                            {
                                await attendanceService.CreateAutoAttendanceReportAsync(data.UserId, checkInTime, checkOutTime);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error creating attendance report for mentor {data.UserId}: {ex.Message}");
                            }
                        }
                    }
                    //await Task.Delay(_interval, stoppingToken); // for test
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in AutoAttendanceReportService: {ex.Message}");
                }
            }
        }

        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone;
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Windows
                }
                else
                {
                    vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"); // Linux/macOS
                }
            }
            catch (TimeZoneNotFoundException)
            {
                Console.WriteLine("Không tìm thấy múi giờ, sử dụng UTC làm mặc định.");
                vietnamTimeZone = TimeZoneInfo.Utc; // Fallback nếu không tìm thấy múi giờ
            }

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}