![EcoreNetto](https://raw.githubusercontent.com/STARIONGROUP/EcoreNetto/development/Ecorenetto-Logo-text.png)

## Introduction

ECoreNetto is a suite of dotnet core libraries and tools that are used to deserialize an Ecore meta-model for the purpose of code generation. Ecore is a meta-model used to represent models in the Eclipse Modelling Framework. EMF is a powerful framework and code generation facility for building Java applications based on simple model definitions. The intention of ECoreNetto is not to be a port of EMF, it aims at bridging the gap to the .NET world to facilitate code generation of C# class libraries based on an Ecore model using the .NET code available tooling and libraries.

## ECoreNetto

The core library used to deserialize an ecore file and create an in memory ECore model object graph.

## ECoreNetto.Extensions

The **ECoreNetto.Extensions** library provides extensions methods to the EcoreNetto library to support code generation. This library is part of the EcoreNetto ecosystem.

## ECoreNetto.HandleBars

The **ECoreNetto.HandleBars** library provides [HandleBars](https://github.com/Handlebars-Net/Handlebars.Net) helpers to support code generation. This library is part of the EcoreNetto ecosystem.

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

http://download.eclipse.org/modeling/emf/emf/javadoc/2.10.0/index.html?org/eclipse/emf/ecore/EObject.html

# License

The EcoreNetto libraries are provided to the community under the Apache License 2.0.

# Contributions

Contributions to the code-base are welcome. However, before we can accept your contributions we ask any contributor to sign the Contributor License Agreement (CLA) and send this digitaly signed to s.gerene@stariongroup.eu. You can find the CLA's in the CLA folder.

[Contribution guidelines for this project](.github/CONTRIBUTING.md)