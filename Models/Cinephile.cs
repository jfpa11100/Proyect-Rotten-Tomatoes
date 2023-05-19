using Microsoft.AspNetCore.Identity;

namespace Proyect_Rotten_Tomatoes.Models
{
    public class Cinephile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<FavouriteMovies> FavouriteMovies { get; set; }
        public List<FavouriteSeries> FavouriteSeries { get; set; }
    }
}
