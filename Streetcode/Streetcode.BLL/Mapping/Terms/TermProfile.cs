using AutoMapper;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.DTO.Terms;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Mapping.Terms
{
    public class TermProfile : Profile
    {
        public TermProfile()
        {
            CreateMap<TermCreateDTO, Term>();
            CreateMap<Term, TermDto>();
        }
    }
}
