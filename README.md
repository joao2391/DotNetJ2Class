> ⚠️ THIS LIB IS UNDER MAINTENANCE

# DotNet.J2Class

[![NuGet](https://img.shields.io/nuget/v/DotNetJ2Class)](https://www.nuget.org/packages/DotNetJ2Class/)

DotNet.J2Class generates plain .NET POCO types at runtime from JSON payloads using Reflection.Emit.
It's useful for quickly mapping unknown or evolving JSON schemas into runtime-accessible objects without maintaining static DTOs.

Quick highlights
- Runtime type generation using `TypeBuilder`/`PropertyBuilder` and `ILGenerator`.
- Supports nested objects and arrays (primitive and object arrays are handled).
- Caches generated types by schema signature to avoid re-emitting identical types.

# Getting started

Install (NuGet):

```bash
Install-Package DotNetJ2Class
```

# Basic usage

```csharp
using DotNet.J2Class;

string simple = "{'Foo':'bar'}";
var obj = J2Class.CreateObjectFromJson(simple, "MyClass");
dynamic dyn = obj;
Console.WriteLine(dyn.Foo); // -> bar

string complex = "{'Person': {'Name':'Alice','Age':30}, 'Tags': ['x','y']}";
var obj2 = J2Class.CreateObjectFromComplexJson(complex, "RootClass");
dynamic d2 = obj2;
Console.WriteLine(d2.Person.Name); // -> Alice
```

# Commands

```bash
dotnet build
dotnet test
dotnet run --project test/ConsoleApp1
```

Important notes / gotchas
- The project targets `net9.0` (see `src/DotNet.J2Class/DotNet.J2Class.csproj`).
- Parsing: inputs using single quotes are normalized internally, but prefer valid JSON (double quotes) for robustness.
- Arrays: homogeneous primitive arrays are materialized as typed `List<T>` (e.g. `List<string>`); mixed or empty arrays fall back to `List<object>`.
- Exceptions: `CreateObjectFromJson` wraps failures in `InvalidOperationException` to surface errors; `CreateObjectFromComplexJson` preserves thrown exceptions for easier debugging in some paths.
- Caching: generated types are cached by a schema signature — this improves performance but increases runtime memory usage for many distinct schemas.

# Testing

- Tests live in `test/DotNet.J2Class.Tests` and use NUnit. They cover simple, complex, array, concurrency and basic performance scenarios.

# Contributing

- PRs welcome. For larger changes open an issue first.
- When changing parsing or type generation, add tests in `test/DotNet.J2Class.Tests` demonstrating expected behaviors.

# License

[MIT](https://choosealicense.com/licenses/mit/)
