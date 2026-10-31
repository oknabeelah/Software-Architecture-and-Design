27/10/2026

**Part 1: Verify the starter project**
- [ ] Inspect and build the six project scaffold
```bash
Admin@DESKTOP-5EETDPC MINGW64 ~/Documents/My Obsidian Vault/00.04.Software-Architecture-and-Design/PgrStudentManagement-week06-starter (week-6)
$ dotnet sln list
Project(s)
----------
PgrStudentManagement.Adapters.API\PgrStudentManagement.Adapters.API.csproj
PgrStudentManagement.Adapters.Persistence\PgrStudentManagement.Adapters.Persistence.csproj
PgrStudentManagement.Adapters.Web\PgrStudentManagement.Adapters.Web.csproj
PgrStudentManagement.Application\PgrStudentManagement.Application.csproj
PgrStudentManagement.Business\PgrStudentManagement.Business.csproj
PgrStudentManagement.Domain\PgrStudentManagement.Domain.csproj
PgrStudentManagement.Tests\PgrStudentManagement.Tests.csproj
```

order of implementing projects
```shell
PgrStudentManagement.Domain  
PgrStudentManagement.Business  
PgrStudentManagement.Adapters.Persistence  
PgrStudentManagement.Adapters.Web  
PgrStudentManagement.Adapters.API  
PgrStudentManagement.Application  
PgrStudentManagement.Tests  
```

