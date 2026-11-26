// create a business contract for processing the supplied input

using PgrStudentManagement.Domain.DummyModule;

namespace PgrStudentManagement.Business.DummyModule;

public interface IDummyService
{
    DummyResult Process(string input);
    //Task<DummyResult> ProcessAsync(string input);
}