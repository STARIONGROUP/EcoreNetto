<img src="https://github.com/STARIONGROUP/EcoreNetto/raw/development/Ecorenetto-Logo-text.png" width="350">

## Introduction

ECoreNetto is a suite of dotnet core libraries that are used to deserialize an Ecore meta-model for the purpose of code generation. Ecore is a meta-model used to represent models in the Eclipse Modelling Framework. EMF is a powerful framework and code generation facility for building Java applications based on simple model definitions. The intention of ECoreNetto is not to be a port of EMF, it aims at bridging the gap to the .NET world to facilitate code generation of C# class libraries based on an Ecore model using the .NET code available tooling and libraries.

## ECoreNetto.Extensions

The **ECoreNetto.Extensions** library provides extensions methods to the EcoreNetto library to support code generation. This library is part of the EcoreNetto ecosystem.

## ECoreNetto.HandleBars

The **ECoreNetto.HandleBars** library provides [HandleBars](https://github.com/Handlebars-Net/Handlebars.Net) helpers to support code generation. This library is part of the EcoreNetto ecosystem.

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

  - ECoreNetto: [![NuGet Badge](https://buildstats.info/nuget/ECoreNetto)](https://buildstats.info/nuget/ECoreNetto)
  - ECoreNetto.Extensions: [![NuGet Badge](https://buildstats.info/nuget/ECoreNetto.Extensions)](https://buildstats.info/nuget/ECoreNetto.Extensions)
  - ECoreNetto.HandleBars: [![NuGet Badge](https://buildstats.info/nuget/ECoreNetto.HandleBars)](https://buildstats.info/nuget/ECoreNetto.HandleBars)

## Build Status

GitHub actions are used to build and test the EcoreNetto libraries

Branch | Build Status
------- | :------------
Master | ![Build Status](https://github.com/STARIONGROUP/EcoreNetto/actions/workflows/CodeQuality.yml/badge.svg?branch=master)
Development | ![Build Status](https://github.com/STARIONGROUP/EcoreNetto/actions/workflows/CodeQuality.yml/badge.svg?branch=development)

## Ecore Documentation

http://download.eclipse.org/modeling/emf/emf/javadoc/2.10.0/index.html?org/eclipse/emf/ecore/EObject.html