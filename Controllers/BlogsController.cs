using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogProj_12_10_22.Data;
using BlogProj_12_10_22.Models;
using BlogProj_12_10_22.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BlogProj_12_10_22.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IConfiguration _configuration;
        public BlogsController(ApplicationDbContext context,
                               IImageService imageService,
                               UserManager<BlogUser> userManager,
                               IConfiguration configuration)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
            _configuration = configuration;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // GET: Blogs
        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Blog.Include(b => b.BlogUser);

            var im = await _imageService.EncodeImageAsync("blogOneWord.jpg");
            var image1 = _imageService.DecodeImage(im, "jpg");
            ViewData["HeaderImage"] = image1;
            ViewData["MainText"] = "Laurence's IT Blog";
            ViewData["SubText"] = "Read posts from my Blogs below";

            return View(await applicationDbContext.ToListAsync());

            
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // GET: Blogs/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog
                .Include(b => b.BlogUser)
                .FirstOrDefaultAsync(m => m.id == id);
            if (blog == null)
            {
                return NotFound();
            }

            if (blog.ImageData != null)
            {
                var image1 = _imageService.DecodeImage(blog.ImageData, blog.ContentType);
                ViewData["HeaderImage"] = image1;
            }
            else
            {
                var imageDef = await _imageService.EncodeImageAsync("blogMainBdrop.jpg");
                var image1 = _imageService.DecodeImage(imageDef, "jpg");
                ViewData["HeaderImage"] = image1;
            }
            ViewData["MainText"] = blog.Name;
            ViewData["SubText"] = blog.Description;

            return View(blog);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // GET: Blogs/Create
        [Authorize]
        public async Task<IActionResult> CreateAsync()
        {
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");

            var im = await _imageService.EncodeImageAsync("blogOneWord.jpg");
            var image1 = _imageService.DecodeImage(im, "jpg");
            ViewData["HeaderImage"] = image1;
            ViewData["MainText"] = "Only for admininstrator";
            ViewData["SubText"] = "Create a blog below";

            return View();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.Created = DateTime.Now;
                blog.BlogUserId = _userManager.GetUserId(User);
                if(blog.Image is not null)
                {
                    blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                    blog.ContentType = _imageService.ContentType(blog.Image);
                }
                _context.Add(blog);

                await _context.SaveChangesAsync();

                ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");

                var im = await _imageService.EncodeImageAsync("blogOneWord.jpg");
                var image1 = _imageService.DecodeImage(im, "jpg");
                ViewData["HeaderImage"] = image1;
                ViewData["MainText"] = "The blog has just been created";
                ViewData["SubText"] = "It should be available in the index";

                return RedirectToAction(nameof(Index));
            }

            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", blog.BlogUserId);
            return View(blog);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // GET: Blogs/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");
            var im = await _imageService.EncodeImageAsync("blogOneWord.jpg");
            var image1 = _imageService.DecodeImage(im, "jpg");
            ViewData["HeaderImage"] = image1;
            ViewData["MainText"] = "Edit blog only for admininstrator";
            ViewData["SubText"] = "Edit the blog below";
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,Image,id")] Blog blog, IFormFile? newImage)
        {
            if (id != blog.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var newblog = await _context.Blog.FindAsync(blog.id);

                    newblog.Updated = DateTime.Now;
                    newblog.Name = blog.Name;
                    newblog.Description = blog.Description;
                    newblog.ImageData = null;
                    newblog.ContentType = null;
                    newblog.ImageData = await _imageService.EncodeImageAsync(newImage);
                    newblog.ContentType = _imageService.ContentType(newImage);
                    //_context.Update(blog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", blog.BlogUserId);
            return View(blog);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // GET: Blogs/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog
                .Include(b => b.BlogUser)
                .FirstOrDefaultAsync(m => m.id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // POST: Blogs/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blog.FindAsync(id);
            _context.Blog.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blog.Any(e => e.id == id);
        }
    }
}
