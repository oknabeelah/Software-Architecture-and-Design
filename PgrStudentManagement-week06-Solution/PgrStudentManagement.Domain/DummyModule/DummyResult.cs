namespace PgrStudentManagement.Domain.DummyModule;

public sealed record DummyResult(
    string Message,
    DateTimeOffset ProcessedAtUtc);

// the timestamp belongs here because its generated as part of processing the request