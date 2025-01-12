using AutoMapper;
using Streetcode.BLL.DTO.News;
using Streetcode.Domain.Entities.News;

namespace Streetcode.BLL.Mapping.Newss
{
    public class NewsProfile : Profile
    {
        public NewsProfile()
        {
            CreateMap<News, NewsDto>().ReverseMap();
        }
    }
}
