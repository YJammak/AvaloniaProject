# AvaloniaProject

A modern Avalonia UI desktop application demonstrating MVVM architecture with ReactiveUI 24 + Primitives, AOT compilation, and real-time internationalization.

## Tech Stack

| Category | Library | Version |
|---|---|---|
| UI Framework | [Avalonia](https://avaloniaui.net/) | 12.1 |
| MVVM | [ReactiveUI](https://reactiveui.net/) + ReactiveUI.Primitives | 24.0 / 7.1 |
| Source Gen | [ReactiveUI.SourceGenerators](https://github.com/reactiveui/ReactiveUI.SourceGenerators) | 3.1 |
| DI | [Splat](https://github.com/reactiveui/splat) + Source Generator | 20.2 / 2.3 |
| Theme | [Semi.Avalonia](https://github.com/irihitech/Semi.Avalonia) + [Ursa](https://github.com/irihitech/Ursa.Avalonia) | 12.1 / 2.1 |
| Icons | [Optris.Icons.Avalonia](https://github.com/Optris/Optris.Icons.Avalonia) (Material Design) | 12.0 |
| Markdown | [LiveMarkdown.Avalonia](https://github.com/giacomelli/LiveMarkdown.Avalonia) | 2.2 |
| Logging | [NLog](https://nlog-project.org/) via Splat.NLog | 20.2 |
| Runtime | .NET 10 / AOT (`PublishAot`) | |

## Architecture

```
Program.cs                     ← Composition root (ConfigureViewLocator + DI)
App.axaml                      ← Theme, styles, DataTemplates
App.axaml.cs                   ← Localization init, MainWindow creation
├── Views/
│   ├── ReactiveUrsaBase.cs        ← IViewFor<T> base (UserControl + UrsaWindow)
│   ├── MainWindow.axaml/.cs       ← Title bar, theme/language toggle, VVH
│   ├── MainView.axaml/.cs         ← NavMenu sidebar + VVH page host
│   ├── AboutView.axaml/.cs        ← App version dialog
│   └── Pages/
│       ├── HomePageView.axaml/.cs     ← LiveMarkdown renderer
│       ├── BindingPageView.axaml/.cs  ← Compiled AXAML binding demo
│       └── ValidationPageView.axaml/.cs ← Manual form validation
├── ViewModels/
│   ├── ViewModelBase.cs           ← ReactiveObject + IActivatableViewModel
│   ├── MainWindowViewModel.cs     ← Wraps MainViewModel
│   ├── MainViewModel.cs           ← Discovers IPageViewModel[] via DI
│   ├── AboutViewModel.cs          ← Assembly version display
│   └── Pages/
│       ├── IPageViewModel.cs      ← Page contract (Name, Icon, Index)
│       ├── PageViewModel.cs       ← Locale-aware Name via CultureChanged
│       ├── HomePageViewModel.cs   ← Index=0, mdi-home-outline
│       ├── BindingPageViewModel.cs ← Commands (RxVoid), ObservableCollection
│       └── ValidationPageViewModel.cs ← Manual validation with [Reactive] error props
├── Services/
│   ├── ILocalizationService.cs    ← Interface (injectable, mockable)
│   ├── LocalizationService.cs     ← en-US / zh-Hans via .resx
│   ├── LocalizationSource.cs      ← INotifyPropertyChanged bridge for XAML
│   └── SatelliteAssemblyResolver.cs ← AOT satellite assembly loader
└── Utils/
    └── PageExtensions.cs          ← Page ViewModel DI registration
```

## Key Design Decisions

- **Constructor injection** — all dependencies declared explicitly; composition root in `Program.cs`
- **ConfigureViewLocator + Map<T,T>()** — ReactiveUI 24 native view registration, AOT-safe
- **ReactiveUI 24 Primitives** — `RxVoid`, `MultipleDisposable`, `Signal.FromEventPattern`, zero System.Reactive dependency
- **Manual ReactiveCommands** — typed `ReactiveCommand<RxVoid, RxVoid>` (source generator removed in 3.1)
- **ObservableCollection<T>** — replaces DynamicData SourceList; .NET native, zero extra dependencies
- **Compiled bindings** — `<AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>`, bindings in AXAML with `x:DataType`
- **ReactiveUrsaBase** — Custom `IViewFor<T>` base classes replacing binary-incompatible `Irihi.Ursa.ReactiveUIExtension`
- **Interface-based** — `ILocalizationService`, `IPageViewModel` for testability
- **Activation lifecycle** — all event subscriptions managed by `WhenActivated`/`DisposeWith`
- **Localization** — real-time English/Simplified Chinese switching; satellite assemblies relocated for AOT

## Features

- **Sidebar navigation** — Ursa `NavMenu` with `ViewModelViewHost` page switching
- **ReactiveUI binding showcase** — Two-way, Command, Boolean, and Collection bindings
- **Form validation** — Manual per-field error sync with `WhenAnyValue` + `[Reactive]` error properties
- **Light/Dark/System theme** — Semi.Avalonia + Ursa `ThemeToggleButton`
- **Live language switching** — CultureChanged propagates to all Views/ViewModels
- **Markdown home page** — Locale-aware content rendered with LiveMarkdown
- **AOT-ready** — `PublishAot=true`, `link.xml`, zero reflection view dispatch

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
    ├── Program.cs            ← Entry point + composition root
    ├── App.axaml/.cs         ← Application definition
    ├── Services/             ← Business services + interfaces
    ├── ViewModels/           ← MVVM ViewModels (Pages/)
    ├── Views/                ← AXAML Views (Pages/)
    │   └── ReactiveUrsaBase.cs ← Custom IViewFor<T> base classes
    ├── Utils/                ← Extension methods
    ├── Resources/            ← .resx (EN + zh-Hans)
    └── Assets/               ← logo.ico
```
