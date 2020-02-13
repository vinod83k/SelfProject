using System;

namespace Project.CommandProcessor
{
    public class RequestProcessor : IRequestProcessor
    {
        private readonly IServiceProvider _container;

        public RequestProcessor(IServiceProvider container)
        {
            _container = container;
        }

        public TResult Process<TCommand, TResult>(TCommand command)
            where TCommand : IRequest
            where TResult : RequestResult
        {
            var handler = _container.GetService(typeof(IRequestHandler<TCommand, TResult>));
            try
            {
                //return handler.Handle(command);
                return null;
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