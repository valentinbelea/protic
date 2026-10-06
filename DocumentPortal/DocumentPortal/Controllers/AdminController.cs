using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentPortal.Data;
using DocumentPortal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DocumentPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        // --- MENUS ---
        public async Task<IActionResult> Menus()
        {
            var menus = await _context.Menus.OrderBy(m => m.Order).ThenBy(m => m.Id).ToListAsync();
            return View(menus);
        }

        [HttpPost]
        public async Task<IActionResult> AddMenu(Menu menu)
        {
            if (!ModelState.IsValid)
            {
                // Re-render the list with the validation errors and the values the user entered
                var menus = await _context.Menus.OrderBy(m => m.Order).ThenBy(m => m.Id).ToListAsync();
                return View(nameof(Menus), menus);
            }

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Menus));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Menus));
        }

        [HttpGet]
        public async Task<IActionResult> EditMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();
            return View(menu);
        }

        [HttpPost]
        public async Task<IActionResult> EditMenu(int id, Menu updatedMenu)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(updatedMenu);
            }

            menu.Name = updatedMenu.Name;
            menu.Order = updatedMenu.Order;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Menus));
        }

        // --- SECTIONS ---
        public async Task<IActionResult> Sections()
        {
            var sections = await _context.Sections.Include(s => s.Menu).ToListAsync();
            ViewBag.Menus = new SelectList(_context.Menus, "Id", "Name");
            return View(sections);
        }

        [HttpPost]
        public async Task<IActionResult> AddSection(Section section)
        {
            if (ModelState.IsValid)
            {
                _context.Sections.Add(section);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Sections));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSection(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section != null)
            {
                _context.Sections.Remove(section);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Sections));
        }

        [HttpGet]
        public async Task<IActionResult> EditSection(int id)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null) return NotFound();

            ViewBag.Menus = new SelectList(_context.Menus, "Id", "Name", section.MenuId);
            return View(section);
        }

        [HttpPost]
        public async Task<IActionResult> EditSection(int id, Section updatedSection)
        {
            var section = await _context.Sections.FindAsync(id);
            if (section == null) return NotFound();

            section.MenuId = updatedSection.MenuId;
            section.Title = updatedSection.Title;
            section.IsPrivate = updatedSection.IsPrivate;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Sections));
        }

        // --- QUIZZES ---
        public async Task<IActionResult> Quizzes(int? filterSectionId)
        {
            var quizzesQuery = _context.Quizzes.Include(q => q.Section).ThenInclude(s => s.Menu).OrderBy(q => q.SectionId).AsQueryable();

            if (filterSectionId.HasValue)
            {
                quizzesQuery = quizzesQuery.Where(q => q.SectionId == filterSectionId.Value);
            }

            var quizzes = await quizzesQuery.ToListAsync();

            var sectionsList = _context.Sections.Include(s => s.Menu).Select(s => new {
                Id = s.Id,
                Title = s.Menu!.Name + " - " + s.Title
            }).ToList();

            ViewBag.Sections = new SelectList(sectionsList, "Id", "Title", filterSectionId);
            ViewBag.FilterSectionId = filterSectionId;

            return View(quizzes);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuiz(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Quizzes), new { filterSectionId = quiz.SectionId });
        }

        [HttpGet]
        public async Task<IActionResult> EditQuiz(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            var sectionsList = _context.Sections.Include(s => s.Menu).Select(s => new {
                Id = s.Id,
                Title = s.Menu!.Name + " - " + s.Title
            }).ToList();
            ViewBag.Sections = new SelectList(sectionsList, "Id", "Title", quiz.SectionId);

            return View(quiz);
        }

        [HttpPost]
        public async Task<IActionResult> EditQuiz(int id, Quiz updatedQuiz)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            quiz.SectionId = updatedQuiz.SectionId;
            quiz.Title = updatedQuiz.Title;
            quiz.Description = updatedQuiz.Description;
            quiz.IsActive = updatedQuiz.IsActive;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Quizzes), new { filterSectionId = quiz.SectionId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            int? sectionId = null;
            if (quiz != null)
            {
                sectionId = quiz.SectionId;
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Quizzes), new { filterSectionId = sectionId });
        }

        [HttpGet]
        public async Task<IActionResult> BuildQuiz(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (quiz == null) return NotFound();
            return View(quiz);
        }

        [HttpPost]
        public async Task<IActionResult> SaveQuiz(int id, [FromBody] Quiz payload)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == id);
                
            if (quiz == null) return NotFound();

            _context.Questions.RemoveRange(quiz.Questions);
            
            if (payload.Questions != null)
            {
                foreach(var q in payload.Questions)
                {
                    var newQ = new Question { Text = q.Text, IsMultipleChoice = q.IsMultipleChoice, QuizId = id };
                    foreach(var a in q.AnswerOptions)
                    {
                        newQ.AnswerOptions.Add(new AnswerOption { Text = a.Text, IsCorrect = a.IsCorrect });
                    }
                    quiz.Questions.Add(newQ);
                }
            }
            
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> QuizResults(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Section)
                .ThenInclude(s => s.Menu)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null) return NotFound();

            var submissions = await _context.QuizSubmissions
                .Include(s => s.User)
                .Where(s => s.QuizId == id)
                .OrderByDescending(s => s.SubmissionDate)
                .ToListAsync();

            ViewBag.Quiz = quiz;
            return View(submissions);
        }

        [HttpPost]
        public async Task<IActionResult> ResetQuizSubmission(int id)
        {
            var submission = await _context.QuizSubmissions.FindAsync(id);
            int quizId = 0;
            if (submission != null)
            {
                quizId = submission.QuizId;
                _context.QuizSubmissions.Remove(submission);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(QuizResults), new { id = quizId });
        }

        // --- DOCUMENTS ---
        public async Task<IActionResult> Documents(int? filterSectionId)
        {
            var documentsQuery = _context.Documents.Include(d => d.Section).ThenInclude(s => s.Menu).OrderBy(d => d.SectionId).ThenBy(d => d.Order).AsQueryable();

            if (filterSectionId.HasValue)
            {
                documentsQuery = documentsQuery.Where(d => d.SectionId == filterSectionId.Value);
            }

            var documents = await documentsQuery.ToListAsync();

            var sectionsList = _context.Sections.Include(s => s.Menu).Select(s => new {
                Id = s.Id,
                Title = s.Menu.Name + " - " + s.Title
            }).ToList();

            ViewBag.Sections = new SelectList(sectionsList, "Id", "Title", filterSectionId);
            ViewBag.FilterSectionId = filterSectionId;

            return View(documents);
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(int sectionId, string description, int order, IFormFile? file, string? content)
        {
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var document = new Document
                {
                    SectionId = sectionId,
                    FileName = file.FileName,
                    Description = description,
                    Order = order,
                    FilePath = uniqueFileName,
                    ContentType = file.ContentType
                };

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();
            }
            else if (!string.IsNullOrWhiteSpace(content))
            {
                var document = new Document
                {
                    SectionId = sectionId,
                    Description = description,
                    Order = order,
                    Content = content
                };

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Documents), new { filterSectionId = sectionId });
        }

        [HttpGet]
        public async Task<IActionResult> EditDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            var sectionsList = _context.Sections.Include(s => s.Menu).Select(s => new {
                Id = s.Id,
                Title = s.Menu.Name + " - " + s.Title
            }).ToList();
            ViewBag.Sections = new SelectList(sectionsList, "Id", "Title", document.SectionId);

            return View(document);
        }

        [HttpPost]
        public async Task<IActionResult> EditDocument(int id, int sectionId, string description, int order, string? content)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            document.SectionId = sectionId;
            document.Description = description;
            document.Order = order;

            if (document.FilePath == null)
            {
                document.Content = content;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Documents), new { filterSectionId = sectionId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            int? sectionId = null;
            if (document != null)
            {
                sectionId = document.SectionId;
                if (!string.IsNullOrEmpty(document.FilePath))
                {
                    var filePath = Path.Combine(_env.WebRootPath, "uploads", document.FilePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Documents), new { filterSectionId = sectionId });
        }

        // --- USERS ---
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(user.Password)) user.Password = "password";
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Username = updatedUser.Username;
            user.Password = updatedUser.Password;
            user.Role = updatedUser.Role;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null && user.Username != "admin")
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Users));
        }
    }
}
