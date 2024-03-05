This is my attempt at the 1 billion row challenge. I am doing this just for fun and maybe to learn a few things.
I'm going to start with a C# implementation since that's what I am most familiar with.
I will add other implementations later.

Here's how to build the solution:

dotnet publish --self-contained -f net6.0 -r linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true /p:UseAppHost=true /p:IncludeNativeLibrariesForSelfExtract=true -c Release

dotnet publish --self-contained -f net8.0 -r linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true /p:UseAppHost=true /p:IncludeNativeLibrariesForSelfExtract=true -c Release

Why Dotnet 6 when Dotnet 8 is avaliable? Because I want to see the performance difference between them.

All timing will be done on WSL Ubuntu 22.04
