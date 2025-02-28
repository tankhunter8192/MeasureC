using Gpib.Web.Data;
using Gpib.Web.Data.DBClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Gpib.Web.Pages
{
    public class EditProgramFileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditProgramFileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProgramFile ProgramFile { get; set; }

        public IActionResult OnGet(int id)
        {
            ProgramFile = _context.ProgramFiles.Find(id);
            if (ProgramFile == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var programFileInDb = _context.ProgramFiles.Find(ProgramFile.Id);
            if (programFileInDb == null)
            {
                return NotFound();
            }

            programFileInDb.Name = ProgramFile.Name;
            programFileInDb.Description = ProgramFile.Description;
            programFileInDb.Content = ProgramFile.Content;

            _context.SaveChanges();

            return RedirectToPage("Index");
        }
    }
}