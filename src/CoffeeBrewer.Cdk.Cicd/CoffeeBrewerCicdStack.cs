using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;
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
                    new CodeBuildStep("RunTests", new CodeBuildStepProps
                    {
                        BuildEnvironment = new BuildEnvironment
                        {
                            BuildImage = LinuxBuildImage.FromDockerRegistry("mcr.microsoft.com/dotnet/sdk:8.0"),
                            ComputeType = ComputeType.SMALL
                        },
                        Commands = new[]
                        {
                            "dotnet test src/CoffeeBrewer.Api.Tests -c Release --logger trx",
                            "dotnet test src/CoffeeBrewer.App.Tests -c Release --logger trx"
                        }
                    })
                }
            });
        }
    }
}
