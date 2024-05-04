FROM node:20-alpine AS base
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN corepack enable

# This Dockerfile is based on the main turbo docs at https://github.com/vercel/turbo/tree/main/examples/with-docker
# and on the main Next.js docs at https://github.com/vercel/next.js/blob/canary/examples/with-docker/Dockerfile

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
RUN turbo prune nextjs --docker

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
RUN pnpm turbo build --filter=nextjs...

FROM base AS runner
WORKDIR /app

ENV NODE_ENV production
# Uncomment the following line in case you want to disable telemetry during runtime.
# ENV NEXT_TELEMETRY_DISABLED 1

# Don't run production as root
RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

#
# CHANGE THIS LINE to change which turbo app is being built by this Dockerfile
#
COPY --from=installer /app/apps/nextjs/public /app/apps/nextjs/public

# Set the correct permission for prerender cache
RUN mkdir .next
RUN chown nextjs:nodejs .next

# Automatically leverage output traces to reduce image size
# https://nextjs.org/docs/advanced-features/output-file-tracing

#
# CHANGE THIS LINE to change which turbo app is being built by this Dockerfile
#
COPY --from=installer --chown=nextjs:nodejs /app/apps/nextjs/.next/standalone ./

#
# CHANGE THIS LINE to change which turbo app is being built by this Dockerfile
#
COPY --from=installer --chown=nextjs:nodejs /app/apps/nextjs/.next/static /app/apps/nextjs/.next/static

USER nextjs

EXPOSE 3000

ENV PORT 3000

# server.js is created by next build from the standalone output
# https://nextjs.org/docs/pages/api-reference/next-config-js/output
#
# CHANGE THIS LINE to change which turbo app is being built by this Dockerfile
#
CMD HOSTNAME="0.0.0.0" node /app/apps/nextjs/server.js
