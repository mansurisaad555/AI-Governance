namespace Backend.Services.Models;

public record RiskAssessmentResult(
    string RiskLevel,
    double Confidence,
    string Rationale,
    string ModelName,
    string ModelVersion,
    IReadOnlyCollection<string> PolicyAlerts
);
