using Amazon.CDK;
using Amazon.CDK.AWS.CodeBuild;
using Amazon.CDK.AWS.Lambda;
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

            pipeline.AddStage(new AppDeployStage(this, "App Deploy Stage"), new AddStageOpts
            {
                Pre = new[]
                {
                    new CodeBuildStep("Run Tests", new CodeBuildStepProps
                    {
                        BuildEnvironment = new BuildEnvironment
                        {
                            BuildImage = LinuxBuildImage.AMAZON_LINUX_2_5,
                            ComputeType = ComputeType.SMALL
                        },
                        Commands = new[]
                        {
                            "dotnet test src -c Release --logger trx"
                        }
                    })
                }
            });
        }
    }
}
