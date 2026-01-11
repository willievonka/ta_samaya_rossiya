FROM node:24.11.1-alpine3.22 AS build

WORKDIR /app

COPY package*.json ./
RUN npm install -g @angular/cli
RUN npm install

COPY . .
RUN ng build --configuration production

FROM alpine:3.22.2

RUN apk add --no-cache nginx

COPY --from=build /app/dist/ta_samaya_rossiya /usr/share/nginx/html

RUN chmod -R 755 /usr/share/nginx/html

COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
