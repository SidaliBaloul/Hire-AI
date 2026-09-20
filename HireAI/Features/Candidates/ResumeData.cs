namespace HireAI.Features.Candidates;

public sealed record ResumeData(string FullName, string? Email, string? Phone, List<string> Skills, List<ExperienceData> Experiences);

public sealed record ExperienceData(string Company, string JobTitle, string? Description, int? YearsOfExperience);
