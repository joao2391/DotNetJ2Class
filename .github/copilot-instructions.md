# Copilot / AI Agent Instructions for DotNet.J2Class

Purpose: quickly orient an AI coding agent to be productive in this repository.

- **Big picture**: This is a small .NET library that generates runtime POCO types from JSON using Reflection.Emit. The core behavior lives in [src/DotNet.J2Class/J2Class.cs](src/DotNet.J2Class/J2Class.cs). JSON parsing helpers are in [src/DotNet.J2Class/StringNormalize.cs](src/DotNet.J2Class/StringNormalize.cs). Default names are in [src/DotNet.J2Class/Constants.cs](src/DotNet.J2Class/Constants.cs).

- **How it works (quick)**:
  - Input JSON is normalized into key/value maps by `StringNormalize` and then passed to `J2Class.CompileResultType`/`CompileResultTypeForComplexJson`.
  - Types are created with `AssemblyBuilder.DefineDynamicAssembly`, `ModuleBuilder`, `TypeBuilder` and properties are emitted with `FieldBuilder` + `PropertyBuilder` + `ILGenerator` (see `CreateProperty`).
  - Nested objects become nested generated types (see `GetPropertyType` logic) and lists become `List<T>` of generated or primitive types.

- **Important implementation notes / gotchas**:
  - `StringNormalize.ReturnKeyValueFromJson` uses a very naive string-splitting approach (not a full JSON serializer). Be careful: edge cases and whitespace/quotes may break parsing. Prefer using `Newtonsoft.Json` if changing parsing behavior.
  - `CreateObjectFromJson` wraps failures in an `InvalidOperationException`. `CreateObjectFromComplexJson` rethrows the original exception—tests expect current behavior.
  - The project targets `net9.0` ([src/DotNet.J2Class/DotNet.J2Class.csproj](src/DotNet.J2Class/DotNet.J2Class.csproj)). Tests use NUnit in [test/DotNet.J2Class.Tests](test/DotNet.J2Class.Tests).

- **Common tasks & commands**:
  - Build library: `dotnet build` from repo root or `dotnet build src/DotNet.J2Class`.
  - Run tests: `dotnet test` from repo root or `dotnet test test/DotNet.J2Class.Tests`.
  - Run sample console app (if needed): `dotnet run --project test/ConsoleApp1`.

- **Testing & patterns**:
  - Tests create JSON constants (see [test/DotNet.J2Class.Tests/J2ClassTests.cs](test/DotNet.J2Class.Tests/J2ClassTests.cs)) and call `J2Class.CreateObjectFromJson(...)` or `CreateObjectFromComplexJson(...)`, then assert the generated type has expected properties using `GetProperty`.
  - When adding tests, follow the same pattern: inline JSON consts, create object, reflect against properties.

- **Dependencies & integration points**:
  - Uses `Newtonsoft.Json` for JObject parsing in complex scenarios and `System.Reflection.Emit` for runtime type creation. Both are referenced in the project file.
  - NuGet metadata and packaging info live in the library csproj.

- **Developer conventions**:
  - Keep changes minimal and focused for library code; public surface area is the static `J2Class` API.
  - If you alter parsing logic, update tests in `test/DotNet.J2Class.Tests` demonstrating both simple and nested JSON inputs.

- **Examples to consult**:
  - Runtime type creation example: [src/DotNet.J2Class/J2Class.cs](src/DotNet.J2Class/J2Class.cs) — inspect `GetPropertyType`, `CreateProperty`, and `CompileResultType`.
  - Naive parser to replace / improve: [src/DotNet.J2Class/StringNormalize.cs](src/DotNet.J2Class/StringNormalize.cs).
  - Unit tests and expected behaviors: [test/DotNet.J2Class.Tests/J2ClassTests.cs](test/DotNet.J2Class.Tests/J2ClassTests.cs).

If anything here is unclear or you want a different emphasis (for example, more packaging/publishing guidance or a checklist for PR reviewers), tell me which section to expand or correct.
