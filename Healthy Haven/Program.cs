using Healthy_Haven.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Healthy_Haven.Models;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.OpenApi.Models;
using Amazon.SimpleNotificationService;
using Amazon.Runtime;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Amazon.XRay.Recorder.Handlers.EntityFramework;

AWSSDKHandler.RegisterXRayForAllServices();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
    {
        mySqlOptions.EnableRetryOnFailure();
    });
});

builder.Services.Configure<IdentityOptions>(opts =>
{
    opts.SignIn.RequireConfirmedEmail = true;
});

builder.Services.AddDefaultIdentity<ApplicationUser>().AddDefaultTokenProviders().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

var awsOptions = builder.Configuration.GetAWSOptions();

var profile = awsOptions.Profile;
var region = awsOptions.Region;

var accessKeyId = builder.Configuration["AWS:Credentials:AccessKeyId"];
var secretKey = builder.Configuration["AWS:Credentials:SecretKey"];
var sessionToken = builder.Configuration["AWS:Credentials:SessionToken"];

var awsCredentials = new SessionAWSCredentials(accessKeyId, secretKey, sessionToken);

builder.Services.AddSingleton<IAmazonSimpleNotificationService>(sp =>
{
    return new AmazonSimpleNotificationServiceClient(
        awsCredentials,
        new AmazonSimpleNotificationServiceConfig { RegionEndpoint = region });
});


var app = builder.Build();
app.UseXRay("Healthy-Haven");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
});

app.Run();