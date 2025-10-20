namespace Backend.Services;

public class ComplianceMapper
{
    private static readonly string[] GdprKeywords =
    [
        "gdpr",
        "general data protection regulation",
        "eu personal data"
    ];

    private static readonly string[] HipaaKeywords =
    [
        "hipaa",
        "medical record",
        "phi",
        "patient data"
    ];

    private static readonly string[] FinancialKeywords =
    [
        "pci",
        "credit card",
        "ssn",
        "social security",
        "financial"
    ];

    public ComplianceRecommendation BuildChecklist(string riskLevel, string dataType, IEnumerable<string> policyAlerts, string narrative)
    {
        var tasks = new List<string>();
        var majorViolations = new List<string>();
        var alerts = new HashSet<string>(policyAlerts.Select(p => p.ToLowerInvariant()));
        var loweredDataType = dataType.ToLowerInvariant();
        var loweredNarrative = narrative.ToLowerInvariant();

        if (alerts.Overlaps(GdprKeywords) || GdprKeywords.Any(keyword => loweredNarrative.Contains(keyword) || loweredDataType.Contains(keyword)))
        {
            majorViolations.Add("GDPR data handling risk");
            tasks.Add("Data Protection Officer review (GDPR)");
            tasks.Add("Record processing activity in Article 30 register");
        }

        if (alerts.Overlaps(HipaaKeywords) || HipaaKeywords.Any(keyword => loweredNarrative.Contains(keyword) || loweredDataType.Contains(keyword)))
        {
            majorViolations.Add("HIPAA compliance risk");
            tasks.Add("Legal counsel sign-off for HIPAA");
            tasks.Add("Business Associate Agreement verification");
        }

        if (alerts.Overlaps(FinancialKeywords) || FinancialKeywords.Any(keyword => loweredNarrative.Contains(keyword) || loweredDataType.Contains(keyword)))
        {
            tasks.Add("PCI DSS security checklist");
        }

        switch (riskLevel?.ToLowerInvariant())
        {
            case "high":
                tasks.Add("Executive approval required");
                tasks.Add("NIST AI RMF high-risk assessment");
                break;
            case "medium":
                tasks.Add("Team lead manual review");
                tasks.Add("Privacy impact assessment");
                break;
            default:
                tasks.Add("Auto-approval eligible");
                break;
        }

        if (loweredDataType.Contains("pii") || loweredNarrative.Contains("personal data"))
        {
            tasks.Add("Data minimization checklist");
        }

        var recommendation = riskLevel?.ToLowerInvariant() switch
        {
            "low" => "Auto-approve",
            "medium" => "Manual review recommended",
            "high" => "Escalate for manual approval",
            _ => "Pending assessment"
        };

        if (majorViolations.Count > 0)
        {
            recommendation = "Auto-deny";
        }

        return new ComplianceRecommendation(
            Checklist: string.Join("; ", tasks.Distinct()),
            Recommendation: recommendation,
            MajorViolations: majorViolations
        );
    }
}

public record ComplianceRecommendation(string Checklist, string Recommendation, IReadOnlyCollection<string> MajorViolations);
