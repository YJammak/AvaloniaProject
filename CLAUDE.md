# CLAUDE.md

> **Note**: Versions and file listings below reflect the project at the time this was written.
> Claude should verify against actual `.csproj` and directory listings when making changes.
> The architecture patterns, conventions, and how-to sections are the durable parts.

AvaloniaProject — a modern Avalonia UI desktop application demonstrating MVVM with ReactiveUI, AOT compilation, and real-time internationalization.

## Build & Run

```bash
cd src
dotnet build              # Debug build
dotnet run                # Run the app
dotnet publish -c Release -r win-x64  # AOT publish
```

## Tech Stack

| Category | Library | Version |
|---|---|---|
| UI | Avalonia + Avalonia.Desktop | 12.0.5 |
| MVVM | ReactiveUI.Avalonia + SourceGenerators | 12.0.3 / 3.1.0 |
| DI | Splat + DependencyInjection.SourceGenerator | 19.4.1 / 2.3.0 |
| Theme | Semi.Avalonia + Irihi.Ursa + Ursa.Themes.Semi | 12.0.3 / 2.0.1 |
| Icons | Optris.Icons.Avalonia.MaterialDesign | 12.0.7 |
| Markdown | LiveMarkdown.Avalonia | 2.2.0 |
| Logging | NLog via Splat.NLog | 5.x / 19.4.1 |
| Validation | ReactiveUI.Validation | 7.1.0 |
| Target | net10.0, PublishAot=true, WinExe | |

## Project Structure

```
src/AvaloniaProject/
├── Program.cs              # Entry point + composition root (DI registration)
├── App.axaml / .cs         # Application: themes, styles, ViewLocator, startup
├── ViewLocator.cs          # ViewModel→View type dictionary (AOT-safe, no reflection)
├── AssemblyInfo.cs         # ComVisible, GUID, XmlnsDefinition mappings
├── app.manifest            # Windows compatibility manifest
├── link.xml                # AOT trimmer root descriptors
├── Services/
│   ├── ILocalizationService.cs     # Interface — injectable, mockable
│   ├── LocalizationService.cs      # en-US / zh-Hans via .resx + ResourceManager
│   ├── LocalizationSource.cs       # INotifyPropertyChanged bridge for XAML {Binding [key]}
│   └── SatelliteAssemblyResolver.cs # AOT satellite assembly resolver (locales/ folder)
├── ViewModels/
│   ├── ViewModelBase.cs            # ReactiveObject + IActivatableViewModel + validation
│   ├── MainWindowViewModel.cs      # Wraps MainViewModel for the window
│   ├── MainViewModel.cs            # Discovers IPageViewModel[] via DI, manages SelectedPage
│   ├── AboutViewModel.cs           # Assembly version display
│   └── Pages/
│       ├── IPageViewModel.cs       # Page contract (Name, Icon, Index, IconSize)
│       ├── PageViewModel.cs        # Abstract base: locale-aware Name via CultureChanged
│       ├── HomePageViewModel.cs    # Index=0, mdi-home-outline
│       └── BindingPageViewModel.cs # Binding demo: commands, collections, two-way
├── Views/
│   ├── MainWindow.axaml / .cs      # Title bar: logo, theme toggle, language selector
│   ├── MainView.axaml / .cs        # NavMenu sidebar + ViewModelViewHost content area
│   ├── AboutView.axaml / .cs       # Dialog: logo, name, version
│   └── Pages/
│       ├── HomePageView.axaml / .cs    # LiveMarkdown renderer with light-theme fix
│       └── BindingPageView.axaml / .cs # Compiled AXAML bindings (no code-behind bindings)
├── Utils/
│   └── PageExtensions.cs   # Registers page ViewModels into Splat DI
├── Resources/
│   ├── Strings.resx         # English strings + HomePage content (Markdown)
│   └── Strings.zh-Hans.resx # Simplified Chinese strings + HomePage content
└── Assets/
    └── logo.ico
```

## Architecture Decisions

### DI: Composition Root in Program.cs
All service/ViewModel registration happens in `Program.RegisterServices()`. ViewModels receive dependencies via constructor injection. `Locator.Current.GetService<T>()` calls are confined to:
- `Program.cs` (composition root — acceptable)
- `PageViewModel` constructor (base class infrastructure — one controlled call)
- View constructors (Avalonia controls can't use DI constructors)

### ViewLocator: Type Dictionary (AOT-Safe)
`ViewLocator.cs` uses a `Dictionary<Type, Func<Control>>` instead of `Type.GetType()` + `Activator.CreateInstance`. This is required for AOT compilation. When adding a new page, register the mapping both in `ViewLocator.ViewFactories` and `PageExtensions.UsePages()`.

### Bindings: AXAML Compiled Bindings
`<AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>`. All View→ViewModel bindings use AXAML `{Binding}` with `x:DataType` for compile-time validation. Code-behind bindings are only used where the binding target is not declaratively expressible (e.g., `ViewModelViewHost.ViewModel`).

### Activation Lifecycle: WhenActivated / DisposeWith
All event subscriptions (CultureChanged, etc.) are managed through ReactiveUI's activation lifecycle:
- **ViewModels**: override `OnWhenActivatedAsync(CompositeDisposable)` and use `DisposeWith(disposable)`
- **Views**: use `this.WhenActivated(action)` and `DisposeWith(disposable)`
- **Permanent subscriptions**: only `PageViewModel.Name` CultureChanged (pages live for app lifetime)

### Localization: Lazy<T> + string.Empty PropertyChanged
- `LocalizationSource.Instance` uses `Lazy<T>` for thread-safe singleton
- `LocalizationSource` fires `PropertyChanged(string.Empty)` on culture change (not `"Item[]"`) — Avalonia compiled bindings handle `string.Empty` ("all properties changed") more reliably than indexer notifications
- `App.Initialize()` forces `LocalizationSource.Instance` creation BEFORE `SetCulture()` so the CultureChanged subscription is in place before the initial culture change fires

### Error Handling: Guarded DI Resolution
All `Locator.Current.GetService<T>()` calls use `?? throw new InvalidOperationException("message")` instead of `!` — provides clear diagnostic messages when DI configuration is wrong.

### ViewModelBase Activation
Uses `ContinueWith(OnlyOnFaulted)` instead of `async void` for error handling in the activation pipeline. Errors are logged via Splat/NLog.

## Adding a New Page

1. Create `ViewModels/Pages/NewPageViewModel.cs`:
```csharp
public class NewPageViewModel : PageViewModel
{
    public NewPageViewModel() : base("Page_New", "mdi-icon-name", 2) { }
}
```

2. Create `Views/Pages/NewPageView.axaml` + `.axaml.cs`:
```xml
<u:ReactiveUrsaView x:Class="..." x:TypeArguments="ap:NewPageViewModel"
                    x:DataType="ap:NewPageViewModel" ...>
```
```csharp
public partial class NewPageView : ReactiveUrsaView<NewPageViewModel>
{
    public NewPageView() { InitializeComponent(); }
}
```

3. Register in `Utils/PageExtensions.cs`:
```csharp
Register(() => new NewPageViewModel());
```

4. Register in `ViewLocator.cs`:
```csharp
[typeof(NewPageViewModel)] = () => new NewPageView(),
```

5. Add localization keys to `Resources/Strings.resx` and `Strings.zh-Hans.resx`:
   - `Page_New` — display name for the NavMenu

> If the page needs form validation, see **Adding a Page with Validation** below for ReactiveUI.Validation patterns and known API traps.

## Adding a New Service

1. Create the interface in `Services/` (e.g., `IMyService.cs`)
2. Implement in `Services/` (e.g., `MyService.cs`)
3. Register in `Program.RegisterServices()`:
```csharp
Locator.CurrentMutable.RegisterConstant<IMyService>(new MyService());
```
4. Inject via constructor in ViewModels that need it

## Adding a Page with Validation

When using `ReactiveUI.Validation`, several patterns and pitfalls apply:

### Validation Rules
- `ValidationRule` message parameter only accepts `string` (eager), **not** `Func<string>`. Messages are set once at registration time and won't auto-update on language switch.
- Validator predicates must accept `string?` (not `string`) to match `Func<string?, bool>` and avoid nullable warnings.
- Extract validation predicates as `private static bool IsValidXxx(string? value)` methods so they can be reused in both `ValidationRule` (constructor) and per-field error sync (OnWhenActivatedAsync).

### Per-Field Error Display
- `ValidationContext.Text` is `IValidationText` (implements `IReadOnlyList<string>`), **not** `string` — cannot bind directly in AXAML.
- `ValidationContext` does **not** implement `INotifyDataErrorInfo` — cannot use Avalonia's built-in validation error display.
- To retrieve per-property errors from `ValidationContext.Validations`: cast to avoid `ImmutableArray<T>` extension method conflicts: `foreach (var c in ValidationContext.Validations) { if (c.PropertyName == name && !c.IsValid) { ... } }`
- **Recommended pattern**: create `[Reactive]` properties for each field's error text and visibility, sync via `WhenAnyValue` using the same validation predicates:

```csharp
[Reactive] public partial string UsernameError { get; private set; }
[Reactive] public partial bool HasUsernameError { get; private set; }

// In OnWhenActivatedAsync:
this.WhenAnyValue(x => x.Username)
    .Subscribe(name => {
        var valid = IsValidUsername(name);
        UsernameError = valid ? string.Empty : Localization["Username_Error"];
        HasUsernameError = !valid;
    })
    .DisposeWith(disposable);
```

### Cross-Property Validation
- When field A's validity depends on field B, observe B's `WhenAnyValue` and also update A's error. Example: ConfirmPassword must match Password — the Password subscription calls both `SetFieldError(IsValidPassword(...))` AND re-checks ConfirmPassword.

### Language Switch
- Error messages from `Localization["key"]` are eagerly captured in `WhenAnyValue` handlers. Add a `CultureChanged` subscription that calls a `RefreshAllFieldErrors()` method to re-evaluate all fields with the new locale:

```csharp
Observable.FromEventPattern(
    h => Localization.CultureChanged += h,
    h => Localization.CultureChanged -= h)
    .Subscribe(_ => RefreshAllFieldErrors())
    .DisposeWith(disposable);
```

### Submit Button
- Bind `IsEnabled="{Binding IsFormValid}"` on the Submit button (sync `IsFormValid` from `ValidationContext.WhenAnyValue(x => x.IsValid)`).

### AXAML Layout
- Error text in horizontal StackPanel with label: add `TextWrapping="Wrap"` to handle long messages.
- Theme colors: `{DynamicResource SemiColorDanger}` for errors, `{DynamicResource SemiColorSuccess}` for success.
- Remove `MaxWidth` constraint to prevent horizontal clipping.

### ReactiveUI.Validation API Traps
| Trap | Fix |
|---|---|
| `IValidationText` not `string` | Mirror to `[Reactive] string` property |
| No `INotifyDataErrorInfo` implementation | Manual `WhenAnyValue` sync |
| `ImmutableArray<T>.Where()` conflicts | Use `foreach` or explicit cast |
| `Func<TValue, bool>` expects `TValue?` | Use `string?` parameter |
| Message is `string` not `Func<string>` | Use `CultureChanged` to refresh |

## Key Conventions

- Namespace prefix `ap:` maps to `https://yjammak.net/avalonia-project` (defined in `AssemblyInfo.cs`)
- `reactiveUrsaView` / `reactiveUrsaWindow` base classes from Ursa provide typed `ViewModel` property
- `[Reactive]` source generator creates `partial` properties with INPC — the partial class must be declared `partial`
- Satellite assemblies (`.resources.dll`) are relocated to `locales/{culture}/` by MSBuild targets for AOT compatibility
- XAML uses `{DynamicResource}` for theme-aware resources (SemiColorPrimary, CardBorder, etc.)
- XAML uses `Source={x:Static ap:LocalizationSource.Instance}` for localized strings

## Known Trade-offs

- **PageViewModel uses Locator.Current** for ILocalizationService — pragmatic compromise: base infrastructure class resolves its own dependency so subclasses don't need constructor parameters for it
- **View constructors use Locator.Current** — Avalonia controls can't use DI constructors (created by `new` in ViewLocator); one-time resolution in constructor is acceptable
- **No unit test project** — ViewModels depend on interfaces (`ILocalizationService`, `IPageViewModel`), so tests are possible; just not yet added
- **Empty Models/ folder** — declared in csproj and AssemblyInfo but unused; safe to populate or remove
- **Page registration requires two changes** — `PageExtensions` (DI) + `ViewLocator` (View mapping); see "Adding a New Page" above
