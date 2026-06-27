# AvaloniaProject

A modern Avalonia UI desktop application showcasing best practices for building .NET cross-platform apps with the MVVM pattern, AOT compilation, and internationalization.

## Tech Stack

| Category | Library | Version |
|---|---|---|
| UI Framework | [Avalonia](https://avaloniaui.net/) | 12.0 |
| MVVM | [ReactiveUI](https://reactiveui.net/) + Source Generators | 12.0 / 3.x |
| DI | [Splat](https://github.com/reactiveui/splat) | 19.4 |
| Theme | [Semi.Avalonia](https://github.com/irihitech/Semi.Avalonia) + [Ursa](https://github.com/irihitech/Ursa.Avalonia) | 12.0 / 2.0 |
| Icons | [Optris.Icons.Avalonia](https://github.com/Optris/Optris.Icons.Avalonia) (Material Design) | 12.0 |
| Markdown | [LiveMarkdown.Avalonia](https://github.com/giacomelli/LiveMarkdown.Avalonia) | 2.2 |
| Logging | [NLog](https://nlog-project.org/) via Splat.NLog | 5.x |
| Runtime | .NET 10 / AOT (PublishAot) | |

## Architecture

```
Program.cs                  ← Composition root (DI registration)
├── App.axaml               ← Theme, styles, ViewLocator
├── Services/
│   ├── ILocalizationService    ← Interface (testable)
│   ├── LocalizationService     ← en-US / zh-Hans via .resx
│   └── LocalizationSource      ← INotifyPropertyChanged bridge for XAML bindings
├── ViewModels/
│   ├── ViewModelBase           ← ReactiveObject + activation lifecycle
│   ├── MainWindowViewModel     ← Injects MainViewModel
│   ├── MainViewModel           ← Injects IEnumerable<IPageViewModel>
│   └── Pages/
│       ├── IPageViewModel      ← Page contract (testable)
│       ├── PageViewModel       ← Base class with locale-aware Name
│       ├── HomePageViewModel   ← Markdown welcome page
│       └── BindingPageViewModel ← Binding demo (commands, collections)
├── Views/
│   ├── MainWindow              ← Title bar, language toggle, theme toggle
│   ├── MainView                ← NavMenu sidebar + page host
│   └── Pages/
│       ├── HomePageView        ← LiveMarkdown renderer
│       └── BindingPageView     ← Compiled AXAML bindings demo
└── Utils/
    └── PageExtensions          ← Page registration into DI
```

## Key Design Decisions

- **Constructor injection** — all dependencies declared explicitly; composition root in `Program.cs`
- **AOT-safe ViewLocator** — type dictionary mapping instead of `Type.GetType()` reflection
- **Compiled bindings** — `<AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>`, bindings declared in AXAML with `x:DataType`
- **Interface-based** — `ILocalizationService`, `IPageViewModel` for testability
- **Activation lifecycle** — all event subscriptions managed by `WhenActivated`/`DisposeWith`
- **Localization** — real-time English/Simplified Chinese switching; satellite assemblies relocated for AOT

## Features

- **Sidebar navigation** — Ursa `NavMenu` with `ViewModelViewHost` page switching
- **ReactiveUI binding showcase** — Two-way, Command, Boolean, and Collection bindings
- **Light/Dark/System theme** — Semi.Avalonia + Ursa `ThemeToggleButton`
- **Live language switching** — CultureChanged propagates to all Views via `INotifyPropertyChanged`
- **Markdown home page** — Locale-aware content rendered with LiveMarkdown

## Getting Started

```bash
cd src
dotnet run
```

Publish with AOT:

```bash
dotnet publish -c Release -r win-x64
```

## Project Structure

```
src/
└── AvaloniaProject/         ← Single .NET 10 project
    ├── Program.cs            ← Entry point
    ├── App.axaml/.cs         ← Application definition
    ├── ViewLocator.cs        ← ViewModel→View resolution
    ├── Services/             ← Business services + interfaces
    ├── ViewModels/           ← MVVM ViewModels (Pages/)
    ├── Views/                ← AXAML Views (Pages/)
    ├── Utils/                ← Extension methods
    ├── Resources/            ← .resx (EN + zh-Hans)
    └── Assets/               ← logo.ico
```
