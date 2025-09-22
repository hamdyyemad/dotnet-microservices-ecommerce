using MediatR;

namespace BuildingBlocks.CQRS
{
    // For commands that DO return data 
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
    // For commands that don't return data (Most common - Success/completion indication without actual data)
    public interface ICommand : ICommand<Unit>
    {
    }
}
