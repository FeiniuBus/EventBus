dotnet restore ../src/EventBus.Core/EventBus.Core.csproj -v m
dotnet build ../src/EventBus.Core/EventBus.Core.csproj -v m

dotnet restore ../src/EventBus.Publish/EventBus.Publish.csproj -v m
dotnet build ../src/EventBus.Publish/EventBus.Publish.csproj -v m

dotnet restore ../src/EventBus.Subscribe/EventBus.Subscribe.csproj -v m
dotnet build ../src/EventBus.Subscribe/EventBus.Subscribe.csproj -v m