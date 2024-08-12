using MediatR;

namespace CoffeeBrewer.App
{
    public interface IValidator<TRequest> where TRequest : IBaseRequest
    {
        public Task<Exception?> ValidateAsync(TRequest request);
    }
}
