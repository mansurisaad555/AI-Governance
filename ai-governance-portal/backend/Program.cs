using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
    )
);

// EF Core + SQLite
builder.Services.AddDbContext<GovernanceDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AI Governance API",
        Version = "v1"
    });
});

var app = builder.Build();

// Automatically apply migrations & create the SQLite database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GovernanceDbContext>();
    db.Database.Migrate();
}

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AI Governance API v1");
    });
}

// Minimal CRUD endpoints

// GET all entries (admin)
app.MapGet("/api/usage", async (GovernanceDbContext db) =>
    await db.UsageEntries.ToListAsync()
);

// GET entries by user
app.MapGet("/api/usage/user/{username}", async (GovernanceDbContext db, string username) =>
    await db.UsageEntries
            .Where(e => e.Username == username)
            .ToListAsync()
);

// POST a new entry
app.MapPost("/api/usage", async (GovernanceDbContext db, UsageEntry entry) =>
{
    db.UsageEntries.Add(entry);
    await db.SaveChangesAsync();
    return Results.Created($"/api/usage/{entry.Id}", entry);
});

// PUT (update) an entry
app.MapPut("/api/usage/{id:int}", async (GovernanceDbContext db, int id, UsageEntry updated) =>
{
    var existing = await db.UsageEntries.FindAsync(id);
    if (existing is null) return Results.NotFound();
    existing.ToolName  = updated.ToolName;
    existing.DataType  = updated.DataType;
    existing.Purpose   = updated.Purpose;
    existing.Frequency = updated.Frequency;
    existing.RiskLevel = updated.RiskLevel;
    existing.Status    = updated.Status;
    await db.SaveChangesAsync();
    return Results.Ok(existing);
});

// DELETE an entry
app.MapDelete("/api/usage/{id:int}", async (GovernanceDbContext db, int id) =>
{
    var entry = await db.UsageEntries.FindAsync(id);
    if (entry is null) return Results.NotFound();
    db.UsageEntries.Remove(entry);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
