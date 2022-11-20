using BlogProj_12_10_22.Models;

namespace BlogProj_12_10_22.ViewModels
{
    public class PostDetailViewModel
    {
        public Post? Post { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
