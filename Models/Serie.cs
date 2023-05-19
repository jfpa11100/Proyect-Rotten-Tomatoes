namespace Proyect_Rotten_Tomatoes.Models
{

    public class Serie
    {
        public int Id { get; set; }
        public string url { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int Critic_Rating { get; set; }
        public int Audience_Rating { get; set; }
        public string Available_Platforms { get; set; }
        public string Synopsis { get; set; }
        public string Genre { get; set; }
        public string Serie_Team { get; set; }
        public string Principal_Actors { get; set; }
        public string Premiere_Date { get; set; }
        public bool Top { get; set; }
    }
}
