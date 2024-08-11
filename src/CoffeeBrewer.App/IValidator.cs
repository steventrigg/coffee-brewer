using MediatR;

namespace CoffeeBrewer.App
{
    public interface IValidator<TRequest> where TRequest : IBaseRequest
    {
        public Exception? Validate(TRequest request);
    }
}
