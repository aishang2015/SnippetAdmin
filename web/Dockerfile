FROM node:14 AS builder
WORKDIR /app
COPY . /app
RUN mkdir -p /app/node_modules
RUN npm config set registry https://registry.npm.taobao.org
RUN npm install
RUN npm run build
FROM nginx
COPY --from=builder /app/build/ /usr/share/nginx/html/
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]