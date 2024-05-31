using DB;
using DB.Repositories;
using DB.Repositories.Interfaces;
using DB.Services;
using DB.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using RestProject.HATEOAS.Filters;
using RestProject.HATEOAS.Services;
using RestProject.Middleware;

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

// Dodanie obs³ugi XML
builder.Services.AddControllers().AddXmlSerializerFormatters();

// Dodanie http context accessor
builder.Services.AddHttpContextAccessor();



// Rejestracja HateoasServices i HateoasFilters
builder.Services.AddSingleton<HateoasFlightService>();
builder.Services.AddScoped<HateoasFlightFilter>();
builder.Services.AddSingleton<HateoasUserService>();
builder.Services.AddScoped<HateoasUserFilter>();
builder.Services.AddSingleton<HateoasFlightReservationService>();
builder.Services.AddScoped<HateoasFlightReservationFilter>();
builder.Services.AddSingleton<HateoasAirportService>();
builder.Services.AddScoped<HateoasAirportFilter>();



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

// Uzycie middleware do np. sprawdzenia czasy wykonywania sie metody lub autoryzacji uzytkownika
app.UseMiddleware<AddServicePathToHeader>();
app.UseMiddleware<MeasureTimeMiddleware>();
app.UseMiddleware<UserAuthenticationMiddleware>();

app.Run("https://localhost:8080");
