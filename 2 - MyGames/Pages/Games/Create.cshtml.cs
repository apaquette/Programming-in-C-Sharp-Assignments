using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyGames.Data;
using MyGames.Models;

namespace MyGames.Pages.Games
{
    public class CreateModel : PageModel
    {
        private readonly MyGames.Data.MyGamesDbContext _context;

        public CreateModel(MyGames.Data.MyGamesDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            List<SelectListItem> PublisherSelectOptions = new();
            foreach(Publisher p in _context.Publishers){
                PublisherSelectOptions.Add(new SelectListItem($"{p.Name}", p.PublisherId.ToString()));
            }
            PublisherOptions = new SelectList(PublisherSelectOptions, "Value", "Text");
            return Page();
        }

        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if(Game is not null){
                _context.Games?.Add(Game);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
        [BindProperty]
        public Game Game { get; set; }
        public SelectList? PublisherOptions { get; set; }
    }
}
