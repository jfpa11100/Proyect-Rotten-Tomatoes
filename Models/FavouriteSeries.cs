namespace Proyect_Rotten_Tomatoes.Models
{
    public class FavouriteSeries
    {
        public int Id { get; set; }
        public int SerieId { get; set; }
        public Serie Serie { get; set; }

        public int CinephileId { get; set; }
        public Cinephile Cinephile { get; set; }
    }
}
