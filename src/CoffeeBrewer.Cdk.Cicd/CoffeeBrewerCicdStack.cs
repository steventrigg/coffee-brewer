using Amazon.CDK;
using Amazon.CDK.Pipelines;
using CoffeeBrewer.Cdk.Cicd.AppStack;
using Constructs;

namespace CoffeeBrewer
{
    public class CoffeeBrewerCicdStack : Stack
    {
        internal CoffeeBrewerCicdStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var pipeline = new CodePipeline(this, "Pipeline", new CodePipelineProps
            {
                SelfMutation = true,
                Synth = new ShellStep("Synth", new ShellStepProps
                {
                    Input = CodePipelineSource.GitHub("steventrigg/coffee-brewer", "main"),
                    Commands = new[]
                    {
                        "npm install -g aws-cdk",
                        "cdk synth"
                    }
                })
            });

            pipeline.AddStage(new AppDeployStage(this, "AppDeployStage"), new AddStageOpts
            {
                Pre = new[]
                {
                    new ShellStep("RunTests", new ShellStepProps
                    {
                        Commands = new[]
                        {
                            "/bin/sh",
                            "-c",
                            " dotnet tool install -g Amazon.Lambda.Tools" +
                            " && dotnet build" +
                            " && dotnet test"
                        }
                    })
                }
            });
        }
    }
}
