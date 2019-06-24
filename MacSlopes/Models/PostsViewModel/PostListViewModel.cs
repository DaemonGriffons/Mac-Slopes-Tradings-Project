using PagedList.Core;

namespace MacSlopes.Models.PostsViewModel
{
    public class PostListViewModel
    {
        public string Search { get; set; }

        public PagedList<PostsIndexViewModel> PostsIndexViewModels { get; set; }
    }
}