using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using server.Configs;
using server.Controllers;
using server.Models;
using DotNetEnv;
using server.Middleware;
using server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc;
using server.Filter;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using server.Services.RatingRepository;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddCorsPolicy();

// Add services to the container.
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<ISpecialty, SpecialtyServices>();
builder.Services.AddScoped<IService, ServiceServices>();
builder.Services.AddScoped<IUser, UserServices>();
builder.Services.AddScoped<IDoctor, DoctorServices>();
builder.Services.AddScoped<IPatient, PatientServices>();
builder.Services.AddScoped<IAppointment, AppointmentServices>();
builder.Services.AddScoped<IMedicine, MedicineService>();
builder.Services.AddScoped<IMedicalRecord, MedicalRecordService>();
builder.Services.AddScoped<IAuth, AuthServices>();
builder.Services.AddScoped<IReview, ReviewServices>();
builder.Services.AddScoped<IContact, ContactServices>();
//Connect Momo API Payment
// Binding config cho MOMO
builder.Services.AddOptions<MomoOptionModel>()
    .Bind(builder.Configuration.GetSection("MomoAPI"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
    
builder.Services.AddDbContext<ClinicManagementContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    )
);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ClinicManagementContext>()
    .AddDefaultTokenProviders();

builder.Services.AddJWT(builder.Configuration); 

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddControllers(options =>
{
    // options.Filters.Add<ValidationFilter>();
    options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
});



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();


builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("_allowSpecificOrigins");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.MapControllers();

app.Run();