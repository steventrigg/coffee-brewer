﻿using Amazon.CDK;
using Amazon.CDK.AWS.Apigatewayv2;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AwsApigatewayv2Integrations;
using Constructs;
using System.Collections.Generic;
using HttpMethod = Amazon.CDK.AWS.Apigatewayv2.HttpMethod;

namespace CoffeeBrewer.Cdk.Cicd.AppStack
{
    public class CoffeeBrewerAppStack : Stack
    {
        public CoffeeBrewerAppStack(Construct scope, string id, StackProps props = null) : base(scope, id, props)
        {
            var buildOption = new BundlingOptions()
            {
                Image = Runtime.DOTNET_8.BundlingImage,
                User = "root",
                OutputType = BundlingOutput.ARCHIVED,
                Command = new[]
                {
                    "/bin/sh",
                    "-c",
                    " dotnet tool install -g Amazon.Lambda.Tools" +
                    " && dotnet build" +
                    " && dotnet lambda package -pl CoffeeBrewer.Api --output-package /asset-output/function.zip"
                }
            };

            var table = new Table(this, "CoffeeBrewerHopperLevel", new TableProps
            {
                TableName = "CoffeeBrewerHopperLevel",
                PartitionKey = new Attribute { Name = "Key", Type = AttributeType.STRING },
                BillingMode = BillingMode.PAY_PER_REQUEST,
            });

            var coffeeBrewerApiLambda = new Function(this, "CoffeeBrewerApiFunction", new FunctionProps
            {
                Runtime = Runtime.DOTNET_8,
                Handler = "CoffeeBrewer.Api::CoffeeBrewer.Api.LambdaEntryPoint::FunctionHandlerAsync",
                Code = Code.FromAsset("./src", new Amazon.CDK.AWS.S3.Assets.AssetOptions
                {
                    Bundling = buildOption
                }),
                Environment = new Dictionary<string, string>
                {
                    { "HopperLevelTableName", table.TableName }
                }
            });

            table.GrantReadWriteData(coffeeBrewerApiLambda);

            var api = new HttpApi(this, "CoffeeBrewerHttpApi", new HttpApiProps
            {
                ApiName = "CoffeeBrewerHttpApi"
            });

            var integration = new HttpLambdaIntegration("CoffeeBrewerHttpApiIntegration", coffeeBrewerApiLambda);

            api.AddRoutes(new AddRoutesOptions
            {
                Path = "/{proxy+}",
                Methods = new[] { HttpMethod.ANY },
                Integration = integration
            });
        }
    }
}