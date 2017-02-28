@echo on
echo 'start pack'
nuget.exe pack ..\DisconfClient.csproj -Prop Configuration=Release
echo 'start upload'
NuGetPackageUploader .