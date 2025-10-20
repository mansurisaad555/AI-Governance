using System.Net.Http.Json;
using Backend.Services.Models;
using Microsoft.Extensions.Options;

namespace Backend.Services;

public class RiskAssessmentClient : IRiskAssessmentClient
{
    private readonly HttpClient _httpClient;
    private readonly RiskAssessmentOptions _options;

    public RiskAssessmentClient(HttpClient httpClient, IOptions<RiskAssessmentOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    public async Task<RiskAssessmentResult> AssessAsync(string toolName, string dataType, string purpose, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            toolName,
            dataType,
            purpose
        };

        using var response = await _httpClient.PostAsJsonAsync("/assess", payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        var apiResponse = await response.Content.ReadFromJsonAsync<RiskAssessmentApiResponse>(cancellationToken: cancellationToken)
                          ?? throw new InvalidOperationException("Risk assessment service returned an empty response");

        return new RiskAssessmentResult(
            apiResponse.RiskLevel,
            apiResponse.Confidence,
            apiResponse.Rationale,
            apiResponse.ModelName,
            apiResponse.ModelVersion,
            apiResponse.PolicyAlerts ?? Array.Empty<string>()
        );
    }

    private sealed record RiskAssessmentApiResponse(
        string RiskLevel,
        double Confidence,
        string Rationale,
        string ModelName,
        string ModelVersion,
        IReadOnlyCollection<string>? PolicyAlerts
    );
}
