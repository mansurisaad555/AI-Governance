namespace Backend.Services;

public class RiskAssessmentOptions
{
    public string BaseUrl { get; set; } = "http://localhost:8000";
    public int TimeoutSeconds { get; set; } = 20;
}
