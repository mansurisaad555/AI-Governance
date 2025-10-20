using System.Text.Json.Serialization;

namespace Backend.Models;

public class ModelCard
{
    public int Id { get; set; }
    public int UsageEntryId { get; set; }
    [JsonIgnore]
    public UsageEntry UsageEntry { get; set; } = null!;
    public string ApprovedBy { get; set; } = "System";
    public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;
    public string FinalRiskLevel { get; set; } = "Pending";
    public string AiRiskLevel { get; set; } = "Unknown";
    public double? AiConfidence { get; set; }
    public string? AiRationale { get; set; }
    public string ComplianceChecklist { get; set; } = "None";
    public string StatusDecision { get; set; } = "Pending";
    public string? PolicyAlerts { get; set; }
    public string? Notes { get; set; }
}
