using AppHost;

//
// NOTE: The .csproj file for the AppHost project declares
//       a two-step pre-build action. The first step checks for the existence
//       of a top-level node_modules file in ../TurboRepo/, and,
//       if it does not exist, runs pnpm install to create and populate the
//       node_modules folder(s). The second step runs a top level pnpm build,
//       which runs turbo build. Turbo build automatically caches build outputs,
//       so subsequent builds of the node apps are generally quite fast.
//

var builder = DistributedApplication.CreateBuilder(args);

var express = builder.AddGenericNodeApp("express", "../TurboRepo", "pnpm", "start-express")
    .WithHttpEndpoint(targetPort: 3000, env: "PORT")
    .WithExternalHttpEndpoints()
    .MyPublishAsDockerFile("apps/express/Dockerfile");

var nextjs = builder.AddGenericNodeApp("nextjs", "../TurboRepo", "pnpm", "start-nextjs")
    .WithHttpEndpoint(targetPort: 3000, env: "PORT")
    .WithExternalHttpEndpoints()
    .MyPublishAsDockerFile("apps/nextjs/Dockerfile");

builder.Build().Run();
