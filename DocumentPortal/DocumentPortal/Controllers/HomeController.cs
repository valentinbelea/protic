using DocumentPortal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using DocumentPortal.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace DocumentPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Land on the first menu that actually has content; fall back to the first menu at all
            var firstMenu = await _context.Menus
                                .Where(m => m.Sections.Any())
                                .OrderBy(m => m.Order).ThenBy(m => m.Id)
                                .FirstOrDefaultAsync()
                            ?? await _context.Menus.OrderBy(m => m.Order).ThenBy(m => m.Id).FirstOrDefaultAsync();
            if (firstMenu != null)
            {
                return RedirectToAction("Menu", new { id = firstMenu.Id });
            }
            return View();
        }

        public async Task<IActionResult> Menu(int id)
        {
            var menu = await _context.Menus
                .Include(m => m.Sections.OrderBy(s => s.Id))
                    .ThenInclude(s => s.Documents.OrderBy(d => d.Order))
                .Include(m => m.Sections.OrderBy(s => s.Id))
                    .ThenInclude(s => s.Quizzes.Where(q => q.IsActive))
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        public async Task<IActionResult> ViewDocument(int id)
        {
            var document = await _context.Documents
                .Include(d => d.Section)
                .ThenInclude(s => s.Menu)
                .FirstOrDefaultAsync(d => d.Id == id);
                
            if (document == null || document.Content == null) return NotFound();

            bool isAuthenticated = User.Identity != null && User.Identity.IsAuthenticated;
            if (document.Section.IsPrivate && !isAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Home/ViewDocument/{id}" });
            }

            return View(document);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> TakeQuiz(int id)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Home/TakeQuiz/{id}" });
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == id && q.IsActive);
                
            if (quiz == null) return NotFound();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null) return NotFound();

            var previousSubmission = await _context.QuizSubmissions
                .Include(s => s.Answers)
                .FirstOrDefaultAsync(s => s.QuizId == id && s.UserId == user.Id);

            var vm = new DocumentPortal.ViewModels.TakeQuizViewModel
            {
                Quiz = quiz,
                Answers = new Dictionary<int, List<int>>(),
                PreviousSubmission = previousSubmission
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(int id, [FromForm] IFormCollection form)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = $"/Home/TakeQuiz/{id}" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null) return NotFound();

            var existingSubmission = await _context.QuizSubmissions.AnyAsync(s => s.QuizId == id && s.UserId == user.Id);
            if (existingSubmission)
            {
                return RedirectToAction("TakeQuiz", new { id = id });
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.AnswerOptions)
                .FirstOrDefaultAsync(q => q.Id == id && q.IsActive);

            if (quiz == null) return NotFound();

            int correctAnswers = 0;
            int totalQuestions = quiz.Questions.Count;

            foreach (var question in quiz.Questions)
            {
                var userSelectedOptionIds = form[$"Answers[{question.Id}]"].ToArray()
                    .Where(val => !string.IsNullOrEmpty(val))
                    .Select(val => int.Parse(val))
                    .ToList();

                var correctOptionIds = question.AnswerOptions.Where(a => a.IsCorrect).Select(a => a.Id).ToList();

                // If user selected EXACTLY the correct options (no more, no less)
                if (userSelectedOptionIds.Count == correctOptionIds.Count && !userSelectedOptionIds.Except(correctOptionIds).Any())
                {
                    correctAnswers++;
                }
            }

            var submission = new QuizSubmission
            {
                QuizId = id,
                UserId = user.Id,
                SubmissionDate = System.DateTime.UtcNow,
                Score = correctAnswers,
                TotalQuestions = totalQuestions
            };

            foreach (var question in quiz.Questions)
            {
                var userSelectedOptionIds = form[$"Answers[{question.Id}]"].ToArray()
                    .Where(val => !string.IsNullOrEmpty(val))
                    .Select(val => int.Parse(val))
                    .ToList();
                
                foreach(var optionId in userSelectedOptionIds)
                {
                    submission.Answers.Add(new QuizSubmissionAnswer
                    {
                        QuestionId = question.Id,
                        AnswerOptionId = optionId
                    });
                }
            }

            _context.QuizSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            // Redirect to TakeQuiz (GET) so the user sees the readonly review with highlighted answers
            return RedirectToAction(nameof(TakeQuiz), new { id = id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                Microsoft.AspNetCore.Localization.CookieRequestCultureProvider.DefaultCookieName,
                Microsoft.AspNetCore.Localization.CookieRequestCultureProvider.MakeCookieValue(new Microsoft.AspNetCore.Localization.RequestCulture(culture)),
                new CookieOptions { Expires = System.DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}
