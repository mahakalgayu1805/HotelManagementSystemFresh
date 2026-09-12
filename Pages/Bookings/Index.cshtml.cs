using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystemFresh.Data;
using HotelManagementSystemFresh.Models;

namespace HotelManagementSystemFresh.Pages_Bookings
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get; set; } = default!;

        public string? Search { get; set; }

        public string? Status { get; set; }

        public async Task OnGetAsync(string? search, string? status)
        {
            Search = search;
            Status = status;

            var query = _context.Bookings
                .Include(b => b.Room)
                .Include(b => b.Customer)
                .AsQueryable();

            // Search by Room Number or Customer Name
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(b =>
                    b.Room != null &&
                    b.Customer != null &&
                    (b.Room.RoomNumber.Contains(search) ||
                     b.Customer.FullName.Contains(search)));
            }

            // Filter by booking status
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(b => b.Status == status);
            }

            Booking = await query
                .OrderByDescending(b => b.CheckInDate)
                .ToListAsync();
        }
    }
}