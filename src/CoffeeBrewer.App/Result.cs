namespace CoffeeBrewer.App
{
    public interface IResult<T>;

    public class Result<T> : IResult<T>
    {
        public T? Value { get; }
        public Exception? Exception { get; }
        public bool HasError => Exception != null;

        public Result(T value)
        {
            Value = value;
        }

        public Result(Exception exception)
        {
            Exception = exception;
        }
    }
}
