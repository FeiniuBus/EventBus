# EventBus
## Sample
### Step 1 : Configure EventBus Options
*** Add following code after `services.AddDbContext` in `StartUp.cs` ***
```
services.AddEventBus(options =>
{
  // using EntityFramework
  options.UseEntityframework<**Your DbContext Type**>();
  
  // OR using Ado.NET 
  options.UseMySQL(**Database connection string**)
  
  // using RabbitMQ
  options.UseRabbitMQ(rabbit =>
  {
    rabbit.HostName = "**Your hostname of RabbitMQ**";
    rabbit.UserName = "**Your username if needed**";
    rabbit.Password = "**Your password if needed**";
  });
 });
 ```
