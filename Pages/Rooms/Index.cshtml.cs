using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystemFresh.Data;
using HotelManagementSystemFresh.Models;

namespace HotelManagementSystemFresh.Pages_Rooms
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Room> Room { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RoomType { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Availability { get; set; }

        public List<string> RoomTypes { get; set; } = new();

        public async Task OnGetAsync()
        {
            var query = _context.Rooms.AsQueryable();

            // Search by room number
            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(r =>
                    r.RoomNumber.Contains(Search));
            }

            // Filter by room type
            if (!string.IsNullOrWhiteSpace(RoomType))
            {
                query = query.Where(r =>
                    r.RoomType == RoomType);
            }

            // Filter by availability
            if (Availability == "Available")
            {
                query = query.Where(r => r.IsAvailable);
            }
            else if (Availability == "Occupied")
            {
                query = query.Where(r => !r.IsAvailable);
            }

            Room = await query
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            RoomTypes = await _context.Rooms
                .Select(r => r.RoomType)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }
    }
}