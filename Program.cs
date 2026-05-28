using Microsoft.EntityFrameworkCore;
using Proyecto_Final.Data;
using Proyecto_Final.Services;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────
// 1. CONTROLADORES
// ─────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ─────────────────────────────────────────────────────────────
// 2. BASE DE DATOS — Entity Framework Core con SQL Server
//    La cadena de conexión está en appsettings.json -> "Conexion"
// ─────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Conexion")));

// ─────────────────────────────────────────────────────────────
// 3. INYECCIÓN DE DEPENDENCIAS — Servicios de negocio
//    AddScoped: una instancia por petición HTTP
// ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<ProductoServices>();
builder.Services.AddScoped<InventarioServices>();
builder.Services.AddScoped<VentaServices>();

// ─────────────────────────────────────────────────────────────
// 4. CORS — Permitir peticiones desde el frontend (React)
// ─────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .AllowAnyOrigin()   // En producción cambiar por la URL del frontend
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ─────────────────────────────────────────────────────────────
// 5. SWAGGER — Documentación automática de la API
// ─────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Proyecto Final POS — API",
        Version = "v1",
        Description = "API REST para el sistema POS de Grupo Los García. " +
                      "Gestiona ventas, inventario, reportes y catálogos."
    });

    // Incluir los comentarios XML de documentación
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// ─────────────────────────────────────────────────────────────
// BUILD
// ─────────────────────────────────────────────────────────────
var app = builder.Build();

// ─────────────────────────────────────────────────────────────
// MIDDLEWARE
// ─────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "POS API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz: https://localhost:xxxx/
    });
}

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
