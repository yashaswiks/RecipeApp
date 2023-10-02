using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RecipeApi.Areas.Identity.Data;

public class ApplicationUser : IdentityUser
{
    [StringLength(308)]
    public string FirstName { get; set; }

    [StringLength(308)]
    public string LastName { get; set; }

    public DateTime? LastLoggedInDate { get; set; }
    public DateTime? UserCreatedOn { get; set; }
}