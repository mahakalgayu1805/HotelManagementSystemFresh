namespace HotelManagementSystemFresh.Models;

public class Booking
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public int CustomerId { get; set; }

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public string Status { get; set; } = "Booked";

    // Navigation Properties
    public Room? Room { get; set; }

    public Customer? Customer { get; set; }

    // Billing
    public int NumberOfNights { get; set; }

    public decimal TotalAmount { get; set; }
}