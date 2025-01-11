using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService.BLL.DTO.ConsumerDtos
{
    public class EmailMessageConsumerDto
    {
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Subject { get; set; } = null!;
    }
}
