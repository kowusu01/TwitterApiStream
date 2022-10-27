using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterStream.Core.Models;

namespace TwitterStream.Core.Interfaces
{
    /// <summary>
    /// an observable list of raw tweet
    /// </summary>
    public interface IObservableList : IObservable<IEnumerable<RawTweet>>
    {
        void Add(RawTweet rawTweet);
        void Remove(int index);
        RawTweet Get(int index);
    }
}
