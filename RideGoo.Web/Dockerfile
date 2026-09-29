FROM node:20-alpine AS build
WORKDIR /app

COPY RideGoo.Web/package*.json ./
RUN npm install

COPY RideGoo.Web/ ./
RUN npm run build

FROM nginx:alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html
COPY RideGoo.Web/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80