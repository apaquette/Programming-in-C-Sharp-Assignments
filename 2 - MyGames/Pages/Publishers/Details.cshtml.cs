using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyGames.Data;
using MyGames.Models;

namespace MyGames.Pages.Publishers
{
    public class DetailsModel : PageModel
    {
        private readonly MyGames.Data.MyGamesDbContext _context;

        public DetailsModel(MyGames.Data.MyGamesDbContext context)
        {
            _context = context;
        }

        public Publisher Publisher { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Publisher = await _context.Publishers.Include(g => g.Games).FirstOrDefaultAsync(m => m.PublisherId == id);

            if (Publisher == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
