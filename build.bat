mkdir packages
call %1\NuGetRestorePackagesOnly.bat VersionOne.ServiceHost.Core.csproj %1
call %1\NuGetUpdatePackages.bat packages.config 
msbuild VersionOne.ServiceHost.Core.csproj ^
/p:V1BuildToolsPath=%1 ^
/p:NuGetExePath=%1\NuGet.exe ^
/p:RequireRestoreConsent=false ^
/p:Configuration=Release ^
/p:Platform=AnyCPU ^
/p:Major=1 ^
/p:Minor=0 ^
/p:Revision=1 ^
/p:AssemblyInformationalVersion="See https://github.com/versionone/VersionOne.ServiceHost.Core/wiki" ^
/p:AssemblyCopyright="Copyright 2008-2013, VersionOne, Inc. Licensed under modified BSD." ^
/p:CompanyName="VersionOne, Inc" ^
/p:AssemblyProduct="VersionOne.ServiceHost.Core" ^
/p:AssemblyTitle="VersionOne ServiceHost Core" ^
/p:AssemblyDescription="VersionOne ServiceHost Core Release Build" ^
/p:SignAssembly=%SIGN_ASSEMBLY% ^
/p:AssemblyOriginatorKeyFile=%SIGNING_KEY% 