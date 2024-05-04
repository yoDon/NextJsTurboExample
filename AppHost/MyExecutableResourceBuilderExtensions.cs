// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Publishing;
using Aspire.Hosting.Utils;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding executable resources to the <see cref="IDistributedApplicationBuilder"/> application model.
/// </summary>
public static class ExecutableResourceBuilderExtensions
{
    /// <summary>
    /// Adds annotation to <see cref="ExecutableResource" /> to support containerization during deployment.
    /// </summary>
    /// <typeparam name="T">Type of executable resource</typeparam>
    /// <param name="builder">Resource builder</param>
    /// <param name="dockerfilePathFromWorkingDirectory">path from working directory to Dockerfile, for example foo/Dockerfile</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    [Obsolete("Use PublishAsDockerFile instead")]
    public static IResourceBuilder<T> MyAsDockerfileInManifest<T>(this IResourceBuilder<T> builder,
        string dockerfilePathFromWorkingDirectory) where T : ExecutableResource
    {
        return builder.MyPublishAsDockerFile(dockerfilePathFromWorkingDirectory);
    }

    /// <summary>
    /// Adds annotation to <see cref="ExecutableResource" /> to support containerization during deployment.
    /// The resulting container image is built, and when the optional <paramref name="buildArgs"/> are provided
    /// they're used with <c>docker build --build-arg</c>.
    /// </summary>
    /// <typeparam name="T">Type of executable resource</typeparam>
    /// <param name="builder">Resource builder</param>
    /// <param name="dockerfilePathFromWorkingDirectory">path from working directory to Dockerfile, for example foo/Dockerfile</param>
    /// <param name="buildArgs">The optional build arguments, used with <c>docker build --build-args</c>.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<T> MyPublishAsDockerFile<T>(this IResourceBuilder<T> builder, string dockerfilePathFromWorkingDirectory, IEnumerable<DockerBuildArg>? buildArgs = null) where T : ExecutableResource
    {
        return builder.WithManifestPublishingCallback(context => MyWriteExecutableAsDockerfileResourceAsync(context, builder.Resource, dockerfilePathFromWorkingDirectory, buildArgs));
    }

    private static async Task MyWriteExecutableAsDockerfileResourceAsync(ManifestPublishingContext context, ExecutableResource executable, string dockerFilePathFromWorkingDirectory, IEnumerable<DockerBuildArg>? buildArgs = null)
    {
        context.Writer.WriteString("type", "dockerfile.v0");

        var appHostRelativePathToDockerfile = Path.Combine(executable.WorkingDirectory, dockerFilePathFromWorkingDirectory);
        var manifestFileRelativePathToDockerfile = context.GetManifestRelativePath(appHostRelativePathToDockerfile);
        context.Writer.WriteString("path", manifestFileRelativePathToDockerfile);

        var manifestFileRelativePathToContextDirectory = context.GetManifestRelativePath(executable.WorkingDirectory);
        context.Writer.WriteString("context", manifestFileRelativePathToContextDirectory);

        if (buildArgs is not null)
        {
            MyWriteDockerBuildArgs(context, buildArgs);
        }

        await context.WriteEnvironmentVariablesAsync(executable).ConfigureAwait(false);
        context.WriteBindings(executable);
    }
    
    private static void MyWriteDockerBuildArgs(ManifestPublishingContext context, IEnumerable<DockerBuildArg>? buildArgs)
    {
        if (buildArgs?.ToArray() is { Length: > 0 } args)
        {
            context.Writer.WriteStartObject("buildArgs");

            for (var i = 0; i < args.Length; i++)
            {
                var buildArg = args[i];

                var valueString = buildArg.Value switch
                {
                    string stringValue => stringValue,
                    IManifestExpressionProvider manifestExpression => manifestExpression.ValueExpression,
                    bool boolValue => boolValue ? "true" : "false",
                    null => null, // null means let docker build pull from env var.
                    _ => buildArg.Value.ToString()
                };

                context.Writer.WriteString(buildArg.Name, valueString);

                context.TryAddDependentResources(buildArg.Value);
            }

            context.Writer.WriteEndObject();
        }
    }
}
