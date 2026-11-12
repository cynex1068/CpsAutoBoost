# CPS AUTO BOOST

**English** | [Deutsch](README-DE.md)

**CPS AUTO BOOST** is a Windows desktop tool that automatically increases mouse click output based on real physical clicks per second.

It monitors physical left and right mouse clicks globally, calculates the real CPS for each button, and sends additional mouse input when the configured activation condition is reached.

## Features

- Global left and right mouse monitoring
- Automatic click boosting through the Windows input API
- Real CPS is calculated from physical clicks only
- Simulated clicks are excluded from CPS calculations
- Simulated clicks cannot trigger another boost cycle
- Configurable activation conditions for left and right buttons
- Configurable multipliers for left and right buttons
- English, French, German, Russian, Ukrainian, Chinese, and Japanese UI
- System language detection by default
- Unsupported system languages fall back to English
- Self-contained single-file Windows executable publishing

## Default Settings

| Button | Activation condition | Default multiplier | Default result |
| --- | ---: | ---: | ---: |
| Left | x >= 5 | 2x | y = 2x |
| Right | x >= 4 | 3x | y = 3x |

The activation conditions and multipliers can be changed directly in the application.

## Calculation

`x` represents the real CPS measured from physical mouse clicks.

For the left button:

```text
if x >= left activation condition:
    y = x * left multiplier
else:
    y = x
```

For the right button:

```text
if x >= right activation condition:
    y = x * right multiplier
else:
    y = x
```

The application sends only the additional clicks required to reach the target rate. It does not add a complete extra second of clicks for every physical click.

## Safety of CPS Measurement

Automatically generated clicks are marked and filtered by the global mouse hook. While automatic input is being sent, the application suppresses those events from the real CPS counters. This prevents simulated clicks from increasing `x` or causing recursive acceleration.

## Requirements

- Windows 10 or later
- .NET 8 SDK for building from source
- x64 Windows system

The published executable is self-contained and does not require the .NET runtime to be installed separately.

## Build from Source

Install the .NET 8 SDK, open PowerShell in the project directory, and run:

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\build.ps1
```

The generated executable will be available at:

```text
publish\WindowsCpsTool.exe
```

## Usage

1. Start `CPS AUTO BOOST`.
2. Select a language or keep `System` to follow the Windows language.
3. Set the left and right activation conditions.
4. Set the left and right multipliers.
5. Enable automatic boosting.
6. Disable automatic boosting before changing applications where automatic mouse input is not desired.

CPS AUTO BOOST only sends mouse input after the feature is enabled. Closing the application removes the global mouse hook and stops automatic input.

## License

This project is licensed under the **MIT License**.
