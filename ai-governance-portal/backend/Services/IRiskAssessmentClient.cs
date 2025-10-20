using Backend.Services.Models;

namespace Backend.Services;

public interface IRiskAssessmentClient
{
    Task<RiskAssessmentResult> AssessAsync(string toolName, string dataType, string purpose, CancellationToken cancellationToken = default);
}
