![RxGen logo](https://raw.githubusercontent.com/jrgcubano/RxGen/master/art/rxgen-logo-256x256.png)

[![Build status](https://ci.appveyor.com/api/projects/status/m6pywkwtki9wua1y/branch/master?svg=true)](https://ci.appveyor.com/project/jrgcubano/rxgen/branch/master)
[![NuGet](https://img.shields.io/nuget/v/rxgen.svg)](https://www.nuget.org/packages/RxGen/)

<!--[![Travis](https://img.shields.io/travis/jrgcubano/RxGEN.svg?maxAge=3600&label=travis)](https://travis-ci.org/jrgcubano/RxGen)-->
<!-- [![MyGet CI](https://img.shields.io/myget/jrogalan/v/rxgen.svg)](http://myget.org/gallery/jrogalan) -->

## What is RxGen?

Generate random data to use and test real apps, using a fluent api with reactive extensions.

## Quick start

Install into your project using

```
dotnet add package RxGen
```

Configure People generator with basic seed info and subscribe to it as observable.

```csharp
RxGen.People()
     .Ammount(10)
     .Nationality(Nationality.ES)
     .Gender(Gender.Female);
     .AsObservable()
     .Subscribe(
         res => Console.WriteLine($"Users: {res.Result[0]}")
     );
```

## Where can I get it?

Install using the [RxGen NuGet package](https://www.nuget.org/packages/RxGen):

```
PM> Install-Package RxGen
```

or

```
dotnet add package RxGen
```

## How do I use it?

When you install the package, it should be added to your `csproj`. Alternatively, you can add it directly by adding:

```xml
<PackageReference Include="RxGen" Version="0.1.0" />
```

### People generator

* Basic use case with observables

Configure generator with basic info exposed as an observable and subscribe to it.

In the example, we generate five female users of Spanish nationality.

```csharp
RxGen.People()
     .Ammount(5)
     .Nationality(Nationality.ES)
     .Gender(Gender.Female)
     .AsObservable()
     .Subscribe(
        (res) => Console.WriteLine($"Users: {res.Result[0]}"),
        (ex) => Console.WriteLine($"Error: {ex.Message}")
     );
```

* Get results from Task

Configure generator and just get it as a normal task.

```csharp
var generator = RxGen
    .People()
    .Ammount(5)
    .Nationality(Nationality.ES)
    .Gender(Gender.Female);

result = await generator.AsTask();

Console.WriteLine($"Users: {result.Result[0]}");

```

* Simple pagination

Configure generator with a seed reference name to remember the past results when paging.

```csharp
var generator = RxGen
    .People()
    .Seed("mypeople")
    .Ammount(5)
    .Nationality(Nationality.ES, Nationality.US)
    .Gender(Gender.Female);
```

And then request any page whenever you want.

```csharp
generator
    .Page(1)
    .AsObservable()
    .Subscribe(
       (res) => Console.WriteLine($"Page: 1, Users: {res.Result[0]}"),
       (ex) => Console.WriteLine($"Error: {ex.Message}")
    );

generator
    .Page(2)
    .AsObservable()
    .Subscribe(
        (res) => Console.WriteLine($"Page: 2, Users: {res.Result[0]}"),
        (ex) => Console.WriteLine($"Error: {ex.Message}")
    );
```

* Automatic pagination

Repeat automatic paging from the first to the last page and vice versa. You need to specify a maximum total of pages and a delay between
each paging call.

Cycle: page up until max page -> then down until first -> repeat the process...

```csharp
RxGen.People()
     .Seed("mypeople")
     .Ammount(5)
     .Nationality(Nationality.US)
     .Gender(Gender.Male)
     .AutoPagination(5, TimeSpan.FromSeconds(2))
     .Subscribe(
         (res) => Console.WriteLine($"Page: {res.Info.Page}, Users: {res.Result[0]}"),
         (ex) => Console.WriteLine($"Error: {ex.Message}")
     );
```

* Repeat forever with delay

Configure generator to repeat forever with delay between calls.

```csharp
RxGen.People()
     .Ammount(5)
     .Nationality(Nationality.US)
     .Gender(Gender.Male)
     .RepeatForever(TimeSpan.FromSeconds(2))
     .Subscribe(
        (res) => Console.WriteLine($"Users: {res.Result[0]}"),
        (ex) => Console.WriteLine($"Error: {ex.Message}")
     );
```

* Repeat forever with random error

Configure generator to repeat forever with a random error.
You need to specify an error probability and a delay between each call.

The example is configured to generate some error with a 10% probability between calls.
This is a common use case in a real scenario.

```csharp
RxGen.People()
     .Ammount(5)
     .Nationality(Nationality.US)
     .Gender(Gender.Male)
     .RepeatWithRandomError(0.1, TimeSpan.FromSeconds(2))
     .Subscribe(
        (res) => Console.WriteLine($"Users: {res.Result[0]}"),
        (ex) => Console.WriteLine($"Error: {ex.Message}")
     );
```

## Performance Tips
Remember to free up your resources by implementing IDisposable when you need it.

```csharp
public class PlayWithRxGen : IDisposable
{
    private IDisposable _subscription;

    private IObservable<GenPeopleResponse> GetUsers() =>
        RxGen.People()
             .Ammount(1)
             .RepeatForever(TimeSpan.FromSeconds(2));

    public void Play() =>
        _subscription = GetUsers()
            .Subscribe(
                (res) => Console.WriteLine($"Users: {res.Result[0]}"),
                (ex) => Console.WriteLine($"Error: {ex.Message}")
            );

    public void Dispose() =>
        _subscription?.Dispose();
}
```

## Contribute

It would be awesome if you would like to contribute code or help with bugs. Just follow the guidelines [CONTRIBUTING](https://github.com/jrgcubano/RxGen/blob/master/CONTRIBUTING.md).

## Additional Resources
* [API for generating random user data](https://randomuser.me/)
* [Project based on RxPeople for Java](https://github.com/cesarferreira/RxPeople)