using System.ComponentModel.DataAnnotations;

namespace Aspnet.Models;

public class Country
{
    [Key]
    public string Name { get; init; }
    public string Capital { get; init; }
    public string Currency { get; init; }
}
