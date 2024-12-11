using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class BillingEntryCronJobService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BillingEntryCronJobService> _logger;
        private Timer _timer;

        public BillingEntryCronJobService(IServiceProvider serviceProvider, ILogger<BillingEntryCronJobService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BillingEntryCronJobService started at {time}", DateTime.UtcNow);

            // Set the timer to execute every 5 seconds or 1 hour
            //_timer = new Timer(ExecuteJob, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            _timer = new Timer(ExecuteJob, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private async void ExecuteJob(object state)
        {
            _logger.LogInformation("Cron Job started at {time}", DateTime.UtcNow);

            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var billingEntryService = scope.ServiceProvider.GetRequiredService<BillingEntryService>();
                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    DateTime currentDateTime = DateTime.Now;
                    int currentDayOfWeek = (int)currentDateTime.DayOfWeek + 1;
                    var c = unitOfWork.schedule.GetMulti(s => s.TutorLearnerSubjectId != null).ToList();
                    var schedules = unitOfWork.schedule.GetMulti(schedule =>
                        schedule.EndTime < currentDateTime.TimeOfDay && schedule.DayOfWeek == currentDayOfWeek
                        && schedule.TutorLearnerSubjectId != null);

                    _logger.LogInformation("Found {count} schedules to process at {time}", schedules.Count(), DateTime.UtcNow);

                    if (schedules.Any())
                    {
                        foreach (var schedule in schedules)
                        {
                            _logger.LogInformation("Processing schedule ID {scheduleId}", schedule.ScheduleId);

                            var tutorLearnerSubject = await unitOfWork.TutorLearnerSubject.GetTutorLearnerSubjectAsyncById(schedule.TutorLearnerSubjectId ?? 0);

                            if (tutorLearnerSubject != null && tutorLearnerSubject.PricePerHour.HasValue && tutorLearnerSubject.IsContractVerified == true)
                            {
                                _logger.LogInformation("Found TutorLearnerSubject ID {tutorLearnerSubjectId} with rate {rate}",
                                    tutorLearnerSubject.TutorLearnerSubjectId, tutorLearnerSubject.PricePerHour.Value);

                                // Tính toán thời gian
                                var startDateTime = DateTime.Today.Add(schedule.StartTime ?? TimeSpan.Zero);
                                var endDateTime = DateTime.Today.Add(schedule.EndTime ?? TimeSpan.Zero);

                                decimal totalAmount = billingEntryService.CalculateTotalAmount(
                                    startDateTime,
                                    endDateTime,
                                    tutorLearnerSubject.PricePerHour.Value);

                                _logger.LogInformation("Calculated total amount {totalAmount} for schedule ID {scheduleId}",
                                    totalAmount, schedule.ScheduleId);

                                var billingEntryDTO = new AdddBillingEntryDTO
                                {
                                    TutorLearnerSubjectId = tutorLearnerSubject.TutorLearnerSubjectId,
                                    Rate = tutorLearnerSubject.PricePerHour.Value,
                                    StartDateTime = startDateTime,
                                    EndDateTime = endDateTime,
                                    TotalAmount = totalAmount,
                                    Description = "Auto-generated billing entry based on schedule"
                                };

                                if (await billingEntryService.IsValidBillingEntry(billingEntryDTO))
                                {
                                    await billingEntryService.AddBillingEntryAsync(billingEntryDTO, null);

                                    var notificationDto = new NotificationRequestDto
                                    {
                                        UserId = tutorLearnerSubject.TutorSubject.TutorId ?? new Guid(),
                                        Title = "Đã ghi nhận 1 buổi học mới",
                                        Description = $"Buổi học lúc {startDateTime.ToString("HH:mm")} - {endDateTime.ToString("HH:mm")} của lớp {tutorLearnerSubject.TutorLearnerSubjectId} ngày hôm nay đã được ghi nhận",
                                        NotificationType = Data.Enum.NotificationType.BillingEntryAdd,
                                        Href = $"/user/teaching-classrooms/{tutorLearnerSubject.TutorLearnerSubjectId}/billing-entries/"
                                    };

                                    await notificationService.SendNotificationAsync(notificationDto, false);
                                    _logger.LogInformation("Billing entry created for schedule ID {scheduleId}", schedule.ScheduleId);
                                }
                                else
                                {
                                    _logger.LogWarning("Billing entry is not valid for schedule ID {scheduleId}", schedule.ScheduleId);
                                }
                            }
                            else
                            {
                                _logger.LogWarning("TutorLearnerSubject not found or rate not available for schedule ID {scheduleId}", schedule.ScheduleId);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No schedules found to process at {time}", DateTime.UtcNow);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while executing Cron Job at {time}", DateTime.UtcNow);
                }
            }

            _logger.LogInformation("Cron Job finished at {time}", DateTime.UtcNow);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BillingEntryCronJobService stopped at {time}", DateTime.UtcNow);

            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _logger.LogInformation("BillingEntryCronJobService disposed at {time}", DateTime.UtcNow);
        }
    }
}
