using BlogProj_12_10_22.Data;
using BlogProj_12_10_22.Models;
using BlogProj_12_10_22.Services;
using BlogProj_12_10_22.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace BlogProj_12_10_22.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IImageService _imageService;

        public HomeController(ILogger<HomeController> logger,
                              IBlogEmailSender emailSender,
                              ApplicationDbContext context,
                              IConfiguration configuration,
                              IImageService imageService)
        {
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _configuration = configuration;
            _imageService = imageService;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        public async Task<IActionResult> Index(int? page)
        {
            
            var im = await _imageService.EncodeImageAsync("blogMainBdrop.jpg");
            var im1 = _imageService.DecodeImage(im, "jpg");
            ViewData["HeaderImage"] = im1;
            ViewData["MainText"] = "Laurence's IT Blog";
            ViewData["SubText"] = "Read posts from my Blogs below";

            var pageNumber = page ?? 1;
            var pageSize = 12;
            var OnePageOfBlogs = await _context.Blog.Include(b => b.BlogUser)
                                 .OrderByDescending(b => b.Created).ToPagedListAsync(pageNumber, pageSize);
            
            return View(OnePageOfBlogs);
        }
        
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMe? model)
        {
            model.Message = $"{model.Message} <hr/> Phone: {model.Phone}";
            await _emailSender.SendContactEmailAsync(model.Email, model.Name, model.Subject,model.Message);
            return RedirectToAction("Index");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult AboutMe()
        {
            return View();
        }
    }
}
