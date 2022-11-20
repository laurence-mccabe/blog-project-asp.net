using BlogProj_12_10_22.Data;
using BlogProj_12_10_22.Models;
using BlogProj_12_10_22.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using BlogProj_12_10_22.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace BlogProj_12_10_22.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISlugService _slugService;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly BlogSearchService _blogSearchSer;
        private readonly IConfiguration _configuration;
        public PostsController(ApplicationDbContext context,
                               ISlugService slugService,
                               IImageService imageService,
                               UserManager<BlogUser> userManager,
                               BlogSearchService blogSearchSer,
                               IConfiguration configuration)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _userManager = userManager;
            _blogSearchSer = blogSearchSer;
            _configuration = configuration;
        }
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> TagIndex(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Tag = await _context.Tag
                // .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                // .Include(p => p.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Tag == null)
            {
                return NotFound();
            }

            return View(Tag);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // GET: Posts
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Post.Include(p => p.Blog).Include(p => p.BlogUser).Include(p => p.Tags);

            var image = await _imageService.EncodeImageAsync("BlogPosts.jpg");
            var image1 = _imageService.DecodeImage(image, "jpg");
            ViewData["HeaderImage"] = image1;
            ViewData["MainText"] = "The Blog Posts Index";
            ViewData["SubText"] = "See a list of all the blog posts below";

            return View(await applicationDbContext.ToListAsync());
            
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // GET: BlogPostIndex
        public async Task<IActionResult> BlogPostsIndex(int? id,int? page)
           
        {
            var pageNumber = page ?? 1;
            var pageSize = 3;
            if (id is null) { return NotFound(); }
            var bpIndex = await _context.Post.Include(b => b.BlogUser).Where(b => b.BlogId == id)
                                .OrderByDescending(p => p.Created)
                                .Include(b => b.Blog).Include(p => p.Tags).ToPagedListAsync(pageNumber, pageSize);
            var blog = await _context.Blog.Where(b => b.id == id).FirstOrDefaultAsync();
            if(blog.ImageData is not null)
            {
                ViewData["HeaderImage"] = _imageService.DecodeImage(blog.ImageData, blog.ContentType);
            }
            else {
                var image = await _imageService.EncodeImageAsync("BlogPosts.jpg");
                var image1 = _imageService.DecodeImage(image, "jpg");
                ViewData["HeaderImage"] = image1;
            }
            ViewData["MainText"] = $"Posts for {blog.Name}";
            ViewData["SubText"] = blog.Description;

            return View(bpIndex);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // GET: SearchIndex
        ///
        public async Task<IActionResult> SearchIndex(int? page, List<string> searchTerm)
        {
            var pageNum = page ?? 1;
            var pageSize = 100;
            ViewData["SearchTerm"] = searchTerm;
            var SearchTerm = searchTerm[0];

            var posts = _blogSearchSer.Search(SearchTerm);

            var image = await _imageService.EncodeImageAsync("blogMainBdrop.jpg");
            var image1 = _imageService.DecodeImage(image, "jpg");
            ViewData["HeaderImage"] = image1;
            ViewData["MainText"] = "Search Results..";
            ViewData["SubText"] = "See search results from matching Blogs and Posts below";

            return View(await posts.ToPagedListAsync(pageNum,pageSize));
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        // GET: Posts/Details/5
        [Authorize]
        public async Task<IActionResult> Details(string slug)
        {
            if (slug == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null)
            {
                return NotFound();
            }

            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
            ViewData["ModerationTypes"] = new SelectList(Enum.GetNames(typeof(Enums.ModerationType)));

            if(post.ImageData is not null)
            {
                ViewData["HeaderImage"] = _imageService.DecodeImage(post.ImageData, post.ContentType);

            }
            else {
                var image = await _imageService.EncodeImageAsync("BlogPosts.jpg");
                var image1 = _imageService.DecodeImage(image, "jpg");
                ViewData["HeaderImage"] = image1;
            }
            ViewData["MainText"] = post.Title;
            ViewData["SubText"] = post.Abstract;

            return View(post);
        }


        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        public async Task<IActionResult> BpIndexDetails(string slug)
        {
            if (slug == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null)
            {
                return NotFound();
            }

            var dataVM = new PostDetailViewModel()

            {
                Post = post,
                Tags = _context.Tag.Select(t => t.Text.ToLower()).Distinct().ToList()
            };

            var image = await _imageService.EncodeImageAsync("blogMainBdrop.jpg");
            var image1 = _imageService.DecodeImage(image, "jpg");
            ViewData["HeaderImage"] = image1;
            ViewData["MainText"] = post.Title;
            ViewData["SubText"] = post.Abstract;
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));
            ViewData["ModerationTypes"] = new SelectList(Enum.GetNames(typeof(Enums.ModerationType)));

            

            return View(dataVM);
        }


        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        // GET: Posts/Create
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateAsync()
        {

            ViewData["BlogId"] = new SelectList(_context.Blog, "id", "Name");
            ViewData["BlogUserId"] = new SelectList(_context.Users, "FirstName");

            var image = await _imageService.EncodeImageAsync("BlogPosts.jpg");
            var image1 = _imageService.DecodeImage(image, "jpg");
            ViewData["HeaderImage"] = image1;

            ViewData["MainText"] = "Create a Post page.";
            ViewData["SubText"] = "Enter details below..";

            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("BlogId", "BlogUser", "Abstract", "ReadyStatus", "Title", "Content")] Post post, List<string> TagValues)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now;
                var authorId = _userManager.GetUserId(User);
                if(post.Image != null)
                {
                    post.ImageData = await _imageService.EncodeImageAsync(post.Image);
                    post.ContentType = _imageService.ContentType(post.Image);
                }
                
                post.BlogUserId = authorId;



                var slug = _slugService.UrlFriendly(post.Title);
                if (!_slugService.isUnique(slug) || string.IsNullOrEmpty(slug))
                {
                    ModelState.AddModelError("Title", "The title you provided is blank or already in use, please try again.");
                    ViewData["TagValues"] = string.Join(",", TagValues);
                    return View(post);
                }

                post.Slug = slug;

                _context.Add(post);
                await _context.SaveChangesAsync();


                foreach (var Tag in TagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        Text = Tag

                    });
                }
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "id", "Description", post.BlogId);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // GET: Posts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }


            ViewData["BlogId"] = new SelectList(_context.Blog, "id", "Name", post.BlogId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));

            var image = await _imageService.EncodeImageAsync("BlogPosts.jpg");
            var image1 = _imageService.DecodeImage(image, "jpg");
            ViewData["HeaderImage"] = image1;

            ViewData["MainText"] = "Edit your post here.";
            ViewData["SubText"] = "Edit your post details below..";

            return View(post);

        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus,Image")] Post post, List<string> TagValues)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    var origPost = await _context.Post.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == post.Id);

                    if (post.Title != origPost.Title)
                    {
                        var slug = _slugService.UrlFriendly(post.Title);
                        if (!_slugService.isUnique(slug) || string.IsNullOrEmpty(slug))
                        {
                            ModelState.AddModelError("Title", "The title you provided is blank or already in use, please try again.");
                            ViewData["TagValues"] = string.Join(",", TagValues);
                            ViewData["BlogId"] = new SelectList(_context.Blog, "id", "Name", post.BlogId);

                            return View(post);
                        }
                        origPost.Title = post.Title;
                        origPost.Slug = post.Slug;
                    }


                    origPost.Updated = DateTime.Now;
                    origPost.Title = post.Title;
                    origPost.Abstract = post.Abstract;
                    origPost.Content = post.Content;
                    origPost.ReadyStatus = post.ReadyStatus;

                    
                    if (post.Image is not null)
                    {
                        origPost.ImageData = await _imageService.EncodeImageAsync(post.Image);
                        origPost.ContentType = _imageService.ContentType(post.Image);
                    }

                    
                    _context.Tag.RemoveRange(origPost.Tags);

                    await _context.SaveChangesAsync();
                    foreach (var Tag in TagValues)
                    {
                        _context.Add(new Tag()
                        {
                            PostId = post.Id,
                            BlogUserId = post.BlogUserId,
                            Text = Tag
                        });
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "id", "Description", post.BlogId);
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", post.BlogUserId);
            ViewData["TagValues"] = string.Join(",", post.Tags.Select(t => t.Text));

            return RedirectToAction(nameof(Index));
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // GET: Posts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            var image = await _imageService.EncodeImageAsync("BlogPosts.jpg");
            var image1 = _imageService.DecodeImage(image, "jpg");
            ViewData["HeaderImage"] = image1;

            ViewData["MainText"] = "Post deletion page";
            ViewData["SubText"] = "..think carefully";

            return View(post);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        // POST: Posts/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
