using MediatR;

namespace CoffeeBrewer.App
{
    public class ValidationBehaviour<TRequest, IResult> : IPipelineBehavior<TRequest, IResult> where TRequest : IRequest<IResult>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<IResult> Handle(TRequest request, RequestHandlerDelegate<IResult> next, CancellationToken cancellationToken)
        {
            var validators = _validators.Where(x => x is IValidator<TRequest>);

            foreach (var validator in _validators)
            {
                var ex = validator.Validate(request);
                if (ex != null)
                {
                    // Short-circuit. A more ideal alternative would be to check all validators and return a list.
                    var result = (IResult?)Activator.CreateInstance(typeof(IResult), ex);

                    if (result != null)
                    {
                        return Task.FromResult(result);
                    }

                    throw new Exception("Instance activation failed");
                }
            }

            return next();
        }
    }
}
