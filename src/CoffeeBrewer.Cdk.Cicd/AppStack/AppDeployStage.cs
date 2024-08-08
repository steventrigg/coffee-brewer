using Amazon.CDK;
using Constructs;

namespace CoffeeBrewer.Cdk.Cicd.AppStack
{
    public class AppDeployStage : Stage
    {
        public AppDeployStage(Construct scope, string id, IStageProps props = null) : base(scope, id, props)
        {
            _ = new CoffeeBrewerAppStack(this, "CoffeeBrewerApiStack");
        }
    }
}
