This is my attempt at the 1 billion row challenge. I am doing this just for fun and maybe to learn a few things.
I'm going to start with a C# implementation since that's what I am most familiar with.
I will add other implementations later.

Here's how to build the solution:

dotnet publish --self-contained -r linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true /p:UseAppHost=true /p:IncludeNativeLibrariesForSelfExtract=true -c Release -f net6.0

dotnet publish --self-contained -r linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true /p:UseAppHost=true /p:IncludeNativeLibrariesForSelfExtract=true -c Release -f net8.0

Why Dotnet 6 when Dotnet 8 is avaliable? Because I want to see the performance difference between them.

All timing will be done on WSL Ubuntu 22.04 on AMD Ryzen 9 3900X 12 core.

Results:

|        Change                                     | net 6     | net 8     |
|---------------------------------------------------|---------- |-----------|
| base implementation                               | 3m16.860s | 2m22.885s |
| method inlining + StringComparer.Ordinal          | 3m15.854s | 2m19.295s |
| memory-mapped file                                | 3m10.700s | 2m10.776s |
| pointer arithmetic, ReadonlySpan, multithreading  | 0m15.413s | 0m10.323s |
| use ReadOnlySpan<byte> as dictionary key          | 0m12.381s | 0m07.426s |
| custom number parsing                             | 0m04.657s | 0m04.048s |