using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QStreamNet.Core.StreamApp.MiddlePoints
{
    public interface ICmdPoint
    {
        
        byte[] CmdData { get; }
        string CmdName { get; }
    }
}