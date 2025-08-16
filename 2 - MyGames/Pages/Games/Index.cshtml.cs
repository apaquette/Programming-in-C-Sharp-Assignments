using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyGames.Data;
using MyGames.Models;

namespace MyGames.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly MyGames.Data.MyGamesDbContext _context;

        public IndexModel(MyGames.Data.MyGamesDbContext context)
        {
            _context = context;
        }

        public IList<Game> Game { get;set; }

        public async Task OnGetAsync()
        {
            Game = await _context.Games
                .Include(g => g.Publisher).ToListAsync();
        }
    }
}
