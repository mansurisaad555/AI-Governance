namespace Backend.Models;

public class UsageEntry
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;    // employee who logged it
    public string ToolName { get; set; } = null!;    // e.g. ChatGPT, Copilot
    public string DataType { get; set; } = null!;    // e.g. PII, Marketing
    public string Purpose { get; set; } = null!;    // why they need it
    public int Frequency { get; set; }             // times per week/month
    public string RiskLevel { get; set; } = "Pending"; // Final risk level after review
    public string Status { get; set; } = "Pending";    // Pending/Approved/Denied
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // AI governance insights
    public string AiRiskLevel { get; set; } = "Unknown";
    public double? AiConfidence { get; set; }
    public string? AiRationale { get; set; }
    public string? AiRecommendation { get; set; }
    public DateTime? AssessedAt { get; set; }
    public string? ModelName { get; set; }
    public string? ModelVersion { get; set; }
    public string? PolicyAlerts { get; set; }

    // Cybersecurity + compliance automation
    public string ComplianceChecklist { get; set; } = "None";
    public string? MajorViolations { get; set; }
    public bool AdversarialFlag { get; set; }
    public string? AdversarialIndicators { get; set; }

    // Decision context
    public string? DenialReason { get; set; }
    public string? AutoDecisionSource { get; set; }

    // Data lineage / audit trail
    public ModelCard? ModelCard { get; set; }
}
