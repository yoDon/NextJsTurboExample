# Turborepo example

This example also shows how to use [Workspace Configurations](https://turbo.build/repo/docs/core-concepts/monorepos/configuring-workspaces).

## Using this example

Run the following command:

```sh
pnpm install
pnpm start-express
pnpm start-nextjs
```

## What's inside?

This Turborepo includes the following packages and apps:

### Apps and Packages

- `express`: an [Express](https://expressjs.com/) server
- `nextjs`: a [Next.js](https://nextjs.org/) app
- `@repo/eslint-config`: ESLint configurations used throughout the monorepo
- `@repo/jest-presets`: Jest configurations
- `@repo/logger`: isomorphic logger (a small wrapper around console.log)
- `@repo/ui`: a dummy React UI library (which contains `<CounterButton>` and `<Link>` components)
- `@repo/typescript-config`: tsconfig.json's used throughout the monorepo

Each package and app is 100% [TypeScript](https://www.typescriptlang.org/).

### Utilities

This Turborepo has some additional tools already setup for you:

- [TypeScript](https://www.typescriptlang.org/) for static type checking
- [ESLint](https://eslint.org/) for code linting
- [Jest](https://jestjs.io) test runner for all things JavaScript
- [Prettier](https://prettier.io) for code formatting
