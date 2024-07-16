using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.User;

public class UserOverviewDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}