using TcpNet.Pipelines;

namespace QStreamNet.Core.StreamApp.Middlewares
{
    public interface IStreamMiddlewareFactory
    {
        //  IMiddleware? Create(Type middlewareType);       
        IStreamMiddleware? Create<TMiddleware>() where TMiddleware: IStreamMiddleware;
    }
}
