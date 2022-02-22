# ECoreNetto

## Introduction

ECoreNetto is a dotnet core library that is used to deserialize an Ecore meta-model for the purpose of code generation. Ecore is a meta-model used to represent models in the Eclipse Modelling Framework. EMF is a powerful framework and code generation facility for building Java applications based on simple model definitions. The intention of ECoreNetto is not to be a port of EMF, it aims at bridging the gap to the .NET world to facilitate code generation of C# class libraries based on an Ecore model using the .NET code available tooling and libraries.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_EcoreNetto&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_EcoreNetto)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_EcoreNetto&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_EcoreNetto)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_EcoreNetto&metric=coverage)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_EcoreNetto)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_EcoreNetto&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_EcoreNetto)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_EcoreNetto&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_EcoreNetto)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_EcoreNetto&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_EcoreNetto)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_EcoreNetto&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_EcoreNetto)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_EcoreNetto&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_EcoreNetto)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_EcoreNetto&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_EcoreNetto)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=RHEAGROUP_EcoreNetto&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=RHEAGROUP_EcoreNetto)

## Installation

The package is available on Nuget at https://www.nuget.org/packages/ECoreNetto/

[![NuGet Badge](https://buildstats.info/nuget/ECoreNetto)](https://buildstats.info/nuget/ECoreNetto)

## Build Status

GitHub actions are used to build and test the library

Branch | Build Status
------- | :------------
Master | ![Build Status](https://github.com/RHEAGROUP/EcoreNetto/actions/workflows/CodeQuality.yml/badge.svg?branch=master)
Development | ![Build Status](https://github.com/RHEAGROUP/EcoreNetto/actions/workflows/CodeQuality.yml/badge.svg?branch=development)

# CodeCov - Code Coverage

Branch      | Build Status
----------- | ------------
Master      | [![codecov](https://codecov.io/gh/RHEAGROUP/EcoreNetto/branch/master/graph/badge.svg?token=2kfZrIOUtI)](https://codecov.io/gh/RHEAGROUP/EcoreNetto)
Development | [![codecov](https://codecov.io/gh/RHEAGROUP/EcoreNetto/branch/development/graph/badge.svg?token=2kfZrIOUtI)](https://codecov.io/gh/RHEAGROUP/EcoreNetto)

## Ecore Documentation

http://download.eclipse.org/modeling/emf/emf/javadoc/2.10.0/index.html?org/eclipse/emf/ecore/EObject.html