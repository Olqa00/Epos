# Create main solution
```bash
dotnet new sln --name Epos
```
# Create directories
```bash
mkdir src
mkdir tests
```
# In src directory, create solutions
```bash
dotnet new webapi --no-https --output WebApi --name Epos.WebApi
dotnet new classlib --output Domain --name Epos.Domain
dotnet new classlib --output Infrastructure --name Epos.Infrastructure
dotnet new classlib --output Application --name Epos.Application
```
# Add solutions to main solution
```bash
dotnet sln add src/WebApi
dotnet sln add src/Application
dotnet sln add src/Domain
dotnet sln add src/Infrastructure
```
# Create references betweeen solutions
```bash
dotnet add .\src\WebApi\Epos.WebApi.csproj reference .\src\Infrastructure\Epos.Infrastructure.csproj
dotnet add .\src\Infrastructure\Epos.Infrastructure.csproj reference .\src\Application\Epos.Application.csproj
dotnet add .\src\Application\Epos.Application.csproj reference .\src\Domain\Epos.Domain.csproj
```
# Create test solutions
```bash
dotnet new mstest --output WebApi.UnitTests --name Epos.WebApi.UnitTests
dotnet new mstest --output Infrastructure.UnitTests --name Epos.Infrastructure.UnitTests
dotnet new mstest --output Application.UnitTests --name Epos.Application.UnitTests
dotnet new mstest --output Domain.UnitTests --name Epos.Domain.UnitTests
```
# Create references betweeen test solutions
```bash
dotnet add .\WebApi.UnitTests\ reference ..\src\WebApi\
dotnet add .\Infrastructure.UnitTests\ reference ..\src\Infrastructure\
dotnet add .\Application.UnitTests\ reference ..\src\Application\
dotnet add .\Domain.UnitTests\ reference ..\src\Domain\
```
# Add solutions to test solution
```bash
dotnet sln add ../src/WebApi
dotnet sln add ../src/Application
dotnet sln add ../src/Domain
dotnet sln add ../src/Infrastructure
dotnet sln add ../src/WebApi.UnitTests
dotnet sln add ../src/Application.UnitTests
dotnet sln add ../src/Domain.UnitTests
dotnet sln add ../src/Infrastructure.UnitTests
```
# Create new branch
```bash
git branch develop
```
# Add gitignore
```bash
dotnet new gitignore
```