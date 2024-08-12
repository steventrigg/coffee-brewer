namespace CoffeeBrewer.App
{
    public interface IPolicy<T>
    {
        public Task<T> ApplyPolicyAsync(T model, CancellationToken ctx);
    }
}
