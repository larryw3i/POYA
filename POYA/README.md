**Hello, here is WSUDisk, I'm Larry Wei, Welcome you**  
you can storage file in WSUDisk, and publish article, chat with friends, and so on. . .  
if you need use source code DO IT AFTER CLONE IT TO LOCAL >_  
#### 0  
add appsettings.json file in root directory  
it is like:  
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
    "password": "xxxxxx"
  }, 
  "ErrorLogHandle": {
    "ReceiveLogEmailAddress": "xxxxxxx@xxx.xxx"
  }
} 
```   
The DefaultConnection is following if you use MSSQL  
**"Server=(localdb)\\\\mssqllocaldb;Database=aspnet-POYA-0E28E843-176D-49F3-9739-6D5E6F1BC3F5;Trusted_Connection=True;MultipleActiveResultSets=true"**  
??HOW TO FIND MY MSSQL ConnectionStrings?? >_  
*Open your **Visual Studio**  
*Find your **SQL Server Object Explorer** and open it    
*You'll find it after you right click your database and click **Properties**  
 