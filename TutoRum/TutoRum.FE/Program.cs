﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TutoRum.Services.IService;
using TutoRum.Services.Service;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Repositories;
using TutoRum.FE.Common;
using TutoRum.Data.Models;
using AutoMapper;
using TutoRum.Services.AutoMapper;
using TutoRum.Services.Helper;
using Xceed.Document.NET;
using TutoRum.FE.VNPay;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ProducesResponseTypeFilterAttribute>();
});

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddServer(new OpenApiServer
    {
        Url = "https://tutorconnectapiservice-hkhecjd7azg2gcfm.canadacentral-01.azurewebsites.net"
    });
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "TutorConnect API", Version = "v1" });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("https://tutor-connect-deploy-six.vercel.app", "http://localhost:3000", "http://localhost:7026", "AllowAllOrigins")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

// Configure Identity with custom Account entity
builder.Services.AddIdentity<AspNetUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
});

// Add DbContext with SQL Server connection string
// Add DbContext with SQL Server connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );

            sqlServerOptions.CommandTimeout(60); // Thời gian chờ tối đa cho mỗi lệnh là 60 giây
        }
    ));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<FeedbackMappingProfile>();
    cfg.AddProfile<QualificationLevelMapper>();
    cfg.AddProfile<TutorMappingProfile>();
    cfg.AddProfile<FapMappingProfile>();
}, AppDomain.CurrentDomain.GetAssemblies());

// Add scoped and singleton services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<ITutorLearnerSubjectService, TutorLearnerSubjectService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IDbFactory, DbFactory>();
builder.Services.AddScoped<IAdminSevice, AdminSevice>();
builder.Services.AddScoped<ITutorService, TutorService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITutorRequestService, TutorRequestService>();
builder.Services.AddScoped<IFaqService, FaqService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<ICertificatesSevice, CertificatesSevice>();
builder.Services.AddScoped<ITeachingLocationsService, TeachingLocationsService>();
builder.Services.AddScoped<IQualificationLevelService, QualificationLevelService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IBillingEntryService, BillingEntryService>();
builder.Services.AddScoped<IBillService, BillService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentRequestService, PaymentRequestService>();


builder.Services.AddScoped<APIAddress>();
builder.Services.AddScoped<ContractService>();
builder.Services.AddHttpClient<ITutorService, TutorService>();
builder.Services.AddScoped<BillingEntryService>();
builder.Services.AddHostedService<BillingEntryCronJobService>();
builder.Services.AddScoped<IFirebaseService, FirebaseService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserTokenService, UserTokenService>();
builder.Services.AddScoped<IRateRangeService, RateRangeService>();


builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

var app = builder.Build();

// 1. Xử lý file tĩnh trước tiên (nếu có)
app.UseStaticFiles();

// 2. Định tuyến
app.UseRouting();

// 3. Bật Swagger (nếu chỉ sử dụng trong môi trường Development, cần kiểm tra môi trường)
app.UseSwagger();
app.UseSwaggerUI();

// 4. HSTS và HTTPS Redirection
app.UseHttpsRedirection();

// 5. Bật CORS (phải đặt trước các middleware authentication/authorization)
app.UseCors("AllowSpecificOrigins");

// 6. Authentication
app.UseAuthentication();

// 7. Authorization
app.UseAuthorization();

// 8. Định tuyến các controller
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// 9. Định tuyến file fallback (Next.js/React.js SPA)
app.MapFallbackToFile("index.html");

// 10. Khởi chạy ứng dụng
app.Run();
