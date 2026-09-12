using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using HotelManagementSystemFresh.Data;
using HotelManagementSystemFresh.Models;

namespace HotelManagementSystemFresh.Pages_Bookings
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        public SelectList? Customers { get; set; }
        public SelectList? Rooms { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDropdownsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return Page();
            }

            if (Booking.CheckOutDate <= Booking.CheckInDate)
            {
                ModelState.AddModelError(
                    "Booking.CheckOutDate",
                    "Check-out date must be after check-in date.");

                await LoadDropdownsAsync();
                return Page();
            }

            bool roomAlreadyBooked = await _context.Bookings.AnyAsync(b =>
                b.RoomId == Booking.RoomId &&
                b.Status != "Cancelled" &&
                Booking.CheckInDate < b.CheckOutDate &&
                Booking.CheckOutDate > b.CheckInDate);

            if (roomAlreadyBooked)
            {
                ModelState.AddModelError(
                    "Booking.RoomId",
                    "This room is already booked for the selected dates.");

                await LoadDropdownsAsync();
                return Page();
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == Booking.RoomId);

            if (room == null)
            {
                ModelState.AddModelError(
                    "Booking.RoomId",
                    "Selected room does not exist.");

                await LoadDropdownsAsync();
                return Page();
            }

            // Calculate number of nights
            Booking.NumberOfNights =
                (Booking.CheckOutDate.Date - Booking.CheckInDate.Date).Days;

            // Calculate total bill
            Booking.TotalAmount =
                Booking.NumberOfNights * room.PricePerNight;

            Booking.Status = "Booked";

            // Mark room as occupied
            room.IsAvailable = false;

            _context.Bookings.Add(Booking);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task LoadDropdownsAsync()
        {
            Customers = new SelectList(
                await _context.Customers
                    .OrderBy(c => c.FullName)
                    .ToListAsync(),
                "Id",
                "FullName");

            Rooms = new SelectList(
                await _context.Rooms
                    .OrderBy(r => r.RoomNumber)
                    .ToListAsync(),
                "Id",
                "RoomNumber");
        }
    }
}