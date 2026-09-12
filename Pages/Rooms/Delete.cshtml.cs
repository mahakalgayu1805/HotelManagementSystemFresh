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
    public class DeleteModel : PageModel
    {
        private readonly HotelManagementSystemFresh.Data.ApplicationDbContext _context;

        public DeleteModel(HotelManagementSystemFresh.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Room Room { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);

            if (room is not null)
            {
                Room = room;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                Room = room;
                _context.Rooms.Remove(Room);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
