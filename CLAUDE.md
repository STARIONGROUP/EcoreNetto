# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ECoreNetto is a suite of .NET libraries and a CLI tool that deserialize an [Ecore](http://download.eclipse.org/modeling/emf/emf/javadoc/2.10.0/) meta-model (from the Eclipse Modeling Framework) into an in-memory object graph, primarily to drive C# code generation. It is *not* a port of EMF; it bridges Ecore models into the .NET tooling world.

## Build, Test, Run

The solution is `EcoreNetto.sln`. The CLI (`ECoreNetto.Tools`) targets **net10.0**; the libraries target **netstandard2.0**; test projects target **net10.0**. You need the .NET 10 SDK installed.

```powershell
dotnet restore EcoreNetto.sln
dotnet build EcoreNetto.sln                       # add -c Release for release builds
dotnet test EcoreNetto.sln                        # run all tests
dotnet test ECoreNetto.Tests/ECoreNetto.Tests.csproj   # one project
```

Run a single test by name (NUnit):

```powershell
dotnet test EcoreNetto.sln --filter "FullyQualifiedName~SomeTestClass.SomeTestMethod"
```

Run the CLI tool from source:

```powershell
dotnet run --project ECoreNetto.Tools -- html-report -i path/to/model.ecore -o report.html
```

When installed as a global tool the command name is `ecoretools`.

## CLI structure (ECoreNetto.Tools)

`Program.cs` builds a Generic Host with **Autofac** as the DI container and **Serilog** for file logging, then assembles a `System.CommandLine` `RootCommand`. Each report type is a `Command` + nested `Handler` pair:

- Subcommands: `xl-report`, `html-report`, `markdown-report`, `model-inspection` (see `Commands/`).
- Each command derives from `ReportCommand` (shared options: `--input-model`/`-i`, `--output-report`/`-o`, `--auto-open-report`/`-a`, `--no-logo`, `--log-level`).
- Each handler derives from `ReportHandler`, which orchestrates version-check → input validation → extension validation → `IReportGenerator.GenerateReport(...)`, all wrapped in Spectre.Console status UI.
- Report generators are resolved from DI (registered in `Program.CreateHostBuilder`) and live in `ECoreNetto.Reporting`, **not** in the Tools project. Adding a new report means: a generator interface+impl in Reporting, DI registration, a `Command`, and a `Handler`.

## Core library architecture (ECoreNetto)

The object model mirrors the Ecore/EMF metamodel hierarchy under `ModelElement/`. Inheritance chain (base → derived):

```
EObject → EModelElement → ENamedElement → EClassifier → EClass / EDataType / EEnum
                                        → ETypedElement → EStructuralFeature → EAttribute / EReference
                                                        → EOperation, EParameter
                                        → EPackage, EEnumLiteral
```

Loading flow (this is the key cross-file pattern):

1. `ResourceSet.CreateResource(uri)` creates a `Resource` for an `.ecore` file URI.
2. `Resource.Load(...)` runs the internal `ECoreParser.ParseXml()`, which loads the XML, builds the root `EPackage`, and calls `ReadXml` recursively.
3. Deserialization is **two-phase**: first every `EObject.ReadXml` stores raw XML attributes into the `Attributes` dictionary; *then* `ECoreParser` iterates `resource.AllContents()` and calls the abstract `SetProperties()` on each element to resolve typed properties and cross-references. When adding a new model element type, follow this pattern — populate `Attributes` in `ReadXml`, resolve them in `SetProperties`.
4. Cross-resource references (`eType`, `eSuperTypes`, `eOpposite`) are resolved lazily via `Resource.GetEObject(uriFragment)`, which demand-loads other `.ecore` files into the same `ResourceSet`. `EObject.ProcessAttributeValue` rewrites implicit `#//` references to point at the current top package.

Logging throughout uses optional injected `ILoggerFactory`, falling back to `NullLogger` when null — preserve this convention in new classes.

## Reporting & templating (ECoreNetto.Reporting, .HandleBars, .Extensions)

- `ReportGenerator` (abstract) provides `LoadRootPackage(FileInfo)`, which wires up a `ResourceSet`/`Resource` and returns the root `EPackage`.
- `HandleBarsReportGenerator` (abstract, derives from `ReportGenerator`) sets up a Handlebars.Net environment, registers helpers from **ECoreNetto.HandleBars**, and compiles `.hbs` templates embedded as resources (`ECoreNetto.Reporting.Templates.{name}.hbs`, loaded via `ResourceLoader`). `CreateHandlebarsPayload` flattens the package tree into ordered enums/datatypes/classes (`HandlebarsPayload`).
- **ECoreNetto.Extensions** holds the query/convenience extension methods over the model (e.g. `EPackage.QueryPackages()` walks subpackages recursively) used by generators and templates.
- **ECoreNetto.HandleBars** holds the reusable Handlebars helpers (string, structural-feature, generalization, documentation, boolean).

## Test data

Sample models live in `TestData/` (`ecore.ecore`, `recipe.ecore`, `wizardEcore.ecore`) and are linked into test projects' output under `Data/` via `<None>` items with `CopyToOutputDirectory`. Test framework is **NUnit 4** with **Moq**; coverage via coverlet. `InternalsVisibleTo` exposes internals (e.g. `ECoreParser`) to each project's `.Tests` assembly.

## Code coverage requirement

**All new code must reach at least 80% line/branch coverage** — this is enforced by the SonarCloud quality gate on new code. When implementing a feature or fix, add tests that exercise every new branch (happy path *and* guard/error paths) so coverage on the changed lines is ≥ 80%. Verify locally before opening/updating a PR:

```powershell
dotnet test ECoreNetto.Tests/ECoreNetto.Tests.csproj --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

Then inspect the generated Cobertura report (or run ReportGenerator) and confirm the new members are covered. Do not consider an issue "done" until its new code clears the 80% bar.

## Code style (from .github/CONTRIBUTING.md)

These differ from default .NET conventions — match them:

- 4 spaces, no tabs. **No `#region`s.**
- Always use `this.` when accessing instance members.
- **No** `_` prefix on member names.
- `using` directives go **inside** the namespace.
- Use `var` unless the inferred type is non-obvious; use C# type aliases (`int`, `string`).
- Long, descriptive names; no Hungarian notation. Always brace `if`/`else`/`using` blocks even when single-line.
- Every type and member carries an XML-doc comment. Every file starts with the **short-form** SPDX copyright header (`Copyright 2017-{year} Starion Group S.A.` followed by `SPDX-License-Identifier: Apache-2.0`), not the verbose Apache license text. Copy the header from any existing file.

## Git workflow

Default branch for PRs is `development`; active development happens on `development`. Never commit directly to or PR from `master`/`development` — always branch off `development`. A CLA is required for external contributions.
