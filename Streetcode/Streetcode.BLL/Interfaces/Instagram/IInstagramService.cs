using Streetcode.Domain.Entities.Instagram;

namespace Streetcode.BLL.Interfaces.Instagram
{
    public interface IInstagramService
    {
        Task<IEnumerable<InstagramPost>> GetPostsAsync();
    }
}
