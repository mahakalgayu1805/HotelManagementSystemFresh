using HotelManagementSystemFresh.Data;
using HotelManagementSystemFresh.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystemFresh.Pages_Bookings
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booking Booking { get; set; } = new Booking();

        public SelectList? Rooms { get; set; }
        public SelectList? Customers { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            Booking = booking;

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
                    "Check-out date must be after check-in date."
                );

                await LoadDropdownsAsync();
                return Page();
            }

            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == Booking.Id);

            if (existingBooking == null)
            {
                return NotFound();
            }

            // Get old room
            var oldRoom = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == existingBooking.RoomId);

            // Get selected new room
            var newRoom = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == Booking.RoomId);

            if (newRoom == null)
            {
                ModelState.AddModelError(
                    "Booking.RoomId",
                    "Selected room does not exist."
                );

                await LoadDropdownsAsync();
                return Page();
            }

            bool roomAlreadyBooked = await _context.Bookings.AnyAsync(b =>
                b.Id != Booking.Id &&
                b.RoomId == Booking.RoomId &&
                b.Status != "Cancelled" &&
                Booking.CheckInDate < b.CheckOutDate &&
                Booking.CheckOutDate > b.CheckInDate
            );

            if (roomAlreadyBooked)
            {
                ModelState.AddModelError(
                    "Booking.RoomId",
                    "This room is already booked for the selected dates."
                );

                await LoadDropdownsAsync();
                return Page();
            }

            // If room was changed, make old room available
            if (oldRoom != null && oldRoom.Id != newRoom.Id)
            {
                oldRoom.IsAvailable = true;
            }

            // Update booking details
            existingBooking.RoomId = Booking.RoomId;
            existingBooking.CustomerId = Booking.CustomerId;
            existingBooking.CheckInDate = Booking.CheckInDate;
            existingBooking.CheckOutDate = Booking.CheckOutDate;
            existingBooking.Status = Booking.Status;

            // Calculate nights
            existingBooking.NumberOfNights =
                (Booking.CheckOutDate.Date - Booking.CheckInDate.Date).Days;

            // Calculate total amount
            existingBooking.TotalAmount =
                existingBooking.NumberOfNights * newRoom.PricePerNight;

            // Update room availability based on booking status
            if (Booking.Status == "Booked" ||
                Booking.Status == "Checked-In")
            {
                newRoom.IsAvailable = false;
            }
            else if (Booking.Status == "Checked-Out" ||
                     Booking.Status == "Cancelled")
            {
                newRoom.IsAvailable = true;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private async Task LoadDropdownsAsync()
        {
            var rooms = await _context.Rooms
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            var customers = await _context.Customers
                .OrderBy(c => c.FullName)
                .ToListAsync();

            Rooms = new SelectList(
                rooms,
                "Id",
                "RoomNumber",
                Booking.RoomId
            );

            Customers = new SelectList(
                customers,
                "Id",
                "FullName",
                Booking.CustomerId
            );
        }
    }
}