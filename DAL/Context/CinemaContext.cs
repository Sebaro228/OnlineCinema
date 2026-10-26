using Azure.Core;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Context
{
        public class CinemaContext(DbContextOptions<CinemaContext> opts) : DbContext(opts)
        {
            public DbSet<Film> Films { get; set; }
            public DbSet<Trailer> Trailers { get; set; }
            public DbSet<User> Users { get; set; }
            public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Film>()
                    .HasMany(f => f.Trailers)
                    .WithOne(t => t.Film)
                    .HasForeignKey(t => t.FilmId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }
}
