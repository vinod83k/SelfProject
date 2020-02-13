namespace Project.CommandProcessor
{
    public interface IRequestProcessor
    {
        TResult Process<TCommand, TResult>(TCommand command)
            where TCommand : IRequest
            where TResult : RequestResult;
    }
}