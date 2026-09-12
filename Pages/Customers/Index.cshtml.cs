using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HotelManagementSystemFresh.Data;
using HotelManagementSystemFresh.Models;

namespace HotelManagementSystemFresh.Pages_Customers
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Customer> Customer { get; set; } = default!;

        public string? Search { get; set; }

        public async Task OnGetAsync(string? search)
        {
            Search = search;

            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.FullName.Contains(search) ||
                    c.Email.Contains(search) ||
                    c.Phone.Contains(search));
            }

            Customer = await query
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }
    }
}