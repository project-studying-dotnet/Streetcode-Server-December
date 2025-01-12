using AutoMapper;
using Streetcode.BLL.DTO.Feedback;
using Streetcode.Domain.Entities.Feedback;

namespace Streetcode.BLL.Mapping.Feedback
{
    public class ResponseProfile : Profile
    {
        public ResponseProfile()
        {
            CreateMap<Response, ResponseDto>().ReverseMap();
        }
    }
}