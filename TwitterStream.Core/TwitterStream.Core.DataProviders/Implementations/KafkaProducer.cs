using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterStream.Core.Interfaces;

namespace TwitterStream.Core.Implementations
{
    /// <summary>
    /// Class to create subscription based topics (like Kafka topics) for consumers
    /// It depends on a distributed server like Kafka to be running
    /// </summary>
    public interface StreamObjectProducer
    {
        /// <summary>
        /// subscribe to data source and watch for changes
        /// </summary>
        /// <param name="dataSource"></param>
        void SubscribeTo(IObservableList dataSource);

        /// <summary>
        /// unsubscribe from the data source, i.e. stop watching for data
        /// </summary>
        void UnSubscribeFrom();

    }
}
