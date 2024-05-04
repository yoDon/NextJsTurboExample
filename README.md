# Aspire/Aspirate Next.JS Express Docker Example

This example shows a Vercel Turbo monorepo that builds and runs a Next.Js site and an Express site,
configured for hosting and deployment using .NET Aspire and Aspirate (aspir8).

## Running locally

Start the AppHost project by any of the following methods:

- Run the `AppHost` project in Visual Studio.
- Install the Rider Aspire Plugin in Rider and run the `AppHost: http` configuration in Rider.
- `dotnet run --project AppHost` from the command line.

Running the solution will open the Aspire dashboard in your browser. 
You can use the dashboard to open the express and nextjs apps.

## Deploying to Kubernetes

- Launch Rancher Desktop to provide a local Kubernetes cluster (Docker Desktop
also provides a Kubernetes cluster, but Docker Desktop is more difficult to
expose the required ports).
- From the command line, perform the following steps to deploy the solution to Kubernetes:

```bash
cd AppHost
aspirate init
aspirate generate
aspirate apply
```

- In the Rancher Desktop UI, go to the Port Forwarding tab forward:

  - aspire-dashboard (dashboard-ui)
  - express (http)
  - nextjs (http)

- For each of the exposed ports, browse to `http://localhost:<port>` to access the services.

- When you are done, run `aspirate destroy` to remove the Kubernetes deployment.