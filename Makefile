all: test 


test: build
	dotnet test ./api.test/api.test.csproj


cover: build
#	dotnet test /p:CollectCoverage=true /p:Include="[Mutant*]*" /p:Exclude="[Test.Mutant*]*"  ./Test.Mutant/Test.Mutant.csproj
	dotnet test /p:CollectCoverage=true /p:Include="[api*]*"  ./api.test/api.test.csproj

build: 
	dotnet build


clean:
	dotnet clean
