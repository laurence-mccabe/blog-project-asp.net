namespace BlogProj_12_10_22.Services
{
    public interface ISlugService
    {
        string UrlFriendly(string title);

        bool isUnique(string slug);
    }
}
