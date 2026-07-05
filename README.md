![EcoreNetto](https://raw.githubusercontent.com/STARIONGROUP/EcoreNetto/development/Ecorenetto-Logo-text.png)

## Introduction

ECoreNetto is a suite of dotnet core libraries and tools that are used to deserialize an Ecore meta-model for the purpose of code generation. Ecore is a meta-model used to represent models in the Eclipse Modelling Framework. EMF is a powerful framework and code generation facility for building Java applications based on simple model definitions. The intention of ECoreNetto is not to be a port of EMF, it aims at bridging the gap to the .NET world to facilitate code generation of C# class libraries based on an Ecore model using the .NET code available tooling and libraries.

## ECoreNetto

The core library used to deserialize an ecore file and create an in memory ECore model object graph.

### Getting Started

The following snippet shows how to load an `.ecore` file with the core **ECoreNetto** library, obtain the root `EPackage` and walk the model. A `ResourceSet` demand-loads the `.ecore` file (and any cross-referenced models) and `Resource.Load(...)` returns the root `EPackage`.

```csharp
using System;
using ECoreNetto;
using ECoreNetto.Resource;

// point the ResourceSet at the .ecore file to deserialize
var uri = new Uri(@"C:\path\to\model.ecore");

var resourceSet = new ResourceSet();
var resource = resourceSet.CreateResource(uri);

// Resource.Load returns the root EPackage of the model
EPackage rootPackage = resource.Load(null);

Console.WriteLine($"package: {rootPackage.Name}");

// iterate over the classifiers (EClass, EDataType, EEnum, ...) in the package
foreach (var classifier in rootPackage.EClassifiers)
{
    Console.WriteLine($"  {classifier.GetType().Name}: {classifier.Name}");

    // for classes, inspect their structural features (attributes and references)
    if (classifier is EClass eClass)
    {
        foreach (var structuralFeature in eClass.EStructuralFeatures)
        {
            Console.WriteLine($"    - {structuralFeature.Name} : {structuralFeature.EType?.Name}");
        }
    }
}
```

## ECoreNetto.Extensions

The **ECoreNetto.Extensions** library provides extensions methods to the EcoreNetto library to support code generation. This library is part of the EcoreNetto ecosystem.

## ECoreNetto.HandleBars

The **ECoreNetto.HandleBars** library provides [HandleBars](https://github.com/Handlebars-Net/Handlebars.Net) helpers to support code generation. This library is part of the EcoreNetto ecosystem.

## ECoreNetto.Reporting

The **ECoreNetto.Reporting** library contains reporting generators. This library is part of the EcoreNetto ecosystem.

## ECoreNetto.Tools

The **ECoreNetto.Tools** commandline application is used to generate reports on the content of the Ecore model. Find the documentation [here](https://github.com/STARIONGROUP/EcoreNetto/wiki/ECoreNetto.Tools).

## Code Quality

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_EcoreNetto&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_EcoreNetto)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_EcoreNetto&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_EcoreNetto)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_EcoreNetto&metric=coverage)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_EcoreNetto)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_EcoreNetto&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_EcoreNetto)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_EcoreNetto&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_EcoreNetto)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_EcoreNetto&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_EcoreNetto)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_EcoreNetto&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_EcoreNetto)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_EcoreNetto&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_EcoreNetto)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_EcoreNetto&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_EcoreNetto)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=STARIONGROUP_EcoreNetto&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=STARIONGROUP_EcoreNetto)

## Installation

The package are available on Nuget at:

  - ECoreNetto: ![NuGet Version](https://img.shields.io/nuget/v/EcoreNetto)
  - ECoreNetto.Extensions: ![NuGet Version](https://img.shields.io/nuget/v/EcoreNetto.Extensions)
  - ECoreNetto.HandleBars: ![NuGet Version](https://img.shields.io/nuget/v/EcoreNetto.HandleBars)
  - ECoreNetto.Tools: ![NuGet Version](https://img.shields.io/nuget/v/EcoreNetto.Tools)

## Build Status

GitHub actions are used to build and test the EcoreNetto libraries

Branch | Build Status
------- | :------------
Master | ![Build Status](https://github.com/STARIONGROUP/EcoreNetto/actions/workflows/CodeQuality.yml/badge.svg?branch=master)
Development | ![Build Status](https://github.com/STARIONGROUP/EcoreNetto/actions/workflows/CodeQuality.yml/badge.svg?branch=development)

## Ecore Documentation

ECoreNetto targets the **Ecore 2.0 metamodel** (namespace `http://www.eclipse.org/emf/2002/Ecore`), the core metamodel of the [Eclipse Modeling Framework (EMF)](https://eclipse.dev/emf/). This metamodel has been stable since EMF 2.0 and is the same one used by current EMF releases. The Java reference implementation is the `org.eclipse.emf.ecore` bundle maintained by the Eclipse Foundation; as of the EMF 2.46.0 distribution (December 2025) the `org.eclipse.emf.ecore` runtime is at version 2.42.0 and `org.eclipse.emf.ecore.xmi` at 2.40.0.

- EMF project &amp; current documentation: https://eclipse.dev/emf/
- Java reference implementation (Ecore source, `org.eclipse.emf.ecore`): https://github.com/eclipse-emf/org.eclipse.emf
- Ecore API reference (EMF Javadoc, v2.10.0): https://download.eclipse.org/modeling/emf/emf/javadoc/2.10.0/index.html?org/eclipse/emf/ecore/EObject.html

## Software Bill of Materials (SBOM)

As part of our commitment to security and transparency, this project includes a Software Bill of Materials (SBOM) in the associated NuGet packages. The SBOM provides a detailed inventory of the components and dependencies included in the package, allowing you to track and verify the software components, their licenses, and versions.

**Why SBOM?**

- **Improved Transparency**: Gain insight into the open-source and third-party components included in this package.
- **Security Assurance**: By providing an SBOM, we enable users to more easily track vulnerabilities associated with the included components.
- **Compliance**: SBOMs help ensure compliance with licensing requirements and make it easier to audit the project's dependencies.

You can find the SBOM in the NuGet package itself, which is automatically generated and embedded during the build process.

# License

The EcoreNetto libraries are provided to the community under the Apache License 2.0.

# Contributions

Contributions to the code-base are welcome. However, before we can accept your contributions we ask any contributor to sign the Contributor License Agreement (CLA) and send this digitaly signed to s.gerene@stariongroup.eu. You can find the CLA's in the CLA folder.

[Contribution guidelines for this project](.github/CONTRIBUTING.md)