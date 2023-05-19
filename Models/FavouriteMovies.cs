namespace Proyect_Rotten_Tomatoes.Models
{
    public class FavouriteMovies
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int CinephileId { get; set; }
        public Cinephile Cinephile { get; set; }
    }
}
