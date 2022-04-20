# DotNet.J2Class [![Nuget](https://img.shields.io/nuget/v/DotNetJ2Class)](https://www.nuget.org/packages/DotNetJ2Class/) ![Nuget](https://img.shields.io/nuget/dt/DotNetJ2Class)

This lib helps you to create a class at runtime from a JSON. It means that you don't need to change your class every time a JSON that you consume has changed.


## Notes
- Upgrade to .NET 6
## BETA Version
You can convert only simple JSON like "{Foo: barValue}". <br>
NEW: you can try to convert this kind of JSON "{'Foo': {Bar1: barValue, Bar2: bar2Value}}".

## Installation

Use the package manager to install.

```bash
Install-Package DotNetJ2Class -Version 1.0.3
```

## Usage

After install the package, add this code in your "using" block:
```C#
using DotNet.J2Class;
```
and
```C#
string json = @"{'Foo': 'barValue'}";

//You can set a name
//for the class that will be created.
//If you don't pass any name,
//it will be create with DefaultName.
//The same happens with module name.
//Both parameters are optional.
string className = "CLASS_NAME";
string moduleName = "MODULE_NAME"

var myObject = J2Class.CreateObjectFromJson(json, className, moduleName);
//myObject will be like a "className" object with a string property called "Foo" and its value "barValue" 
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
