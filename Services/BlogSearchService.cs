using BlogProj_12_10_22.Data;
using BlogProj_12_10_22.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogProj_12_10_22.Services
{
    public class BlogSearchService
    {
        private readonly ApplicationDbContext _context;

        public BlogSearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Post> Search(string searchTerm)
        {
            var searchT = searchTerm;
            var posts = _context.Post.Where(p => p.ReadyStatus == Enums.ReadyStatus.Incomplete).AsQueryable();

            if (searchT != null)

            {
                //normalise searchTerm:
                var searchTermL = searchT.ToLower();

                    posts = posts.Where(
                    p => p.Title.ToLower().Contains(searchTermL) ||
                    p.Abstract.ToLower().Contains(searchTermL) ||
                    p.Comments.Any(c => c.Body.ToLower().Contains(searchTermL) ||
                                        c.ModeratedBody.ToLower().Contains(searchTermL) ||
                                        c.BlogUser.FirstName.ToLower().Contains(searchTermL) ||
                                        c.BlogUser.LastName.ToLower().Contains(searchTermL) ||
                                        c.BlogUser.Email.ToLower().Contains(searchTermL)));
            }
            return posts;
        }
    }
}
