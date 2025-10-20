using System.IO;
using Backend.Data;
using Backend.Models;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
var sqliteConnectionString = EnsureSqlitePath(
    builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=data/governance.db",
    builder.Environment.ContentRootPath
);

builder.Services.AddDbContext<GovernanceDbContext>(options =>
    options.UseSqlite(sqliteConnectionString)
);

// Risk assessment + cybersecurity services
builder.Services.Configure<RiskAssessmentOptions>(builder.Configuration.GetSection("RiskAssessment"));
builder.Services.AddHttpClient<IRiskAssessmentClient, RiskAssessmentClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<RiskAssessmentOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
});

builder.Services.AddSingleton<AdversarialDetector>();
builder.Services.AddSingleton<ComplianceMapper>();

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

// Apply any pending migrations & create the SQLite database
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

// Minimal CRUD endpoints with AI governance integration

// GET all entries (admin)
app.MapGet("/api/usage", async (GovernanceDbContext db) =>
    await db.UsageEntries
            .Include(e => e.ModelCard)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync()
);

// GET single entry by id (for edit form)
app.MapGet("/api/usage/{id:int}", async (GovernanceDbContext db, int id) =>
{
    var entry = await db.UsageEntries
                        .Include(e => e.ModelCard)
                        .FirstOrDefaultAsync(e => e.Id == id);
    return entry is not null ? Results.Ok(entry) : Results.NotFound();
});

// GET entries by user
app.MapGet("/api/usage/user/{username}", async (GovernanceDbContext db, string username) =>
    await db.UsageEntries
            .Include(e => e.ModelCard)
            .Where(e => e.Username == username)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync()
);

// POST a new entry with AI risk assessment + security checks
app.MapPost("/api/usage", async (
    GovernanceDbContext db,
    UsageEntry entry,
    IRiskAssessmentClient riskClient,
    AdversarialDetector adversarialDetector,
    ComplianceMapper complianceMapper,
    CancellationToken cancellationToken) =>
{
    entry.Id = 0;
    entry.CreatedAt = DateTime.UtcNow;

    // Adversarial attack detection
    var attackAnalysis = adversarialDetector.Analyze($"{entry.ToolName} {entry.Purpose}");
    entry.AdversarialFlag = attackAnalysis.IsFlagged;
    entry.AdversarialIndicators = attackAnalysis.IsFlagged
        ? string.Join(", ", attackAnalysis.Indicators)
        : null;

    // External ML risk assessment
    var assessment = await riskClient.AssessAsync(entry.ToolName, entry.DataType, entry.Purpose, cancellationToken);
    entry.AiRiskLevel = assessment.RiskLevel;
    entry.AiConfidence = assessment.Confidence;
    entry.AiRationale = assessment.Rationale;
    entry.ModelName = assessment.ModelName;
    entry.ModelVersion = assessment.ModelVersion;
    entry.PolicyAlerts = assessment.PolicyAlerts.Count > 0
        ? string.Join(", ", assessment.PolicyAlerts)
        : null;
    entry.AssessedAt = DateTime.UtcNow;

    if (string.IsNullOrWhiteSpace(entry.RiskLevel) || entry.RiskLevel.Equals("Pending", StringComparison.OrdinalIgnoreCase))
    {
        entry.RiskLevel = assessment.RiskLevel;
    }

    var compliance = complianceMapper.BuildChecklist(
        entry.RiskLevel,
        entry.DataType,
        assessment.PolicyAlerts,
        entry.Purpose
    );

    entry.ComplianceChecklist = compliance.Checklist;
    entry.AiRecommendation = compliance.Recommendation;
    entry.MajorViolations = compliance.MajorViolations.Count > 0
        ? string.Join(", ", compliance.MajorViolations)
        : null;

    if (entry.AdversarialFlag)
    {
        entry.AiRecommendation = "Manual review required (adversarial indicators detected)";
        entry.AutoDecisionSource = "AdversarialFilter";
    }

    if (compliance.MajorViolations.Count > 0)
    {
        entry.Status = "Denied";
        entry.DenialReason = $"Auto-denied due to: {entry.MajorViolations}";
        entry.AutoDecisionSource = "AI Compliance Guard";
    }
    else if (string.IsNullOrWhiteSpace(entry.Status))
    {
        entry.Status = "Pending";
    }

    db.UsageEntries.Add(entry);
    await db.SaveChangesAsync(cancellationToken);

    return Results.Created($"/api/usage/{entry.Id}", entry);
});

// PUT (update) an entry and refresh compliance automation
app.MapPut("/api/usage/{id:int}", async (
    GovernanceDbContext db,
    int id,
    UsageEntry updated,
    ComplianceMapper complianceMapper,
    CancellationToken cancellationToken) =>
{
    var existing = await db.UsageEntries
                           .Include(e => e.ModelCard)
                           .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    if (existing is null) return Results.NotFound();

    var previousStatus = existing.Status;

    existing.ToolName = updated.ToolName;
    existing.DataType = updated.DataType;
    existing.Purpose = updated.Purpose;
    existing.Frequency = updated.Frequency;

    if (!string.IsNullOrWhiteSpace(updated.RiskLevel))
    {
        existing.RiskLevel = updated.RiskLevel;
    }

    if (!string.IsNullOrWhiteSpace(updated.Status))
    {
        existing.Status = updated.Status;
    }

    existing.DenialReason = updated.DenialReason ?? existing.DenialReason;

    var policyAlerts = string.IsNullOrWhiteSpace(existing.PolicyAlerts)
        ? Array.Empty<string>()
        : existing.PolicyAlerts.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    var compliance = complianceMapper.BuildChecklist(
        existing.RiskLevel,
        existing.DataType,
        policyAlerts,
        existing.Purpose
    );

    existing.ComplianceChecklist = compliance.Checklist;
    existing.AiRecommendation = compliance.Recommendation;
    existing.MajorViolations = compliance.MajorViolations.Count > 0
        ? string.Join(", ", compliance.MajorViolations)
        : null;

    if (compliance.MajorViolations.Count > 0 && existing.Status != "Denied")
    {
        existing.Status = "Denied";
        existing.DenialReason ??= $"Auto-denied due to: {existing.MajorViolations}";
        existing.AutoDecisionSource ??= "AI Compliance Guard";
    }

    await db.SaveChangesAsync(cancellationToken);

    if (previousStatus != "Approved" && existing.Status == "Approved")
    {
        var existingCard = await db.ModelCards
                                   .FirstOrDefaultAsync(c => c.UsageEntryId == existing.Id, cancellationToken);
        if (existingCard is null)
        {
            var card = new ModelCard
            {
                UsageEntryId = existing.Id,
                ApprovedAt = DateTime.UtcNow,
                ApprovedBy = existing.Username,
                FinalRiskLevel = existing.RiskLevel,
                AiRiskLevel = existing.AiRiskLevel,
                AiConfidence = existing.AiConfidence,
                AiRationale = existing.AiRationale,
                ComplianceChecklist = existing.ComplianceChecklist,
                StatusDecision = existing.Status,
                PolicyAlerts = existing.PolicyAlerts,
                Notes = existing.DenialReason
            };

            db.ModelCards.Add(card);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    return Results.Ok(existing);
});

// DELETE an entry
app.MapDelete("/api/usage/{id:int}", async (GovernanceDbContext db, int id, CancellationToken cancellationToken) =>
{
    var entry = await db.UsageEntries.FindAsync(new object?[] { id }, cancellationToken);
    if (entry is null) return Results.NotFound();

    db.UsageEntries.Remove(entry);
    await db.SaveChangesAsync(cancellationToken);
    return Results.NoContent();
});

// GET model cards (audit trail)
app.MapGet("/api/modelcards", async (GovernanceDbContext db) =>
    await db.ModelCards
            .Include(c => c.UsageEntry)
            .OrderByDescending(c => c.ApprovedAt)
            .ToListAsync()
);

app.Run();

static string EnsureSqlitePath(string connectionString, string contentRoot)
{
    const string dataSourceKey = "Data Source=";
    var index = connectionString.IndexOf(dataSourceKey, StringComparison.OrdinalIgnoreCase);
    if (index >= 0)
    {
        var pathValue = connectionString[(index + dataSourceKey.Length)..].Trim();
        if (!Path.IsPathRooted(pathValue))
        {
            var absolute = Path.Combine(contentRoot, pathValue);
            Directory.CreateDirectory(Path.GetDirectoryName(absolute)!);
            return $"{dataSourceKey}{absolute}";
        }

        var directory = Path.GetDirectoryName(pathValue);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    return connectionString;
}
