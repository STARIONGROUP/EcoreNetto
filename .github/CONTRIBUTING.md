# How to contribute

We would like to start with saying thank you for wanting to contribute to EcoreNetto codebase. We want to keep it as easy as possible to contribute changes that get things working in your environment. There are a few guidelines that we need contributors to follow so that we have a chance of keeping on top of things.

- [Making Changes](#making-changes)
  - [Handling Updates from Upstream/Development](#handling-updates-from-upstreamdevelopment)
  - [Sending a Pull Request](#sending-a-pull-request)
- [Building and Testing](#building-and-testing)
  - [Prerequisites](#prerequisites)
  - [Building](#building)
  - [Running the Tests](#running-the-tests)
  - [Collecting Code Coverage Locally](#collecting-code-coverage-locally)
  - [Code-Quality Gate](#code-quality-gate)
- [Style Guidelines](#style-guidelines)

## Making Changes

1. [Fork](http://help.github.com/forking/) on GitHub
1. Clone your fork locally
1. Configure the upstream repo (`git remote add upstream git://github.com/STARIONGROUP/EcoreNetto`)
1. Checkout development
1. Create a local branch (`git checkout -b myBranch`) from development
1. Work on your feature
1. Rebase if required (see below)
1. Push the branch up to GitHub (`git push origin myBranch`)
1. Send a Pull Request on GitHub

You should **never** work on a clone of master or development, and you should **never** send a pull request from master or development - always from a branch. The reasons for this are detailed below.

### Handling Updates from Upstream/Development

While you're working away in your branch it's quite possible that your upstream development (most likely the canonical EcoreNetto version) may be updated. If this happens you should:

1. [Stash](http://git-scm.com/book/en/Git-Tools-Stashing) any un-committed changes you need to
1. `git checkout development`
1. `git pull upstream development`
1. `git checkout myBranch`
1. `git rebase development myBranch`
1. `git push origin development` - (optional) this makes sure your remote development is up to date

This ensures that your history is "clean" i.e. you have one branch off from development followed by your changes in a straight line. Failing to do this ends up with several "messy" merges in your history, which we don't want. This is the reason why you should always work in a branch and you should never be working in, or sending pull requests from, development.

If you're working on a long running feature then you may want to do this quite often, rather than run the risk of potential merge issues further down the line.

### Sending a Pull Request

While working on your feature you may well create several branches, which is fine, but before you send a pull request you should ensure that you have rebased back to a single "Feature branch". We care about your commits, and we care about your feature branch; but we don't care about how many or which branches you created while you were working on it :smile:.

When you're ready to go you should confirm that you are up to date and rebased with upstream/development (see "Handling Updates from Upstream/development" above), and then:

1. `git push origin myBranch`
1. Send a descriptive [Pull Request](https://help.github.com/articles/creating-a-pull-request/) on GitHub - making sure you have selected the correct branch in the GitHub UI!
1. Wait for @samatstarion to merge your changes in.

And remember; **A pull-request with tests is a pull-request that's likely to be pulled in.** :grin: Bonus points if you document your feature in our [wiki](https://github.com/STARIONGROUP/EcoreNetto/wiki) once it has been pulled in

## Building and Testing

Before you send a pull request, please build the solution and run the tests locally. The same build and tests run automatically on every push and pull request (see [Code-Quality Gate](#code-quality-gate) below), so validating locally first is the quickest way to keep your pull request green.

All commands below are run from the repository root and assume the solution file `EcoreNetto.sln`.

### Prerequisites

- The **.NET 10 SDK** ([download](https://dotnet.microsoft.com/download/dotnet/10.0)). The CLI (`ECoreNetto.Tools`) and all test projects target `net10.0`; the libraries target `netstandard2.0`.

### Building

```powershell
dotnet restore EcoreNetto.sln
dotnet build EcoreNetto.sln          # add -c Release for a release build
```

### Running the Tests

The tests are written with **NUnit 4** (with **Moq**) and live in the five `*.Tests` projects.

```powershell
# run the whole suite
dotnet test EcoreNetto.sln

# run a single test project
dotnet test ECoreNetto.Tests/ECoreNetto.Tests.csproj

# run a single test (or a subset) by name
dotnet test EcoreNetto.sln --filter "FullyQualifiedName~SomeTestClass.SomeTestMethod"
```

### Collecting Code Coverage Locally

Coverage is collected with **coverlet**. The most convenient option mirrors what CI runs and produces a Cobertura report:

```powershell
dotnet test EcoreNetto.sln --collect:"XPlat Code Coverage"
```

This writes a `coverage.cobertura.xml` file under each test project's `TestResults/<guid>/` folder.

The repository also ships a `coverlet.runsettings` at the root that emits the OpenCover format, which some tools expect:

```powershell
dotnet test EcoreNetto.sln --settings coverlet.runsettings
```

Optionally, turn the coverage data into a browsable HTML report using the same ReportGenerator tool that CI uses:

```powershell
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:CoverageReport -reporttypes:Html
```

Then open `CoverageReport/index.html` in a browser.

### Code-Quality Gate

Every push and pull request triggers the `.github/workflows/CodeQuality.yml` workflow, which:

- builds and runs the full test suite on both Linux and Windows;
- runs **SonarCloud** analysis (project `STARIONGROUP_EcoreNetto`, organisation `stariongroup`) — see the [SonarCloud dashboard](https://sonarcloud.io/project/overview?id=STARIONGROUP_EcoreNetto) and the badges in the [README](../README.md); and
- posts a code-coverage summary as a comment on the pull request.

The SonarCloud check is required and must pass before a pull request can be merged. Please make sure your change keeps the tests green and does not regress code coverage. You do **not** need to run SonarCloud locally — building and running the tests with coverage (as described above) is enough to anticipate the gate.

## Style Guidelines

- Indent with 4 spaces, **not** tabs.
- No underscore (`_`) prefix for member names.
- Use `this` when accessing instance members, e.g. `this.Name = "EcoreNetto";`.
- Use the `var` keyword unless the inferred type is not obvious.
- Use the C# type aliases for types that have them, e.g. `int` instead of `Int32`, `string` instead of `String` etc.
- Use meaningful names (no hungarian notation), we like long descriptive names of methods, variables and parameters.
- Wrap `if`, `else` and `using` blocks (or blocks in general, really) in curly braces, even if it's a single line.
- Put `using` statements inside namespace.
- Pay attention to whitespace and extra blank lines
- Absolutely **no** regions
- Start every `.cs` file with the **short-form** copyright header shown below. Use the SPDX identifier rather than embedding the full Apache License text, keep the `file="..."` attribute equal to the actual file name, and set the copyright range to the current year:

```csharp
// ------------------------------------------------------------------------------------------------
// <copyright file="MyClass.cs" company="Starion Group S.A.">
//
//   Copyright 2017-2026 Starion Group S.A.
//   SPDX-License-Identifier: Apache-2.0
//
// </copyright>
// ------------------------------------------------------------------------------------------------
```

> If you use **ReSharper** or **Rider**, the repository ships an `EcoreNetto.sln.DotSettings` file that is applied automatically when you open the solution. It covers many of the guidelines above — such as `this.` qualification, placing `using` statements inside the namespace, naming rules, and the short-form SPDX copyright file header. There may be some style guidelines which are not covered by the file, so please pay attention to the style of existing code.
>
> If you do not use ReSharper or Rider, note that the repository does not currently provide an `.editorconfig`; please follow the guidelines above manually and match the style of the surrounding code.
