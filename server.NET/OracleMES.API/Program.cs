using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OracleMES.API.Middleware;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Infrastructure.Data;
using OracleMES.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Oracle DB Connection setup
builder.Services.AddDbContext<MesDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("ConnectionStrings:DefaultConnection")));


// Repository Dependency Injection
builder.Services.AddScoped<IMachineRepository, MachineRepository>();
builder.Services.AddScoped<IWorkorderRepository, WorkorderRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IQualitycontrolRepository, QualitycontrolRepository>();
builder.Services.AddScoped<IOeemetricRepository, OeemetricRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IDowntimeRepository, DowntimeRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IWorkcenterRepository, WorkcenterRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IMaterialconsumptionRepository, MaterialconsumptionRepository>();
builder.Services.AddScoped<IDefectRepository, DefectRepository>();
builder.Services.AddScoped<IBillofmaterialRepository, BillofmaterialRepository>();

// CORS setup
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
          policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
    );

    options.AddPolicy("ProdCors", policy =>
        policy.WithOrigins("https://my-production-app.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
}
else
{
    app.UseCors("ProdCors");
}

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
