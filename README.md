# EventBus
 Branch | Pipeline 
----|----
 master | [![Build status](https://ci.appveyor.com/api/projects/status/42jeqj0h28pdoc3x/branch/master?svg=true)](https://ci.appveyor.com/project/Jamesxql/eventbus/branch/master)[![Build Status](https://travis-ci.org/FeiniuBus/EventBus.svg?branch=master)](https://travis-ci.org/FeiniuBus/EventBus)
 release | [![Build status](https://ci.appveyor.com/api/projects/status/42jeqj0h28pdoc3x/branch/release?svg=true)](https://ci.appveyor.com/project/Jamesxql/eventbus/branch/release)[![Build status](https://ci.appveyor.com/api/projects/status/4bfohsc2n3gfd08i?svg=true)](https://ci.appveyor.com/project/standardcore/eventbus)
 milestone/1.0.0 | [![Build status](https://ci.appveyor.com/api/projects/status/42jeqj0h28pdoc3x/branch/milestone/1.0.0?svg=true)](https://ci.appveyor.com/project/Jamesxql/eventbus/branch/milestone/1.0.0)[![Build Status](https://travis-ci.org/FeiniuBus/EventBus.svg?branch=milestone/1.0.0)](https://travis-ci.org/FeiniuBus/EventBus)

## OverView
EventBus is a .Net Standard library to achieve eventually consistent in distributed architectures system like SOA,MicroService. It is lightweight,easy to use and efficiently.

![](https://raw.githubusercontent.com/FeiniuBus/EventBus/master/EventBus.jpg)


## Getting Started
### Step 1 : Configure EventBus Options
*** Add following code after `services.AddDbContext` in `StartUp.cs` ***
```csharp
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
 *** Add following codes in `Configure` scope of `StartUp.cs` ***
 ```csharp
 app.UseEventBus();
 ```

### Step 2 : Publish Event
* Inject `IEventPublisher` in constructor like `.ctor(IEventPublisher eventPublisher)`
* Begin a transaction
  * ***using EntityFramework***
```csharp

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
  
  ```csharp
  IDbConnection dbConnection; /*Open your database connection.*/
  IDbTransaction dbTransaction = dbConnection.BeginTransaction();
  
  //TODO:Businesses codes
  
  //Publish Event
  await _eventPublisher.PrepareAsync(/*RouteKey*/, /*Content Object*/, /*MetaData Object*/,dbConnection,dbTransaction);
  
  //Commit transaction
  dbTransaction.Commit();
  dbConnection.Close();
  
  //Confirm Published Event.The event message won't publish untill invoked **IEventPublisher.ConfirmAsync()**
  //And you can decide when the event message should be confirmed all by your self.
  await _eventPublisher.ConfirmAsync();
  
  //Or you can just rollback these messages when exception was thrown.
  await _eventPublisher.RollbackAsync();
  ```
  
### step 3 : Dead letter callback handler
 * Declare a callback handler class implemented `IFailureHandler`
 * Register callback handle in `AddEventBus` scope
 ```csharp
 options.UseFailureHandle(failure =>
 {
  failure.RegisterFailureCallback(/*RouteKey*/, /*Type of your deadletter callback handler*/);
 });
 ```
 
 
 
### Step 4 : Consumer callback handler
* Declare a callback handler class implemented `ISubscribeCallbackHandler`
* Register callback handle in `StartUp.cs`
```csharp
services.AddSub(options =>
{
  options.ConsumerClientCount = 1;
  options.DefaultGroup = "/*Default Group Name*/";
  // Use default group
  options.RegisterCallback(/*RouteKey*/, /*Type of your callback handler*/);
  // Use specialized group
  options.RegisterCallback(/*RouteKey*/,/*Group Name*/ /*Type of your callback handler*/);
 });
 ```
 

