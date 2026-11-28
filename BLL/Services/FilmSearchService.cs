using BLL.Interfaces;
using DAL.Context;
using Domain.DTOs;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class FilmSearchService : IFilmSearchService
    {
        private readonly CinemaContext _context;

        public FilmSearchService(CinemaContext context)
        {
            _context = context;
        }

        public async Task<FilmSearchResultDto> SearchAsync(
            FilmSearchDto filters, CancellationToken cancellationToken = default)
        {
            IQueryable<Film> query = _context.Films.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filters.Title))
            {
                string title = filters.Title.Trim();
                query = query.Where(f => f.Title == title);
            }

            if (!string.IsNullOrWhiteSpace(filters.Description))
            {
                string description = filters.Description.Trim();
                query = query.Where(f => f.Description.Contains(description));
            }

            if (filters.Year.HasValue)
            {
                int year = filters.Year.Value;
                query = query.Where(f => f.Year == year);
            }

            int totalCount = await query.CountAsync(cancellationToken);

            List<Film> items = await query
                .OrderByDescending(f => f.Year)
                .ThenBy(f => f.Title)
                .ThenBy(f => f.Id)
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .Include(f => f.Trailers)
                .Include(f => f.Participations)
                    .ThenInclude(p => p.Participant)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            return new FilmSearchResultDto
            {
                Items = items,
                TotalCount = totalCount,
                Page = filters.Page,
                PageSize = filters.PageSize
            };
        }
    }
}
