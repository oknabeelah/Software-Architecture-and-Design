// create an adapter owned view model
using System.ComponentModel.DataAnnotations;
//using PgrStudentManagement.Domain.DummyModule;


namespace PgrStudentManagement.Adapters.Web.ViewModels;

public sealed class DummyInputViewModel
{
    [Required]
    [StringLength(200)]
    public string Input { get; set; } = string.Empty;

    //public string Message { get; set; } = string.Empty;
    //public DateTimeOffset ProcessedAtUtc { get; set; }
}
//This model belongs to the Web adapter because it represents form input and MVC validation metadata.