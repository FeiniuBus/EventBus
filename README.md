# EventBus
## Sample
### Step 1 : Configure EventBus Options
*** Add following code after `services.AddDbContext` in `StartUp.cs` ***
```c
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

### Step 2 : Publish Event
* Inject `IEventPublisher` in constructor like `.ctor(IEventPublisher eventPublisher)`
* Begin a transaction
  * ***using EntityFramework***
```c

using(var transaction = dbContext.Database.BeginTransaction)
{
  //TODO:Businesses codes
  
  //Publish Event
  await _eventPublisher.PrepareAsync(/*RouteKey*/, /*Content Object*/, /*MetaData Object*/);
  
  //Commit transaction
  transaction.Commit();
  
  //Confirm Published Event.The event message won't publish untill invoked **IEventPublisher.ConfirmAsync()**
  //And you can decide when the event message be confirmed all by your self.
  await _eventPublisher.ConfirmAsync();
  
  //Or you can just rollback these messages when exception was thrown.
  await _eventPublisher.RollbackAsync();
}
```
  * ***using Ado.NET***
  
  ```c
  IDbConnection dbConnection; /*Open your database connection.*/
  IDbTransaction dbTransaction = dbConnection.BeginTransaction();
  
  //TODO:Businesses codes
  
  //Publish Event
  await _eventPublisher.PrepareAsync(/*RouteKey*/, /*Content Object*/, /*MetaData Object*/,dbConnection,dbTransaction);
  
  //Commit transaction
  dbTransaction.Commit();
  dbConnection.Close();
  
  //Confirm Published Event.The event message won't publish untill invoked **IEventPublisher.ConfirmAsync()**
  //And you can decide when the event message be confirmed all by your self.
  await _eventPublisher.ConfirmAsync();
  
  //Or you can just rollback these messages when exception was thrown.
  await _eventPublisher.RollbackAsync();
  ```
  
### step 3 : Dead letter callback handler
 * Declare a callback handler class implemented `IFailureHandler`
 * Register callback handle in `AddEventBus` scope
 ```c
 options.UseFailureHandle(failure =>
 {
  failure.RegisterFailureCallback(/*RouteKey*/, /*Type of your deadletter callback handler*/);
 });
 ```
 
 
 
### Step 4 : Consumer callback handler
* Declare a callback handler class implemented `ISubscribeCallbackHandler`
* Register callback handle in `StartUp.cs`
```c
services.AddSub(options =>
{
  options.ConsumerClientCount = 1;
  options.DefaultGroup = "eventbus.testgroup";
  options.RegisterCallback(/*RouteKey*/, /*Type of your callback handler*/);
 });
 ```
 

