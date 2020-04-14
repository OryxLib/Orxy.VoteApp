publishdir = '/app/voteApp'
projcetdir = '/app/'
echo 'install git '

yum install git

echo 'install nginx'

yum install nginx

echo 'dotnet core'

sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc
sudo sh -c 'echo -e "[packages-microsoft-com-prod]\nname=packages-microsoft-com-prod \nbaseurl= https://packages.microsoft.com/yumrepos/microsoft-rhel7.3-prod\nenabled=1\ngpgcheck=1\ngpgkey=https://packages.microsoft.com/keys/microsoft.asc" > /etc/yum.repos.d/dotnetdev.repo'

sudo yum update 
sudo yum install libunwind libicu
sudo yum install  dotnet-sdk-2.1.4

echo 'install supervisor'

yum install supervisor

echo '[program:voteApp]
command=/usr/bin/dotnet /app/voteApp/Oryx.VoteApp.Server.dll
directory=/app/voteApp/
autostart=true
autorestart=true
stderr_logfile=/var/log/VoteApp.err.log
stdout_logfile=/var/log/VoteApp.out.log
environment=HOME=/var/www/,ASPNETCORE_ENVIRONMENT=Production
user=root
stopsignal=INT
stopasgroup=true
killasgroup=true' > /etc/supervisord.d/voteApp.ini

echo '# supervisord service for systemd (CentOS 7.0+)
# by ET-CS (https://github.com/ET-CS)
[Unit]
Description=Supervisor daemon

[Service]
Type=forking
ExecStart=/usr/bin/supervisord -c /etc/supervisord.conf
ExecStop=/usr/bin/supervisorctl $OPTIONS shutdown
ExecReload=/usr/bin/supervisorctl $OPTIONS reload
KillMode=process
Restart=on-failure
RestartSec=42s' > /usr/lib/systemd/system/supervisord.service

echo 'start supervisord.service'
systemctl start supervisord.service 

supervisorctl reload

echo 'start voteApp'
supervisorctl start voteApp

echo 'install mysql '
yum install mysql
# yum install mysql-server
# yum install mysql-devel

yum install mariadb-server mariadb 
systemctl start mariadb

set password for 'root'@'localhost' =password('Linengneng123#');

echo '提升权限'
grant all privileges on *.* to root@'%'identified by 'Linengneng123#' ;
grant all privileges on *.* to root@'localhost' identified by 'Linengneng123#' ;
grant all privileges on *.* to root@'127.0.0.1' identified by 'Linengneng123#' ;

echo 'set character'
SET  character_set_database =utf8;
SET  character_set_server =utf8;


echo 'modify mysql config '
sed -i 's/^open_files_limit.*/open_files_limit = 1024/' /etc/my.cnf
sed -i 's/^max_connections.*/max_connections = 5000/' /etc/my.cnf

echo 'write nginx conf'

echo 'map $http_upgrade $connection_upgrade{
        default upgrade;
        '' close;
}

upstream backend {
        server 127.0.0.1:5000;
}

server {
        listen 80 default_server;
        listen [::]:80 default_server;
        server_name hb.voteapp.oryxl.com;
 	default_type application/octet-stream;
        
        location / {
		proxy_pass http://backend;
		proxy_pass_request_headers on; 
                proxy_store off;
                proxy_redirect  off;
                proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
                proxy_set_header X-Real-IP $remote_addr;
                proxy_set_header Host $http_host;	 
		proxy_http_version 1.1;
		proxy_set_header Upgrade $http_upgrade;
		proxy_set_header Connection $http_connection; 
        }
}' >  /etc/nginx/conf.d/hb.voteapp.conf


echo '#user  nobody; 

worker_processes 4;
worker_cpu_affinity 0001 0010 0100 1000;
worker_rlimit_nofile 65535;

#error_log  logs/error.log;
#error_log  logs/error.log  notice;
#error_log  logs/error.log  info;

#pid        logs/nginx.pid;


events { 
	use epoll;
        worker_connections 65535;
        accept_mutex off;
        multi_accept off;
}


http {
    include       mime.types;
    include /etc/nginx/conf.d/*.conf;
    default_type  application/octet-stream;
           

    #log_format  main  '$remote_addr - $remote_user [$time_local] "$request" '
    #                  '$status $body_bytes_sent "$http_referer" '
    #                  '"$http_user_agent" "$http_x_forwarded_for"';

    #access_log  logs/access.log  main;

    sendfile        on;
    #tcp_nopush     on;
      tcp_nopush on;
    tcp_nodelay on;

    #keepalive_timeout  0;
    keepalive_timeout  65;

    #gzip  on;

    # server {
    #     listen       80;
    #     server_name  localhost;

    #     #charset koi8-r;

    #     #access_log  logs/host.access.log  main;

    #     location / {
    #         root   html;
    #         index  index.html index.htm;
    #     }

    #     #error_page  404              /404.html;

    #     # redirect server error pages to the static page /50x.html
    #     #
    #     error_page   500 502 503 504  /50x.html;
    #     location = /50x.html {
    #         root   html;
    #     }

    #     # proxy the PHP scripts to Apache listening on 127.0.0.1:80
    #     #
    #     #location ~ \.php$ {
    #     #    proxy_pass   http://127.0.0.1;
    #     #}

    #     # pass the PHP scripts to FastCGI server listening on 127.0.0.1:9000
    #     #
    #     #location ~ \.php$ {
    #     #    root           html;
    #     #    fastcgi_pass   127.0.0.1:9000;
    #     #    fastcgi_index  index.php;
    #     #    fastcgi_param  SCRIPT_FILENAME  /scripts$fastcgi_script_name;
    #     #    include        fastcgi_params;
    #     #}
 
    #     #
    #     #location ~ /\.ht {
    #     #    deny  all;
    #     #}
    # }


    # another virtual host using mix of IP-, name-, and port-based configuration
    #
    #server {
    #    listen       8000;
    #    listen       somename:8080;
    #    server_name  somename  alias  another.alias;

    #    location / {
    #        root   html;
    #        index  index.html index.htm;
    #    }
    #}


    # HTTPS server
    #
    #server {
    #    listen       443 ssl;
    #    server_name  localhost;

    #    ssl_certificate      cert.pem;
    #    ssl_certificate_key  cert.key;

    #    ssl_session_cache    shared:SSL:1m;
    #    ssl_session_timeout  5m;

    #    ssl_ciphers  HIGH:!aNULL:!MD5;
    #    ssl_prefer_server_ciphers  on;

    #    location / {
    #        root   html;
    #        index  index.html index.htm;
    #    }
    #}

}
' > /etc/nginx/nginx.conf

echo 'config limits'
sed -i '$ a\speng soft nofile 10240 ' /etc/security/limits.conf
sed -i '$ a\speng hard nofile 10240 ' /etc/security/limits.conf

echo 'config sysctl.conf'
echo '# sysctl settings are defined through files in
# /usr/lib/sysctl.d/, /run/sysctl.d/, and /etc/sysctl.d/.
#
# Vendors settings live in /usr/lib/sysctl.d/.
# To override a whole file, create a new file with the same in
# /etc/sysctl.d/ and put new settings there. To override
# only specific settings, add a file with a lexically later
# name in /etc/sysctl.d/ and put new settings there.
#
# For more information, see sysctl.conf(5) and sysctl.d(5).
net.ipv6.conf.all.disable_ipv6 = 1
net.ipv6.conf.default.disable_ipv6 = 1
net.ipv6.conf.lo.disable_ipv6 = 1

vm.swappiness = 0
net.ipv4.neigh.default.gc_stale_time=120
net.ipv4.ip_local_port_range = 1024 65000

# see details in https://help.aliyun.com/knowledge_detail/39428.html
net.ipv4.conf.all.rp_filter=0
net.ipv4.conf.default.rp_filter=0
net.ipv4.conf.default.arp_announce = 2
net.ipv4.conf.lo.arp_announce=2
net.ipv4.conf.all.arp_announce=2


# see details in https://help.aliyun.com/knowledge_detail/41334.html
net.ipv4.tcp_max_tw_buckets = 5000
net.ipv4.tcp_syncookies = 1
net.ipv4.tcp_synack_retries = 2

 
net.ipv4.tcp_max_orphans = 262144
net.ipv4.tcp_max_syn_backlog = 262144
net.ipv4.tcp_timestamps = 0
net.ipv4.tcp_synack_retries = 1
net.ipv4.tcp_syn_retries = 1' > /etc/sysctl.conf

# sed -i 's/^worker_processes.*/worker_processes 1;/' /etc/nginx/nginx.conf

# dotnet publish /app/Oryx.VoteWeapp/Oryx.VoteApp.Server/Oryx.VoteApp.Server/ -c release -o /app/voteApp