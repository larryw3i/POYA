[English](https://github.com/linghuchong123/POYA/blob/master/POYA/README.md "中文文档") 中文    
**我是韦云清, 这里是POYA项目**   
其实我对这个项目没有什么太大的定义, 但是我做了文件储存, 文章发表的功能, 以后可能还会添加很多功能, 如果你喜欢我的项目并且想使用我的源码, 你要做下面的事情在git它以后 >_   

#### 0  
添加 appsettings.json 文件在项目的根目录   
这是它的内容:  
```json
{
  "ConnectionStrings": { 
    "DefaultConnection": "Data Source=app.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "EmailSender": {
    "userName": "xxxxxx@xxxxxx.xxx",
    "host": "xxxxxx.xxxxxx.xxx",
    "password": "xxxxxx",
    "port": "xx"
  }, 
  "ErrorLogHandle": {
    "ReceiveLogEmailAddress": "xxxxxxx@xxx.xxx"
  }
} 
```     
**DefaultConnection** 是数据库的连接字符串, 上面的值是SQLite的连接字符串, 如果你使用MSSQL, 那么它的DefaultConnection是  **"Server=(localdb)\\\\mssqllocaldb;Database=aspnet-POYA-0E28E843-176D-49F3-9739-6D5E6F1BC3F5;Trusted_Connection=True;MultipleActiveResultSets=true"**   , 当然了, 你可以更改成你喜欢的数据库和相应的连接字符串, 然后在  **Startup.cs** 中修改  **services.AddDbContext**  里面的相应内容   
**EmailSender** 是你的邮件服务相关配置, 它在用户注册和用户接受通知中起关键作用 **userName** 是你的  **smtp** 用户名, 其它的三个你熟悉 **smtp** 的话你应该很容易明白它   
**ReceiveLogEmailAddress** 邮箱地址是用来接收错误信息的(未医先治)

