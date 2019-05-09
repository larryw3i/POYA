English [中文](https://github.com/linghuchong123/POYA/blob/master/POYA/README/README.zh-CN.md "中文文档")  
**Hello, here is X-DOVE project, I'm Larry Wei**  
**❤ Welcome!! ❤**  
In fact, my definition of this project is vague, but I have implemented the function of file storage and article publishing in the project, and I would like to add many functions in the future. If you like my project and want to contribute your code **>_**   
####  null  

This is a **C#/.NET Core MVC** project   
(I'm a beginner, bored and uninterested? Actually, it's **[easy](https://docs.microsoft.com/en-us/aspnet/?view=aspnetcore-2.2#pivot=core "easy")**)  
**. . .** , There should be a lot of things to write here, I don't write it much😅, I think you must know a lot more than I do
#### 0  

Add the **appsettings.json** file to the root directory of the project after **git clone** it   
The contents of **appsettings.json** are:  
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
**DefaultConnection >_** The connection string of database, the connection string above is MSSQL, the  connection string is **"Data Source=app.db** if you use **SQLite**. You can customize your connection string and modify **services.AddDbContext**  in **Startup.cs** file accordingly  
**EmailSender>_** It is the configuration of the mail service(you need to change its value in order to the project to work properly), it plays a key role in user registration and user receiving notifications
**ReceiveLogEmailAddress** This e-mail address is used to receive error messages    
#### 1(inessential)    
You can try it after your appsettings.json is OK **>_**  
**dotnet build**  
**dotnet run**

 