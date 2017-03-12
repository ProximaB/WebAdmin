﻿using Microsoft.EntityFrameworkCore;
using MsDev.DataAgent.EnumTypes;
using MSDev.DataAgent.Models;
using MsDev.DataAgent.Repositories;
using MsDev.Taskschd.Helpers;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MsDev.Taskschd.Tasks
{
    public class DevBlogsTask
    {
        private IRssRepository repository = null;
        private const string devBlogsFeedsLink = "http://sxp.microsoft.com/feeds/3.0/devblogs";

        public DevBlogsTask(IRssRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> GetNews()
        {
            var blogs = await RssCrawler.GetRss(devBlogsFeedsLink);
            var lastNews = await repository.DbSet.Where(x => x.Type == NewsTypes.DevBlog).LastOrDefaultAsync();
            var _RssNews = blogs.Where(x => x.PublishId > lastNews.PublishId).OrderBy(x => x.PublishId).Select(x => new RssNews
            {
                Title = x.Title,
                Author = x.Author,
                Description = x.Description,
                Categories = JsonConvert.SerializeObject(x.Categories),
                CreateTime = x.CreateTime,
                LastUpdateTime = x.LastUpdateTime,
                Link = x.Link,
                PublishId = x.PublishId,
                MobileContent = x.MobileContent,
                Type = NewsTypes.DevBlog,
                Status = ItemStatus.正常
            });
            return await repository.AddRangeAsync(_RssNews) == _RssNews.Count();
        }
    }
}