namespace QStreamNet.Core.Tcp.Servers
{
    public class TPort
    {
        public int Num { get; set; }
        public int ReadTimeoutSec { get; set; }
        public int LimitConnections { get; set; }
    }
}