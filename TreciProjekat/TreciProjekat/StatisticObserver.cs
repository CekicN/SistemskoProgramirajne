using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreciProjekat
{
    internal class StatisticObserver : IObserver<Statistics>
    {
        public void OnNext(Statistics statistic)
        {
            decimal likes = (decimal)(((double)statistic.likeCount / (double)statistic.viewCount) * 100);
            Console.WriteLine(
                $"ViewCount={statistic.viewCount},\n " +
                $"CommentCount={statistic.commentCount},\n " +
                $"LikeCount={statistic.likeCount},\n" +
                $"Likes in %: {decimal.Round(likes, 2, MidpointRounding.AwayFromZero)}%");
        }
        public void OnError(Exception e)
        {
            Console.WriteLine($"Doslo je do greske: {e.Message}");
        }
        public void OnCompleted()
        {
            Console.WriteLine($"Uspesno vraceni svi clanci.");
        }
    }
}
