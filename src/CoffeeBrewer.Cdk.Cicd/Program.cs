using Amazon.CDK;

namespace CoffeeBrewer
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new CoffeeBrewerCicdStack(app, "CoffeeBrewerStack", new StackProps
            {
                Env = new Environment
                {
                    Account = "268258138285",
                    Region = "us-west-2",
                }
            });
            app.Synth();
        }
    }
}
