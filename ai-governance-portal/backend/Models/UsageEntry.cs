namespace Backend.Models;

public class UsageEntry
{
    public int     Id         { get; set; }
    public string  Username   { get; set; } = null!;    // employee who logged it
    public string  ToolName   { get; set; } = null!;    // e.g. ChatGPT, Copilot
    public string  DataType   { get; set; } = null!;    // e.g. PII, Marketing
    public string  Purpose    { get; set; } = null!;    // why they need it
    public int     Frequency  { get; set; }             // times per week/month
    public string  RiskLevel  { get; set; } = null!;    // Low/Medium/High
    public string  Status     { get; set; } = "Pending";    // New/In Review/Approved
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
