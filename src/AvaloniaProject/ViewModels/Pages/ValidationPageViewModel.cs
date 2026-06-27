using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;

namespace AvaloniaProject.ViewModels.Pages;

public partial class ValidationPageViewModel : PageViewModel
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex UsernameRegex = new(
        @"^[a-zA-Z0-9_-]{3,20}$",
        RegexOptions.Compiled);

    // ── reusable validation predicates ──
    private static bool IsValidUsername(string? name) =>
        !string.IsNullOrWhiteSpace(name) && UsernameRegex.IsMatch(name);

    private static bool IsValidEmail(string? email) =>
        !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);

    private static bool IsValidPassword(string? pwd) =>
        !string.IsNullOrWhiteSpace(pwd) && pwd.Length >= 8
        && pwd.Any(char.IsUpper) && pwd.Any(char.IsLower) && pwd.Any(char.IsDigit);

    [Reactive]
    public partial string Username { get; set; }

    [Reactive]
    public partial string Email { get; set; }

    [Reactive]
    public partial string Password { get; set; }

    [Reactive]
    public partial string ConfirmPassword { get; set; }

    [Reactive]
    public partial string FormResult { get; private set; }

    public bool HasFormResult => !string.IsNullOrWhiteSpace(FormResult);

    [Reactive]
    public partial string UsernameError { get; private set; }

    [Reactive]
    public partial string EmailError { get; private set; }

    [Reactive]
    public partial string PasswordError { get; private set; }

    [Reactive]
    public partial string ConfirmPasswordError { get; private set; }

    [Reactive]
    public partial bool HasUsernameError { get; private set; }

    [Reactive]
    public partial bool HasEmailError { get; private set; }

    [Reactive]
    public partial bool HasPasswordError { get; private set; }

    [Reactive]
    public partial bool HasConfirmPasswordError { get; private set; }

    [Reactive]
    public partial bool IsFormValid { get; private set; }

    public ValidationPageViewModel() :
        base("Page_Validation", "mdi-check-circle-outline", 2)
    {
        Username = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        FormResult = string.Empty;
        UsernameError = string.Empty;
        EmailError = string.Empty;
        PasswordError = string.Empty;
        ConfirmPasswordError = string.Empty;

        // ── Field validators ──
        this.ValidationRule(
            vm => vm.Username,
            IsValidUsername,
            Localization["Validation_Username_Error"]);

        this.ValidationRule(
            vm => vm.Email,
            IsValidEmail,
            Localization["Validation_Email_Error"]);

        this.ValidationRule(
            vm => vm.Password,
            IsValidPassword,
            Localization["Validation_Password_Error"]);

        this.ValidationRule(
            vm => vm.ConfirmPassword,
            confirm => string.IsNullOrWhiteSpace(confirm) || confirm == Password,
            Localization["Validation_ConfirmPassword_Error"]);
    }

    protected override async Task OnWhenActivatedAsync(CompositeDisposable disposable)
    {
        await base.OnWhenActivatedAsync(disposable);

        // Clear result when any field changes
        this.WhenAnyValue(
                x => x.Username,
                x => x.Email,
                x => x.Password,
                x => x.ConfirmPassword)
            .Do(_ => FormResult = string.Empty)
            .Subscribe(_ => { })
            .DisposeWith(disposable);

        // Sync HasFormResult for XAML visibility
        this.WhenAnyValue(x => x.FormResult)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(HasFormResult)))
            .DisposeWith(disposable);

        // Per-field error sync using the same predicates as the validators
        this.WhenAnyValue(x => x.Username)
            .Subscribe(name => SetFieldError(
                IsValidUsername(name), ref _usernameError,
                Localization["Validation_Username_Error"],
                v => UsernameError = v, v => HasUsernameError = v))
            .DisposeWith(disposable);

        this.WhenAnyValue(x => x.Email)
            .Subscribe(email => SetFieldError(
                IsValidEmail(email), ref _emailError,
                Localization["Validation_Email_Error"],
                v => EmailError = v, v => HasEmailError = v))
            .DisposeWith(disposable);

        this.WhenAnyValue(x => x.Password)
            .Subscribe(pwd =>
            {
                SetFieldError(
                    IsValidPassword(pwd), ref _passwordError,
                    Localization["Validation_Password_Error"],
                    v => PasswordError = v, v => HasPasswordError = v);

                // Re-check ConfirmPassword when Password changes
                SetFieldError(
                    string.IsNullOrWhiteSpace(ConfirmPassword) || ConfirmPassword == pwd,
                    ref _confirmPasswordError,
                    Localization["Validation_ConfirmPassword_Error"],
                    v => ConfirmPasswordError = v, v => HasConfirmPasswordError = v);
            })
            .DisposeWith(disposable);

        this.WhenAnyValue(x => x.ConfirmPassword)
            .Subscribe(confirm => SetFieldError(
                string.IsNullOrWhiteSpace(confirm) || confirm == Password,
                ref _confirmPasswordError,
                Localization["Validation_ConfirmPassword_Error"],
                v => ConfirmPasswordError = v, v => HasConfirmPasswordError = v))
            .DisposeWith(disposable);

        // Re-evaluate all field errors when language changes (error messages are localized)
        Observable.FromEventPattern(
                h => Localization.CultureChanged += h,
                h => Localization.CultureChanged -= h)
            .Subscribe(_ => RefreshAllFieldErrors())
            .DisposeWith(disposable);

        ValidationContext.WhenAnyValue(x => x.IsValid)
            .Subscribe(valid => IsFormValid = valid)
            .DisposeWith(disposable);
    }

    private void RefreshAllFieldErrors()
    {
        SetFieldError(IsValidUsername(Username), ref _usernameError,
            Localization["Validation_Username_Error"],
            v => UsernameError = v, v => HasUsernameError = v);

        SetFieldError(IsValidEmail(Email), ref _emailError,
            Localization["Validation_Email_Error"],
            v => EmailError = v, v => HasEmailError = v);

        SetFieldError(IsValidPassword(Password), ref _passwordError,
            Localization["Validation_Password_Error"],
            v => PasswordError = v, v => HasPasswordError = v);

        var confirmOk = string.IsNullOrWhiteSpace(ConfirmPassword) || ConfirmPassword == Password;
        SetFieldError(confirmOk, ref _confirmPasswordError,
            Localization["Validation_ConfirmPassword_Error"],
            v => ConfirmPasswordError = v, v => HasConfirmPasswordError = v);
    }

    private static void SetFieldError(bool isValid, ref string field, string errorMessage,
        Action<string> setError, Action<bool> setHasError)
    {
        var message = isValid ? string.Empty : errorMessage;
        if (message == field) return;
        field = message;
        setError(message);
        setHasError(!string.IsNullOrWhiteSpace(message));
    }

    // Backing fields for dedup comparison
    private string _usernameError = string.Empty;
    private string _emailError = string.Empty;
    private string _passwordError = string.Empty;
    private string _confirmPasswordError = string.Empty;

    [ReactiveCommand]
    private void Submit()
    {
        if (!ValidationContext.IsValid)
            return;

        FormResult = string.Format(Localization["Validation_Submit_Success"], Username);
    }

    [ReactiveCommand]
    private void Reset()
    {
        Username = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        FormResult = string.Empty;
    }
}
