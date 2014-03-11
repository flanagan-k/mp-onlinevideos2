﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using Newtonsoft.Json.Linq;
using OnlineVideos.MPUrlSourceFilter;

namespace OnlineVideos.Sites
{
    public class FilmonUtil : SiteUtilBase
    {
        private CookieContainer cc;
        private const string userAgent = @"Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
        public override int DiscoverDynamicCategories()
        {
            if (Settings.Categories == null) Settings.Categories = new BindingList<Category>();
            cc = new CookieContainer();
            string data = GetWebData(@"https://www.filmon.com/tv/live", userAgent: userAgent, cc: cc);
            string jsondata = @"{""result"":" + GetSubString(data, "var groups =", @"if(!$.isArray").Trim().TrimEnd(';') + "}";
            JToken jt = JObject.Parse(jsondata) as JToken;
            foreach (JToken jCat in jt["result"] as JArray)
            {
                RssLink cat = new RssLink();
                cat.Name = jCat.Value<string>("title");
                cat.Description = jCat.Value<string>("description");
                cat.Thumb = jCat.Value<string>("logo_uri");
                Settings.Categories.Add(cat);
                JArray channels = jCat["channels"] as JArray;
                List<VideoInfo> videos = new List<VideoInfo>();
                foreach (JToken channel in channels)
                {
                    VideoInfo video = new VideoInfo();
                    video.ImageUrl = channel.Value<string>("logo");
                    video.Description = channel.Value<string>("description");
                    video.Title = channel.Value<string>("title");
                    video.VideoUrl = @"https://www.filmon.com/ajax/getChannelInfo";
                    video.Other = String.Format(@"channel_id={0}&quality=low", channel.Value<string>("id"));
                    videos.Add(video);
                }
                cat.Other = videos;
            }
            Settings.DynamicCategoriesDiscovered = true;
            return Settings.Categories.Count;
        }

        public override List<VideoInfo> getVideoList(Category category)
        {
            return (List<VideoInfo>)category.Other;
        }

        public override string getUrl(VideoInfo video)
        {
            CookieContainer newCc = new CookieContainer();
            foreach (Cookie c in cc.GetCookies(new Uri(@"https://www.filmon.com/")))
            {
                newCc.Add(c);
            }

            NameValueCollection headers = new NameValueCollection();
            headers.Add("Accept", "*/*");
            headers.Add("User-Agent", userAgent);
            headers.Add("X-Requested-With", "XMLHttpRequest");
            string webdata = GetWebData(video.VideoUrl, (string)video.Other, headers, newCc, null, false, false, null, false);

            string jsondata = @"{""result"":" + webdata + "}";

            JToken jt = JObject.Parse(jsondata) as JToken;
            jt = (jt["result"] as JArray)[0];
            string serverUrl = jt.Value<string>("serverURL");
            RtmpUrl res = new RtmpUrl(serverUrl);
            res.PlayPath = jt.Value<string>("streamName");

            int p = serverUrl.IndexOf("live/?id");
            res.App = serverUrl.Substring(p);

            return res.ToString();
        }
        private static string GetSubString(string s, string start, string until)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            int p = s.IndexOf(start);
            if (p == -1) return String.Empty;
            p += start.Length;
            if (until == null) return s.Substring(p);
            int q = s.IndexOf(until, p);
            if (q == -1) return s.Substring(p);
            return s.Substring(p, q - p);
        }


    }
}
