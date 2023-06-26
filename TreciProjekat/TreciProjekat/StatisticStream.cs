using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace TreciProjekat
{
    internal class StatisticStream:IObservable<Statistics>
    {
        private readonly Subject<Statistics> statisticsSubject;
        private YouTubeService youtubeService; 
        public StatisticStream()
        {
            statisticsSubject = new Subject<Statistics>();
            youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBNZqERYzi2p8QfUqxEDSjRdz5nyMuWmBk",
                ApplicationName = "YouTube Comments Analysis"
            });
        }

        public void GetStatistic(HttpListenerContext context)
        {
            string videoIdInput = context.Request.Url.LocalPath.Substring(1);
            try
            {
                var videoIDs = videoIdInput.Split('/').Select(id => id.Trim());
                foreach (string videoId in videoIDs)
                {
                    var videoRequest = youtubeService.Videos.List("snippet,statistics");
                    videoRequest.Id = videoId;
                    var videoResponse = videoRequest.Execute();

                    var video = videoResponse.Items[0];

                    var statistic = new Statistics
                    {
                        commentCount = (int)video.Statistics.CommentCount,
                        likeCount = (int)video.Statistics.LikeCount,
                        viewCount = (int)video.Statistics.ViewCount
                    };
                    statisticsSubject.OnNext(statistic);
                }
            }
            catch (Exception e)
            {
                statisticsSubject.OnError(e);
            }
        }

        public IDisposable Subscribe(IObserver<Statistics> observer)
        {
            return statisticsSubject.Subscribe(observer);
        }
    }
}
