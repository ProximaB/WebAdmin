using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MSDev.Taskschd.Entities;
using MSDev.Taskschd.Helpers;
using Microsoft.AspNetCore.WebUtilities;
using MSDev.DataAgent.Repositories;
using MSDev.Core.Tools;
using MSDev.DataAgent.Models;

namespace MSDev.Taskschd.Tasks
{
  public class BingNewsTask
  {
    private const String BingSearchKey = "2dd3ac889c7e42d4934d017abf80cae3";
    private const String Domain = "http://msdev.cc/";//TODO: [域名]读取配置
    private const Double Similarity = 0.5;//定义相似度
    private IBingNewsRepository repository;

    public BingNewsTask(IBingNewsRepository repository)
    {
      this.repository = repository;
    }

    public async Task<List<BingNewsEntity>> GetNews(String query, String freshness = "Day")
    {
      //获取新闻
      BingSearchCrawler.SearchApiKey = BingSearchKey;
      var newNews = await BingSearchCrawler.GetNewsSearchResults(query);
      if (newNews == null)
        throw new ArgumentNullException(nameof(newNews));

      //todo:获取过滤来源名单
      String[] providerFilter = { "中金在线", "安卓网资讯专区", "中国通信网", "中国网", "华商网", "A5站长网", "东方财富网 股票", "秦巴在线", "ITBEAR科技资讯", "京华网", "TechWeb", "四海网" };

      //数据预处理
      for (Int32 i = 0; i < newNews.Count; i++)
      {
        //来源过滤
        if (Array.Exists(providerFilter, provider => provider == newNews[i].Provider))
        {
          Console.WriteLine("filter:" + newNews[i].Provider + ":" + newNews[i].Title);
          newNews[i].Title = String.Empty;
          continue;
        }
        //无缩略图过滤
        if (String.IsNullOrEmpty(newNews[i].ThumbnailUrl))
        {
          Console.WriteLine("noPic:" + newNews[i].Title);
          newNews[i].Title = String.Empty;
          continue;
        }

        //TODO: 语义分词重复过滤

        for (Int32 j = i + 1; j < newNews.Count; j++)
        {
          //重复过滤
          if (StringTools.Similarity(newNews[i].Title, newNews[j].Title) > Similarity)
          {
            Console.WriteLine("repeat:" + newNews[i].Title);
            newNews[i].Title = String.Empty;
          }
        }
      }

      //查询库中内容并去重
      HttpClient httpClient = new HttpClient();
      httpClient.GetStringAsync();

      var oldTitles = await repository.GetRecentTitlesAsync(7);
      for (var i = 0; i < newNews.Count; i++)
      {
        if (String.IsNullOrEmpty(newNews[i].Title))
          continue;
        foreach (String oldTitle in oldTitles)
        {
          if (StringTools.Similarity(newNews[i].Title, oldTitle) > Similarity)
          {
            Console.WriteLine("repeat:" + newNews[i].Title);
            newNews[i].Title = String.Empty;
            break;
          }
        }
      }
      //去重后的内容
      var newsTBA = new List<BingNews>();
      foreach (var item in newNews)
      {
        if (String.IsNullOrEmpty(item.Title))
          continue;
        Uri uri = new Uri(item.Url);
        var queryDictionary = QueryHelpers.ParseQuery(uri.Query);
        String targetUrl = queryDictionary["r"];
        targetUrl = Domain + "?r=" + targetUrl;

        var news = new BingNews {
          Title = item.Title,
          Description = item.Description,
          Url = targetUrl,
          ThumbnailUrl = item.ThumbnailUrl,
          Status = 0,
          Tags = query,
          Provider = item.Provider,
          CreatedTime = item.DatePublished,
          UpdatedTime = DateTime.Now
        };
        newsTBA.Add(news);
      }
      var re = repository.AddRange(newsTBA);

      return newNews;
    }
  }
}