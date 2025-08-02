using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OracleMES.API.Middleware;
using OracleMES.Core.Interfaces.Repositories;
using OracleMES.Core.Services;
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
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));


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

// Service Dependency Injection
builder.Services.AddScoped<WorkorderService>();
builder.Services.AddScoped<MachineService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<QualityControlService>();
builder.Services.AddScoped<OEEService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<DowntimeService>();
builder.Services.AddScoped<WorkcenterService>();
builder.Services.AddScoped<MaterialConsumptionService>();



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

app.Urls.Add("http://localhost:5173");


// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
// else
// {
//     app.UseHttpsRedirection();
// }


// 라우팅 미들웨어 순서 수정
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
