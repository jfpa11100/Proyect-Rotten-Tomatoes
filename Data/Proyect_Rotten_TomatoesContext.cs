using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Proyect_Rotten_Tomatoes.Models;

namespace Proyect_Rotten_Tomatoes.Data
{
    public class Proyect_Rotten_TomatoesContext : DbContext
    {
        public Proyect_Rotten_TomatoesContext (DbContextOptions<Proyect_Rotten_TomatoesContext> options)
            : base(options)
        {
        }

        public DbSet<Proyect_Rotten_Tomatoes.Models.Cinephile> Cinephile { get; set; } = default!;

        public DbSet<Proyect_Rotten_Tomatoes.Models.Film_Expert> Film_Expert { get; set; }

        public DbSet<Proyect_Rotten_Tomatoes.Models.Movie>? Movie { get; set; }

        public DbSet<Proyect_Rotten_Tomatoes.Models.Serie>? Serie { get; set; }
    }
}
