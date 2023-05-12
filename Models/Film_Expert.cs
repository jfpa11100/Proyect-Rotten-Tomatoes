using Microsoft.AspNetCore.Identity;

namespace Proyect_Rotten_Tomatoes.Models
{
    public class Film_Expert
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password{ get; set; }
    }
}
