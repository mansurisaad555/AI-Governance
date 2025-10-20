using System.Text.RegularExpressions;

namespace Backend.Services;

public class AdversarialDetector
{
    private static readonly string[] KeywordIndicators =
    [
        "ignore previous instructions",
        "override safeguards",
        "as a developer",
        "print the system prompt",
        "disregard rules",
        "sudo",
        "root access",
        "bypass security",
        "inject"
    ];

    private static readonly Regex SuspiciousPunctuation = new("[#\\\\\\"'{}$]{3,}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public AdversarialAnalysis Analyze(string input)
    {
        var lowered = input.ToLowerInvariant();
        var indicators = KeywordIndicators
            .Where(keyword => lowered.Contains(keyword))
            .ToList();

        if (SuspiciousPunctuation.IsMatch(input))
        {
            indicators.Add("Excessive special characters");
        }

        var isFlagged = indicators.Count > 0;
        return new AdversarialAnalysis(isFlagged, indicators);
    }
}

public record AdversarialAnalysis(bool IsFlagged, IReadOnlyCollection<string> Indicators);
