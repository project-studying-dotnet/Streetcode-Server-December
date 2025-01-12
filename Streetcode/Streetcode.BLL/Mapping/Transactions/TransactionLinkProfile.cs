﻿using AutoMapper;
using Streetcode.BLL.DTO.Transactions;
using Streetcode.Domain.Entities.Transactions;

namespace Streetcode.BLL.Mapping.Transactions
{
    public class TransactionLinkProfile : Profile
    {
        public TransactionLinkProfile()
        {
            CreateMap<TransactionLink, TransactLinkDto>()
               .ReverseMap();
        }
    }
}