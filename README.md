# Uniray


Uniray is a game engine based on the C# [wrapper](https://github.com/ChrisDill/Raylib-cs) for the graphic library [Raylib](https://www.raylib.com/). 
It's purpose is to bring easy Game Development and Design while using a relatively low level tool to make it simple and intuitive to use. You will
find below a guide to build the engine yourself, as it hasn't officially released yet.

## Build

- [ ] [Install](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) the **Net7.0 Framework** (minimum required) if you don't have it already.
- [ ] Run the following command in the project directory:

```
dotnet build
```

This will generate a bin folder containing all the files you need. Simply run the .exe and start designing!

## Publish 

If you want to get a self-contained version (containg the framework), run the following command instead:

```
dotnet publish -c Release -r win-x64 --self-contained
```

This will output a win-x64 folder (or some other runtime, list [here](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog#known-rids)) that 
contains the needed files including the framework ones. You can now run it on any computer that hasn't dotnet installed!

## Contact

The project is still in development and reviews and advices are welcome. If you have any question, consider contacting me at Evan.Comtesse@rpn.ch