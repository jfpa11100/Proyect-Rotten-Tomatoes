namespace Proyect_Rotten_Tomatoes.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string url { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int Critic_Rating { get; set; }
        public int Audience_Rating { get; set; }
        public string Critic_Comments{ get; set; }
        public string Audience_Comments { get; set; }
        public string Available_Platforms{ get; set; }
        public string Synopsis { get; set; }
        public string Rating { get; set; }
        public string Genre { get; set; }
        public string Film_Team{ get; set;}
        public string Duration{ get; set; }
        public string Principal_Actors{ get; set; }
        public  string Release_Date { get; set; }
        public bool Top { get; set; }
    }
}
