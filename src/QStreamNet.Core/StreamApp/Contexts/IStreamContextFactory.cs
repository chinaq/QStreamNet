using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TcpNet.Pipelines;

namespace QStreamNet.Core.StreamApp.Contexts
{
    public interface IStreamContextFactory
    {
        StreamContext Create();
    }
}