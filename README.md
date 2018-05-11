# ECoreNetto

## Introduction

ECoreNetto is a dotnet core library that is used to deserialize an Ecore meta-model for the purpose of code generation. Ecore is  a meta-model used to represent models in the Eclipse Modelling Framework. EMF is a powerful framework and code generation facility for building Java applications based on simple model definitions. The intention of ECoreNetto is not to be a port of EMF, it aims at bridging the gap to the .NET world to facilitate code generation of C# class libraries based on an Ecore model using the .NET code available tooling and libraries.

## Installation

The package is available on Nuget at https://www.nuget.org/packages/ECoreNetto/

[![NuGet Badge](https://buildstats.info/nuget/ECoreNetto)](https://buildstats.info/nuget/ECoreNetto)

## Build Status

AppVeyor is used to build and test the library

Branch | Build Status
------- | :------------
Master |  [![Build Status](https://ci.appveyor.com/api/projects/status/eumtw7k31iko03hh/branch/master?svg=true)](https://ci.appveyor.com/api/projects/status/eumtw7k31iko03hh)
Development |  [![Build Status](https://ci.appveyor.com/api/projects/status/eumtw7k31iko03hh/branch/development?svg=true)](https://ci.appveyor.com/api/projects/status/eumtw7k31iko03hh)

[![Build history](https://buildstats.info/appveyor/chart/samatrhea/ECoreNetto)](https://ci.appveyor.com/project/samatrhea/ECoreNetto/history)

## Ecore Documentation

http://download.eclipse.org/modeling/emf/emf/javadoc/2.10.0/index.html?org/eclipse/emf/ecore/EObject.html