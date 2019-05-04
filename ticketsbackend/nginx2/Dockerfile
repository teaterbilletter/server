FROM nginx:mainline-alpine
RUN rm /etc/nginx/conf.d/*
COPY hello.conf /etc/nginx/conf.d/
COPY Apitester/ /usr/share/nginx/html
CMD ["nginx", "-g", "daemon off;"]
EXPOSE 4000
