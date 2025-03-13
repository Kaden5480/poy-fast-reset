# poy-fast-reset
![Code size](https://img.shields.io/github/languages/code-size/Kaden5480/poy-fast-reset?color=5c85d6)
![Open issues](https://img.shields.io/github/issues/Kaden5480/poy-fast-reset?color=d65c5c)
![License](https://img.shields.io/github/license/Kaden5480/poy-fast-reset?color=a35cd6)

A
[Peaks of Yore](https://store.steampowered.com/app/2236070/)
mod which quickly teleports you to the start of a level (among many other things).

Please see [the guide](Guide.md) for more information.

![Fast Reset Demo](assets/demo.gif)

# Overview
- [Installing](#installing)
- [Building from source](#building-from-source)
    - [Dotnet](#dotnet-build)
    - [Visual Studio](#visual-studio-build)
    - [Custom game locations](#custom-game-locations)
- [Credits](#credits)

# Installing
## BepInEx
If you haven't installed BepInEx yet, follow the install instructions here:
- [Windows](https://github.com/Kaden5480/modloader-instructions#bepinex-windows)
- [Linux](https://github.com/Kaden5480/modloader-instructions#bepinex-linux)

### Fast reset
- Download the latest release
[here](https://github.com/Kaden5480/poy-fast-reset/releases).
- The compressed zip will contain a `plugins` directory.
- Copy the files in `plugins` to `BepInEx/plugins` in your game directory.

# Building from source
Whichever approach you use for building from source, the resulting
plugin/mod can be found in `bin/`.

The following configurations are supported:
- Debug
- Release

## Dotnet build
To build with dotnet, run the following command, replacing
<configuration> with the desired value:
```sh
dotnet build -c <configuration>
```

## Visual Studio build
To build with Visual Studio, open FastReset.sln and build by pressing ctrl + shift + b,
or by selecting Build -> Build Solution.

## Custom game locations
If you installed Peaks of Yore in a custom game location, you may require
an extra file to configure the build so it knows where to find the Peaks of Yore game
libraries.

The file must be in the root of this repository and must be called "GamePath.props".

Below gives an example where Peaks of Yore is installed on the F drive:
```xml
<Project>
  <PropertyGroup>
    <GamePath>F:\Games\Peaks of Yore</GamePath>
  </PropertyGroup>
</Project>
```

# Credits
- [Elvonia](https://github.com/Elvonia) for help with figuring out camera rotations and submitting a PR
- [Xearo](https://www.youtube.com/@xearo1331) for help with designing the UI and testing development builds
- [Revan] for testing development builds
