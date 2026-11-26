using PgrStudentManagement.Domain.DummyModule;

namespace PgrStudentManagement.Business.DummyModule;

public sealed class DummyService : IDummyService
{
    public DummyResult Process(string input)
    {
        var normalisedInput = input.Trim();

        return new DummyResult(
            $"Hello World, this is your input: " + normalisedInput,
            DateTimeOffset.UtcNow);
    }
}
// the service is responsible for trimming input, constructing output message, and capturing DateTimeOffset.UtcNow
// the service doesn't know whether the request came from a public or private page
// authentication is an incoming adapter concern at this stage