using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Project.CommandProcessor
{
    public class RequestProcessor : IRequestProcessor
    {
        private readonly ILifetimeScope _provider;

        public RequestProcessor(ILifetimeScope provider)
        {
            _provider = provider;
        }
        public TResult Process<TCommand, TResult>(TCommand command)
            where TCommand : IRequest
            where TResult : RequestResult
        {
            var handler = _provider.Resolve<IRequestHandler<TCommand, TResult>>();
            try
            {
                return handler.Handle(command);
            }
            catch (Exception ex)
            {
                if (ExceptionHelper.CanHandle(ex))
                {
                    var result = (RequestResult) Activator.CreateInstance(typeof (TResult));
                    result.Errors.Add(ExceptionHelper.GetMessage(ex));
                    return (TResult) result;
                }
                throw;
            }
        }
    }
}