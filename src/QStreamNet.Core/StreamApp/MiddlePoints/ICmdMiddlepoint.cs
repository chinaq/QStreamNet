using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace QStreamNet.Core.StreamApp.MiddlePoints
{
    // [RequiresPreviewFeatures]
    public interface ICmdMiddlepoint
    {
        static virtual byte[]? CmdData { get; }
        static virtual string? CmdName { get; }

        // static virtual byte[] CmdData { get => new byte[] {}; }
        // static virtual string CmdName { get => "unknown"; }
    }
}