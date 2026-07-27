using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Primitives.Disposables;
using ReactiveUI.SourceGenerators;
using Splat;
using static ReactiveUI.Primitives.LinqExtensions;
using RxVoid = ReactiveUI.Primitives.RxVoid;

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

    public ReactiveCommand<RxVoid, RxVoid> SubmitCommand { get; }
    public ReactiveCommand<RxVoid, RxVoid> ResetCommand { get; }

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

        SubmitCommand = ReactiveCommand.Create(Submit);
        ResetCommand = ReactiveCommand.Create(Reset);
    }

    protected override async Task OnWhenActivatedAsync(MultipleDisposable disposable)
    {
        await base.OnWhenActivatedAsync(disposable);

        // Clear result when any field changes
        this.WhenAnyValue(
                x => x.Username,
                x => x.Email,
                x => x.Password,
                x => x.ConfirmPassword)
            .SubscribeSafe(_ => FormResult = string.Empty,
                ex => this.Log().Error(ex, "Error resetting form result"))
            .DisposeWith(disposable);

        // Sync HasFormResult for XAML visibility
        this.WhenAnyValue(x => x.FormResult)
            .SubscribeSafe(_ => this.RaisePropertyChanged(nameof(HasFormResult)),
                ex => this.Log().Error(ex, "Error updating HasFormResult"))
            .DisposeWith(disposable);

        // Per-field error sync using the same predicates as the validators
        this.WhenAnyValue(x => x.Username)
            .SubscribeSafe(name => SetFieldError(
                IsValidUsername(name), ref _usernameError,
                Localization["Validation_Username_Error"],
                v => UsernameError = v, v => HasUsernameError = v),
                ex => this.Log().Error(ex, "Error validating username"))
            .DisposeWith(disposable);

        this.WhenAnyValue(x => x.Email)
            .SubscribeSafe(email => SetFieldError(
                IsValidEmail(email), ref _emailError,
                Localization["Validation_Email_Error"],
                v => EmailError = v, v => HasEmailError = v),
                ex => this.Log().Error(ex, "Error validating email"))
            .DisposeWith(disposable);

        this.WhenAnyValue(x => x.Password)
            .SubscribeSafe(pwd =>
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
            }, ex => this.Log().Error(ex, "Error validating password"))
            .DisposeWith(disposable);

        this.WhenAnyValue(x => x.ConfirmPassword)
            .SubscribeSafe(confirm => SetFieldError(
                string.IsNullOrWhiteSpace(confirm) || confirm == Password,
                ref _confirmPasswordError,
                Localization["Validation_ConfirmPassword_Error"],
                v => ConfirmPasswordError = v, v => HasConfirmPasswordError = v),
                ex => this.Log().Error(ex, "Error validating confirm password"))
            .DisposeWith(disposable);

        // Re-evaluate all field errors when language changes (error messages are localized)
        EventHandler cultureHandler = (_, _) => RefreshAllFieldErrors();
        Localization.CultureChanged += cultureHandler;
        new ActionDisposable(() => Localization.CultureChanged -= cultureHandler)
            .DisposeWith(disposable);

        // Compute IsFormValid from all four field predicates
        this.WhenAnyValue(
                x => x.Username,
                x => x.Email,
                x => x.Password,
                x => x.ConfirmPassword,
                (username, email, pwd, confirm) =>
                    IsValidUsername(username) && IsValidEmail(email) && IsValidPassword(pwd) &&
                    !string.IsNullOrWhiteSpace(confirm) && confirm == pwd)
            .SubscribeSafe(valid => IsFormValid = valid,
                ex => this.Log().Error(ex, "Error computing form validity"))
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

    private void Submit()
    {
        if (!IsFormValid)
            return;

        FormResult = string.Format(Localization["Validation_Submit_Success"], Username);
    }

    private void Reset()
    {
        Username = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        FormResult = string.Empty;
    }
}
