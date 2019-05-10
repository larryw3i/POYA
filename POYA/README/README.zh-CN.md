<a href="../README.md">English</a>   中文    
**我是韦云清, 这里是POYA项目**   
>其实我对这个项目没有什么太大的定义, 但是我做了文件储存, 文章发表的功能, 以后可能还会添加很多功能, 
 如果你喜欢我的项目并且想使用我的源码, 你可以 (<a href="#2">简明扼要</a>) **>_**   
####  null  
>这是一个 **C#/.NET Core MVC** 项目   
(我是新手, 并且我并不喜欢.NET Core MVC? 其实它很 **[简单](https://docs.microsoft.com/en-us/aspnet/?view=aspnetcore-2.2#pivot=core "简单")**)   
这里假设你有很高的 **[C#](https://docs.microsoft.com/en-us/dotnet/csharp/  "C#")** 和   **[dotnet](https://dotnet.microsoft.com/  "dotnet")**  基础(**0基础也是可以的**)  
(我使用的IDE是 **[Visual Studio 2019](https://visualstudio.microsoft.com/  "Visual Studio 2019")**, 如果你喜欢你也可以使用 **[Visual Studio Code](https://code.visualstudio.com/  "Visual Studio Code")** , 或是你实在是骨灰级开发者, 你都可以使用文本编辑器为这个项目做出巨大贡献)  
**. . .** , 还有很多东西, 我就不说了, 反正你肯定比我懂得多很多
#### <span id="null">null</span>  
***  
>git clone它以后, 在项目的根目录重命名 **appsettings.json.txt** 文件为 **appsettings.json**     
这是它的内容:  
```json
{
  "ConnectionStrings": { 
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-POYA-0E28E843-176D-49F3-9739-6D5E6F1BC3F5;Trusted_Connection=True;MultipleActiveResultSets=true"
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
>🧧 **DefaultConnection** 是数据库的连接字符串, 上面的值是MSSQL的连接字符串, 如果你使用SQLite, 那么它的DefaultConnection是  **"Data Source=app.db**   , 当然了, 你可以更改成你喜欢的数据库和相应的连接字符串, 然后在  **Startup.cs** 中修改  **services.AddDbContext**  里面的相应内容   
🧧**EmailSender** 是你的邮件服务相关配置(为了能正常使用, 你可能要更改它的值), 它在用户注册和用户接受通知中起关键作用,  **userName** 是你的  **smtp** 用户名, 其它的三个你熟悉 **smtp** 的话你应该很容易明白它   
🧧**ReceiveLogEmailAddress** 邮箱地址是用来接收错误信息的(未医先治)    
#### 1(多余的)    
***   
>完成 **appsettings.json** 后你可以试试项目能否 **RUN**  
在项目的根目录 **>_**  
**dotnet build**  
**dotnet run**

#### <span id="2">2(直截了当)</span>
```powershell    
#这是powershell命令
cd yourdir
git clone https://github.com/linghuchong123/POYA.git
cd POYA
cd POYA
rename appsettings.json.txt appsettings.json
#更改 appsettings.json
dotnet build
dotnet run
```
  

#### . . .  


 ####  ∞
***      
>Copyright (c) 2019 Larry Wei   
Licensed under the [MIT](https://github.com/linghuchong123/POYA/blob/master/LICENSE "MIT License") License