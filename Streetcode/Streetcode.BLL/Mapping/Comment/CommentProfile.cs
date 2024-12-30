using AutoMapper;
using Streetcode.BLL.DTO.Analytics;
using Streetcode.BLL.DTO.Comment;
using Streetcode.DAL.Entities.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentEntity = Streetcode.DAL.Entities.Comment.Comment;

namespace Streetcode.BLL.Mapping.Comment
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            // Map from Comment to GetCommentDto
            CreateMap<CommentEntity, GetCommentDto>()
                .ForMember(dest => dest.Children, opt => opt.MapFrom(src => src.Children));
            
            CreateMap<CreateCommentDto, CommentEntity>();
            CreateMap<CommentEntity, GetCommentsToReviewDto>();
        }
    }
}
