using DB;
using DB.Repositories;
using DB.Repositories.Interfaces;
using DB.Services;
using DB.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(IFlightRepository), typeof(FlightRepository));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IFlightReservationRepository), typeof(FlightReservationRepository));

builder.Services.AddScoped(typeof(IBaseService<,,>), typeof(BaseService<,,>));
builder.Services.AddScoped(typeof(IFlightService), typeof(FlightService));
builder.Services.AddScoped(typeof(IUserService), typeof(UserService));
builder.Services.AddScoped(typeof(IFlightReservationService), typeof(FlightReservationService));

builder.Services.AddDbContext<MyDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});

// Dodanie obs�ugi XML
builder.Services.AddControllers().AddXmlSerializerFormatters();

// Dodanie http context accessor
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run("https://localhost:8080");
