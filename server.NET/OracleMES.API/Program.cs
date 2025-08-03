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
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Program).Assembly)
    .AddControllersAsServices();

// 로깅 레벨 설정
builder.Logging.SetMinimumLevel(LogLevel.Debug);
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
          policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
    );
});

var app = builder.Build();

// 서버 시작 로그
Console.WriteLine("=== MES API Server Started ===");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"Start Time: {DateTime.Now}");

// DB 연결 테스트
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<MesDbContext>();
    var canConnect = await dbContext.Database.CanConnectAsync();
    Console.WriteLine($"DB Connection: {(canConnect ? "✅ Success" : "❌ Failed")}");
    
    // 설비 개수 확인
    var machineCount = await dbContext.Machines.CountAsync();
    Console.WriteLine($"Machine Count: {machineCount}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ DB Connection Failed: {ex.Message}");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Development Mode");
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevCors");
}
else
{
    Console.WriteLine("Production Mode");
    app.UseHttpsRedirection();
    app.UseCors("DevCors"); // temporary setting for development
}

app.UseRouting();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapControllers();

Console.WriteLine("=== Controller Mapping Complete ===");
Console.WriteLine("Server is running...");

app.Run();
