FROM node:20-alpine AS base
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN corepack enable

# This Dockerfile is based on the main turbo docs at https://github.com/vercel/turbo/tree/main/examples/with-docker

FROM base AS builder
# Check https://github.com/nodejs/docker-node/tree/b4117f9333da4138b03a546ec926ef50a31506c3#nodealpine to understand why libc6-compat might be needed.
RUN apk add --no-cache libc6-compat
RUN apk update

# Set working directory
WORKDIR /app
RUN pnpm install turbo@latest --global
COPY . .

#
# CHANGE THIS LINE to change which turbo app is being built by this Dockerfile
#
RUN turbo prune express --docker

# Add lockfile and package.json's of isolated subworkspace
FROM base AS installer
RUN apk add --no-cache libc6-compat
RUN apk update
WORKDIR /app

# First install dependencies (as they change less often)
COPY .gitignore .gitignore
COPY --from=builder /app/out/json/ .
COPY --from=builder /app/out/pnpm-lock.yaml ./pnpm-lock.yaml
RUN pnpm install

# Build the project and its dependencies
COPY --from=builder /app/out/full/ .
COPY turbo.json turbo.json

# Uncomment and use build args to enable remote caching
# ARG TURBO_TEAM
# ENV TURBO_TEAM=$TURBO_TEAM

# ARG TURBO_TOKEN
# ENV TURBO_TOKEN=$TURBO_TOKEN

#
# CHANGE THIS LINE to change which turbo app is being built by this Dockerfile
#
RUN pnpm turbo build --filter=express...

FROM base AS runner
WORKDIR /app

ENV NODE_ENV production

# Don't run production as root
RUN addgroup --system --gid 1001 expressjs
RUN adduser --system --uid 1001 expressjs

USER expressjs

COPY --from=installer /app .

#
# CHANGE THIS LINE to change which turbo app is being built by this Dockerfile
#
CMD node apps/express/dist/index.js