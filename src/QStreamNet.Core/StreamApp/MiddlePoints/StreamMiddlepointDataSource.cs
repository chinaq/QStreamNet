using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStreamNet.Core.StreamApp.MiddlePoints
{
    public class StreamMiddlepointDataSource
    {
        
        public ICollection<StreamMiddlepoint> Middlepoints { get; }
        // public StreamMiddlepoint MiddleEndpoint { get; set; }

        public StreamMiddlepointDataSource()
        {
            Middlepoints = new List<StreamMiddlepoint>();
        }
    }
}