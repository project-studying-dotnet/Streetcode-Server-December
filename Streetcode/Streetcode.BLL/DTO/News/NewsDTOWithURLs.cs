﻿namespace Streetcode.BLL.DTO.News
{
    public class NewsDtoWithURLs
    {
        public NewsDto News { get; set; } = new NewsDto();
        public string? PrevNewsUrl { get; set; }
        public string? NextNewsUrl { get; set; }
        public RandomNewsDto? RandomNews { get; set; } = new RandomNewsDto();
    }
}
