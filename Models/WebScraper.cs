using HtmlAgilityPack;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Proyect_Rotten_Tomatoes.Data;
using System;
using System.Runtime.InteropServices.ObjectiveC;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

namespace Proyect_Rotten_Tomatoes.Models
{
    public class WebScraper
    {
        public static async Task<string> Call_url(string url)
        {
            HttpClient client = new();
            var response = await client.GetStringAsync(url);
            return response;
        }
        public static List<object> Parse_html_Movie(string html, string url)
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);

            // Image 
            string img = htmlDoc.DocumentNode.Descendants("img")
               .Where(node => node.GetAttributeValue("alt", "").Contains("Watch trailer")).
               ToList()[0].GetAttributeValue("src", "");

            //Critic_Rating 
            string critic_rating_parsed = "";
            int critic_rating = 1;
            try 
            {
                critic_rating_parsed = htmlDoc.DocumentNode.Descendants("score-board")
                   .Where(node => node.GetAttributeValue("data-qa", "").Contains("score-panel"))
                   .ToList()[0].GetAttributeValue("tomatometerscore", "");
                    critic_rating = Convert.ToInt32(critic_rating_parsed);
            }
            catch
            {
                critic_rating = 1; 
            }


            //Audience_Rating 
            int audience_rating = 1;
            try
            {
                string audience_rating_parsed = htmlDoc.DocumentNode.Descendants("score-board")
               .Where(node => node.GetAttributeValue("data-qa", "").Contains("score-panel")).
               ToList()[0].GetAttributeValue("audiencescore", "");
                audience_rating = Convert.ToInt32(audience_rating_parsed);
            }
            catch
            {
                audience_rating = 1;
            }

            //string Available_Platforms 
            string available_platforms = "";
            try
            {
                var platforms_parsed = htmlDoc.DocumentNode.Descendants("bubbles-overflow-container")
                   .Where(node => node.GetAttributeValue("data-curation", "").Contains("rt-affiliates-sort-order")).
                   ToList();
                for (int i=3; i < platforms_parsed[0].ChildNodes.Count; i++)
                {
                    available_platforms += platforms_parsed[0].ChildNodes[i].GetAttributeValue("affiliate", "")+" ";
                }
            }
            catch
            {
                available_platforms = " ";
            }

            //string Synopsis 
            string synopsis = htmlDoc.DocumentNode.Descendants("p")
               .Where(node => node.GetAttributeValue("data-qa", "").Contains("movie-info-synopsis")).ToList()[0].InnerText;
            synopsis = synopsis.Replace("\n", "");
            synopsis = synopsis.Trim();

            //string Rating 
            string rating = htmlDoc.DocumentNode.Descendants("span")
               .Where(node => node.GetAttributeValue("class", "").Contains("info-item-value")).ToList()[0].InnerText;
            rating = rating.Replace("\n", "");
            rating = rating.Trim();

            //string Genre 
            string genre = htmlDoc.DocumentNode.Descendants("ul")
               .Where(node => node.GetAttributeValue("id", "").Contains("info")).ToList()[0].ChildNodes[3].InnerText;
            genre = genre.Replace("\n", "");
            genre = genre.Replace(" Genre:", "");
            genre = genre.Trim();
            genre = Regex.Replace(genre, @"\s+", " ");

            //string Film_Team 
            string director = htmlDoc.DocumentNode.Descendants("ul")
               .Where(node => node.GetAttributeValue("id", "").Contains("info")).ToList()[0].ChildNodes[7].InnerText;
            director = director.Replace("\n", "");
            director = director.Trim();

            string producer = htmlDoc.DocumentNode.Descendants("ul")
               .Where(node => node.GetAttributeValue("id", "").Contains("info")).ToList()[0].ChildNodes[9].InnerText;
            producer = producer.Replace("\n", "");
            producer = producer.Trim();

            string writer = htmlDoc.DocumentNode.Descendants("ul")
               .Where(node => node.GetAttributeValue("id", "").Contains("info")).ToList()[0].ChildNodes[11].InnerText;
            writer = writer.Replace("\n", "");
            writer = writer.Trim();

            string film_team = director + ", " + producer + ", " + writer;
            film_team = Regex.Replace(film_team, @"\s+", " ");

            //string Duration 
            string duration = "";
            try
            {
                duration = htmlDoc.DocumentNode.SelectNodes("//time").ToList()[2].InnerText;
            }
            catch (Exception)
            {
                duration = htmlDoc.DocumentNode.SelectNodes("//time").ToList()[1].InnerText;
            }
            duration = duration.Replace("\n", "");
            duration = Regex.Replace(duration, @"\s+", " ");

            //string Principal_Actors 
            var actors_parsed = htmlDoc.DocumentNode.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "").Contains("cast-and-crew-item ")).Take(6).ToList();
            string actors = "";
            for (int i=0; i<6;i++)
            {
                actors += actors_parsed[0].SelectNodes("//a[@data-qa='cast-crew-item-link']").ToList()[i].InnerText + ",";
                actors = actors.Replace("\n", "");
                actors.Trim();
                actors = Regex.Replace(actors, @"\s+", " ");
            }

            //string Critic_Comments 
            string critic_comments = "";
            try
            {
                critic_comments = Get_Critic_Comments(url + "/reviews?type=top_critics");
            }
            catch
            {
                critic_comments = "";
            }


            //string Audience_Comments 
            string audience_comments = "";
            try
            {
                audience_comments = Get_Audience_Comments(url + "/reviews?type=user");
            }
            catch
            {
                audience_comments = "";
            }

            //Title
            string title = url.Substring(url.LastIndexOf("/") + 1);
            title = title.ToUpper();
            title = title.Replace("_"," ");

            string release_date = htmlDoc.DocumentNode.Descendants("li").
                Where(node => node.GetAttributeValue("data-qa", "").Contains("movie-info-item")).ToList()[6]
                .Descendants("span").
                Where(node => node.GetAttributeValue("data-qa", "").Contains("movie-info-item-value")).ToList()[0].InnerText;
            release_date = release_date.Replace("\n", "");
            release_date.Trim();
            release_date = Regex.Replace(release_date, @"\s+", " ");
            release_date = release_date.Replace("&nbsp;wide", "").Trim();

            List<object> Movie = new()
            {
                url,
                title,
                img,
                critic_rating,
                audience_rating,
                critic_comments,
                audience_comments,
                available_platforms,
                synopsis,
                rating,
                genre,
                film_team,
                duration,
                actors,
                release_date,
            };
            return Movie;
        }

        public static string Get_Critic_Comments(string url)
        {
            string response = WebScraper.Call_url(url).Result;
            string critic_comments = WebScraper.Parse_Critic_Comments(response);
            return critic_comments;
        }
        public static string Parse_Critic_Comments(string html)
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);
            var comments_parsed= htmlDoc.DocumentNode.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "").Contains("review-row")).
               Take(6).ToList();
            string comments = "";
            for (int i = 0; i < 6; i++)
            {
                comments += comments_parsed[0].SelectNodes("//p[@class='review-text']").ToList()[i].InnerText+";.";
            }
            return comments;
        }

        public static string Get_Audience_Comments(string url)
        {
            string response = WebScraper.Call_url(url).Result;
            string audience_comments = WebScraper.Parse_Audience_Comments(response);
            return audience_comments;
        }
        public static string Parse_Audience_Comments(string html)
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);
            var comments_parsed = htmlDoc.DocumentNode.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "").Contains("audience-review-row")).
               Take(6).ToList();
            string comments = "";
            for (int i = 0; i < 6; i++)
            {
                comments += comments_parsed[0].SelectNodes("//p[@class='audience-reviews__review js-review-text']").ToList()[i].InnerText + ";.";
                comments.Trim();
                comments = Regex.Replace(comments, @"\s+", " ");
            }
            return comments;
        }


        public static List<object> Parse_html_Serie(string html)
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);

            // Image 
            string img = htmlDoc.DocumentNode.Descendants("img")
               .Where(node => node.GetAttributeValue("data-qa", "").Contains("poster-image")).
               ToList()[0].GetAttributeValue("src", "");

            //Critic_Rating 
            string critic_rating_parsed = htmlDoc.DocumentNode.Descendants("score-board").ToList()[0].GetAttributeValue("tomatometerscore", "");
            int critic_rating = Convert.ToInt32(critic_rating_parsed);

            //Audience_Rating 
            string audience_rating_parsed = htmlDoc.DocumentNode.Descendants("score-board").
               ToList()[0].GetAttributeValue("audiencescore", "");
            int audience_rating = Convert.ToInt32(audience_rating_parsed);

            //string Available_Platforms 
            string available_platforms = "";
            var platforms_parsed = htmlDoc.DocumentNode.Descendants("bubbles-overflow-container")
               .Where(node => node.GetAttributeValue("data-curation", "").Contains("rt-affiliates-sort-order")).
               ToList();
            for (int i = 2; i < platforms_parsed[0].ChildNodes.Count(); i++)
            {
                available_platforms += platforms_parsed[0].ChildNodes[i].GetAttributeValue("affiliate", "")+" ";
            }
            if (platforms_parsed[0].ChildNodes.Count() == 3)
            {
                available_platforms += platforms_parsed[0].ChildNodes[1].GetAttributeValue("affiliate", "");
            }

            //string Synopsis 
            string synopsis = htmlDoc.DocumentNode.Descendants("p")
               .Where(node => node.GetAttributeValue("data-qa", "").Contains("series-info-description")).ToList()[0].InnerText;
            synopsis = synopsis.Replace("\n", "");
            synopsis = synopsis.Trim();


            //string Genre 
            string genre = htmlDoc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").
            Contains("content")).ToList()[0].Descendants("li").ToList()[3].InnerText;
            bool hasIntegers = Regex.IsMatch(genre, @"\d+");
            if (hasIntegers)
            {
                genre = htmlDoc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").
                Contains("content")).ToList()[0].Descendants("li").ToList()[4].InnerText;
            }
            genre = genre.Replace("\n", "");
            genre = genre.Replace(" Genre:", "");
            genre = genre.Trim();
            genre = Regex.Replace(genre, @"\s+", " ");



            //string Serie_Team 
            string serie_team = "  ";
            try
            {
                var serie_team_parsed = htmlDoc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").
                Contains("content")).ToList()[0].Descendants("li").ToList();
                int stop = serie_team_parsed[0].SelectNodes("//a[@data-qa='series-details-producer']").ToList().Count();
                serie_team = "";
                for (int i=0; i<stop; i++)
                {
                    serie_team += serie_team_parsed[0].SelectNodes("//a[@data-qa='series-details-producer']").ToList()[i].InnerText;
                    serie_team.Replace("\n", "");
                    serie_team.Trim();
                }
                serie_team = Regex.Replace(serie_team, @"\s+", " ");
            }
            catch { }


            //string Principal_Actors 
            var actors_parsed = htmlDoc.DocumentNode.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "").Contains("cast-and-crew-item ")).Take(6).ToList();
            string actors = "";
            for (int i = 0; i < 6; i++)
            {
                actors += actors_parsed[0].SelectNodes("//p[@class='name']").ToList()[i].InnerText + ",";
                actors = actors.Replace("\n", "");
                actors.Trim();
                actors = Regex.Replace(actors, @"\s+", " ");
            }

            string premiere_date = htmlDoc.DocumentNode.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "").Contains("content")).ToList()[0]
               .Descendants("span")
               .Where(node => node.GetAttributeValue("data-qa", "").Contains("series-details-premiere-date")).ToList()[0].InnerText;

            List<object> serie = new()
            {
                img,
                critic_rating,
                audience_rating,
                available_platforms,
                synopsis,
                genre,
                serie_team,
                actors,
                premiere_date
            };
            return serie;
        }

        public static Movie Get_Movie_data(string url)
        {
            string response = WebScraper.Call_url(url).Result;
            List<object> movie_data = WebScraper.Parse_html_Movie(response, url);
            Movie movie = new()
            {
                url = (string)movie_data[0],
                Title = (string)movie_data[1],
                Image = (string)movie_data[2],
                Critic_Rating = (int)movie_data[3],
                Audience_Rating = (int)movie_data[4],
                Critic_Comments = (string)movie_data[5],
                Audience_Comments = (string)movie_data[6],
                Available_Platforms = (string)movie_data[7],
                Synopsis = (string)movie_data[8],
                Rating = (string)movie_data[9],
                Genre = (string)movie_data[10],
                Film_Team = (string)movie_data[11],
                Duration = (string)movie_data[12],
                Principal_Actors = (string)movie_data[13],
                Release_Date = (string)movie_data[14],
                Top = false
            };
            return movie;

        }

        public static async Task<Movie> Get_Movie_dataAsync(string url, int id = -1, Proyect_Rotten_TomatoesContext _context=null)
        {
            string response = WebScraper.Call_url(url).Result;
            List<object> movie_data = WebScraper.Parse_html_Movie(response, url);
            Movie movie = new()
            {
                url = (string)movie_data[0],
                Title = (string)movie_data[1],
                Image = (string)movie_data[2],
                Critic_Rating = (int)movie_data[3],
                Audience_Rating = (int)movie_data[4],
                Critic_Comments = (string)movie_data[5],
                Audience_Comments = (string)movie_data[6],
                Available_Platforms = (string)movie_data[7],
                Synopsis = (string)movie_data[8],
                Rating = (string)movie_data[9],
                Genre = (string)movie_data[10],
                Film_Team = (string)movie_data[11],
                Duration = (string)movie_data[12],
                Principal_Actors = (string)movie_data[13],
                Release_Date = (string)movie_data[14],
                Top = false
            };
            if (id != -1){
                var movieToDelete = await _context.Movie.FindAsync(id);
                
                if (movieToDelete != null)
                {
                    _context.Movie.Remove(movieToDelete);
                    movie.Id = id;
                }
                else
                {
                    return null;
                }
            }

            return movie;
        }

        public static Serie Get_Serie_data(string url)
        {
            string response = WebScraper.Call_url(url).Result;
            List<object> serie_data = WebScraper.Parse_html_Serie(response);
            //Title
            string title = url.Substring(url.LastIndexOf("/") + 1);
            title = title.ToUpper();
            title = title.Replace("_", " ");
            Serie serie = new()
            {
                url = url,
                Title = title,
                Image = (string)serie_data[0],
                Critic_Rating = (int)serie_data[1],
                Audience_Rating = (int)serie_data[2],
                Available_Platforms= (string)serie_data[3],
                Synopsis = (string)serie_data[4],
                Genre = (string)serie_data[5],
                Serie_Team = (string)serie_data[6],
                Principal_Actors = (string)serie_data[7],
                Premiere_Date = (string)serie_data[8],
                Top = false
            };
            return serie;
        }

        public static async Task<Serie> Get_Serie_dataAsync(string url, int id=-1, Proyect_Rotten_TomatoesContext _context=null)
        {
            string response = WebScraper.Call_url(url).Result;
            List<object> serie_data = WebScraper.Parse_html_Serie(response);
            //Title
            string title = url.Substring(url.LastIndexOf("/") + 1);
            title = title.ToUpper();
            title = title.Replace("_", " ");
            Serie serie = new()
            {
                url = url,
                Title = title,
                Image = (string)serie_data[0],
                Critic_Rating = (int)serie_data[1],
                Audience_Rating = (int)serie_data[2],
                Available_Platforms = (string)serie_data[3],
                Synopsis = (string)serie_data[4],
                Genre = (string)serie_data[5],
                Serie_Team = (string)serie_data[6],
                Principal_Actors = (string)serie_data[7],
                Premiere_Date = (string)serie_data[8],
                Top = false
            };
            if (id != -1)
            {
                var serieToDelete = await _context.Serie.FindAsync(id);

                if (serieToDelete != null)
                {
                    _context.Serie.Remove(serieToDelete);
                    serie.Id = id;
                }
                else
                {
                    return null;
                }
            }

            return serie;
        }



        public static async Task Top10Series(Proyect_Rotten_TomatoesContext _context)
        {
            var allRecords = _context.Serie.ToList();
            foreach (var record in allRecords)
            {
                record.Top = false;
            }
            _context.SaveChanges();
            string response = WebScraper.Call_url("https://www.rottentomatoes.com/browse/tv_series_browse/sort:popular").Result;
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(response);

            //TopSeries
            var topSeries = htmlDoc.DocumentNode.Descendants("div")
               .Where(node => node.GetAttributeValue("class", "").Contains("js-tile-link ")).
               ToList().Take(10);

            foreach ( var ser in topSeries ) 
            {
                string rotten = "https://www.rottentomatoes.com";
                string NameSer = ser.Descendants("a")
                    .Where(node => node.GetAttributeValue("data-qa", "").Contains("discovery-media-list-item-caption")).ToList()[0].GetAttributeValue("href", "");
                string SerieLink = rotten + NameSer;

                //Title
                string title = NameSer.Substring(NameSer.LastIndexOf("/") + 1);
                title = title.ToUpper();
                title = title.Replace("_", " ");
                var serieToDelete = await _context.Serie.FirstOrDefaultAsync(s => s.Title == title);
                if (serieToDelete != null)
                {
                    serieToDelete.Top = true;
                }
                else
                {
                    Serie serie = WebScraper.Get_Serie_data(SerieLink);
                    serie.Top = true;
                    _context.Add(serie);
                }
                await _context.SaveChangesAsync();

            }

        }


        public static async Task Top10Movies(Proyect_Rotten_TomatoesContext _context)
        {
            var allRecords = _context.Movie.ToList();
            foreach (var record in allRecords)
            {
                record.Top = false;
            }
            _context.SaveChanges();
            string response = WebScraper.Call_url("https://www.rottentomatoes.com/").Result;
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(response);

            //TopMovies
            var topMovies = htmlDoc.DocumentNode.Descendants("tiles-carousel-responsive-item")
               .Where(node => node.GetAttributeValue("slot", "").Contains("tile")).
               ToList().Take(10);

            foreach (var mov in topMovies)
            {
                string rotten = "https://www.rottentomatoes.com";
                string NameMov = mov.Descendants("a")
                    .ToList()[0].GetAttributeValue("href", "");
                string MovieLink = rotten + NameMov;

                //Title
                string title = NameMov.Substring(NameMov.LastIndexOf("/") + 1);
                title = title.ToUpper();
                title = title.Replace("_", " ");
                var movieExists= await _context.Movie.FirstOrDefaultAsync(m => m.Title == title);
                if (movieExists != null)
                {
                    movieExists.Top = true;
                }
                else
                {
                    Movie movie = WebScraper.Get_Movie_data(MovieLink);
                    movie.Top = true;
                    _context.Add(movie);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
