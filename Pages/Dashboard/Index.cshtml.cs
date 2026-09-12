using HotelManagementSystemFresh.Data;
using HotelManagementSystemFresh.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystemFresh.Pages.Dashboard;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int TotalRooms { get; set; }
    public int AvailableRooms { get; set; }
    public int OccupiedRooms { get; set; }

    public int TotalCustomers { get; set; }

    public int TotalBookings { get; set; }
    public int ActiveBookings { get; set; }

    public int BookedBookings { get; set; }
    public int CheckedInBookings { get; set; }
    public int CheckedOutBookings { get; set; }
    public int CancelledBookings { get; set; }

    public decimal TotalRevenue { get; set; }

    public double OccupancyRate { get; set; }

    public List<Booking> RecentBookings { get; set; } = new();

    public async Task OnGetAsync()
    {
        TotalRooms = await _context.Rooms.CountAsync();

        AvailableRooms = await _context.Rooms
            .CountAsync(r => r.IsAvailable);

        OccupiedRooms = await _context.Rooms
            .CountAsync(r => !r.IsAvailable);

        TotalCustomers = await _context.Customers.CountAsync();

        TotalBookings = await _context.Bookings.CountAsync();

        ActiveBookings = await _context.Bookings
            .CountAsync(b =>
                b.Status == "Booked" ||
                b.Status == "Checked-In");

        BookedBookings = await _context.Bookings
            .CountAsync(b => b.Status == "Booked");

        CheckedInBookings = await _context.Bookings
            .CountAsync(b => b.Status == "Checked-In");

        CheckedOutBookings = await _context.Bookings
            .CountAsync(b => b.Status == "Checked-Out");

        CancelledBookings = await _context.Bookings
            .CountAsync(b => b.Status == "Cancelled");

        TotalRevenue = await _context.Bookings
            .Where(b => b.Status != "Cancelled")
            .SumAsync(b => b.TotalAmount);

        if (TotalRooms > 0)
        {
            OccupancyRate =
                Math.Round(
                    (double)OccupiedRooms / TotalRooms * 100,
                    1
                );
        }

        RecentBookings = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.Customer)
            .OrderByDescending(b => b.Id)
            .Take(5)
            .ToListAsync();
    }
}