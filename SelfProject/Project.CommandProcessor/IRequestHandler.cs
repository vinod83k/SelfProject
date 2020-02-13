namespace Project.CommandProcessor
{
	public interface IRequestHandler<TCommand, TResult> where TCommand : IRequest
	{
		TResult Handle(TCommand cmdParms);
	}
}