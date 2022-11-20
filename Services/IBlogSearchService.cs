using X.PagedList;

namespace BlogProj_12_10_22.Services
{
    public interface IBlogSearchService
    {
        Task<IPagedList> Search(string searchTerm, int? pagen, int? pages);
    }
}
