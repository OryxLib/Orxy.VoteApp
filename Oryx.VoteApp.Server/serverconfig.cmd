REM linux 虚拟内存
设置步骤
这里采用设置交换文件的方法，执行以下命令

cd /var
sudo swapoff /var/swapfile
sudo dd if=/dev/zero of=swapfile bs=1M count=4096
sudo mkswap /var/swapfile
sudo swapon /var/swapfile
 
其中1024表示2048MB的虚拟内存，可以根据实际情况设置，一般为物理内存的两倍即可

最后，修改/etc/fstab，添加一行

/var/swapfile none swap sw 0 0

REM install Git
yum install git

REM config nginx

sudo yum install nginx

REM config dotnet core 
sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc
sudo sh -c 'echo -e "[packages-microsoft-com-prod]\nname=packages-microsoft-com-prod \nbaseurl= https://packages.microsoft.com/yumrepos/microsoft-rhel7.3-prod\nenabled=1\ngpgcheck=1\ngpgkey=https://packages.microsoft.com/keys/microsoft.asc" > /etc/yum.repos.d/dotnetdev.repo'

sudo yum update
sudo yum install libunwind libicu
sudo yum install dotnet-sdk-2.1.4

yum install icu
REM install yum install supervisor

yum install supervisor
cd /etc/supervisord.d/

 supervisord.d/*.ini



REM Config : 
[program:voteApp]
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
killasgroup=true
 
REM Config : 
/usr/lib/systemd/system/supervisord.service
# supervisord service for systemd (CentOS 7.0+)
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
RestartSec=42s

[Install]
WantedBy=multi-user.target 

systemctl stop supervisord.service

systemctl start supervisord.service

supervisorctl reload

supervisorctl start voteApp

supervisorctl restart voteApp

 sudo chmod a+x /app/voteApp/Oryx.VoteApp.Server.dll

 
REM config mysql
2
yum install mysql
yum install mysql-server
yum install mysql-devel

 
yum install mariadb-server mariadb 
systemctl start mariadb

REM option 2 :
REM wget http://dev.mysql.com/get/mysql-community-release-el7-5.noarch.rpm
REM rpm -ivh mysql-community-release-el7-5.noarch.rpm
REM yum install mysql-community-server


set password for 'root'@'localhost' =password('Linengneng123#');

--ALTER USER 'root'@'localhost' IDENTIFIED BY 'Linengneng123#';

rem 提升权限
grant all privileges on *.* to root@'%'identified by 'Linengneng123#' ;

SHOW VARIABLES LIKE 'character_set_%';

SET  character_set_database =utf8;
SET  character_set_server =utf8;

REM Remove mysql

yum remove mysql
yum remove mysql-server
yum remove mysql-devel

 
yum remove mariadb-server mariadb 

mv /var/lib/mysql /var/lib/mysql_old_backup

GRANT ALL PRIVILEGES ON *.* TO 'root'@'%' IDENTIFIED BY 'Linengneng123#' WITH GRANT OPTION;
 FLUSH PRIVILEGES;

Create user root IDENTIFIED BY 'Linengneng123#';


REM package - installed

REM MySql.Data.EntityFrameworkCore
REM MySql.Data.EntityFrameworkCore.Design