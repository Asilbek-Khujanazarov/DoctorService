using Microsoft.EntityFrameworkCore;
using PatientRecoverySystem.DoctorService.Data;
using PatientRecoverySystem.DoctorService.Repositories;
using PatientRecovery.Shared.Messaging;
using System.Reflection;
using PatientRecoverySystem.DoctorService.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<PatientRecoverySystem.DoctorService.Data.DoctorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Repositories
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
// RabbitMQ
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DoctorDbContext>();
    db.Database.Migrate();
}

app.Run();