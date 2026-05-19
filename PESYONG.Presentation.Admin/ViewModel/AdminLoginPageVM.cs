using CMS.Contracts.Admin.Auth;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PESYONG.Presentation.Admin.Services;

namespace PESYONG.Presentation.Admin.ViewModel;

public sealed class AdminLoginPageVM : ObservableObject
{
    private readonly IAdminAuthService _authService;
    private readonly IAdminTokenStore _tokenStore;
    private readonly AdminSession _session;

    public AdminLoginPageVM(
        IAdminAuthService authService,
        IAdminTokenStore tokenStore,
        AdminSession session)
    {
        _authService = authService;
        _tokenStore = tokenStore;
        _session = session;

        LoginCommand = new AsyncRelayCommand(LoginAsync, () => !IsBusy);
        VerifyCodeCommand = new AsyncRelayCommand(VerifyCodeAsync, () => !IsBusy);
        ResendCodeCommand = new AsyncRelayCommand(ResendCodeAsync, () => !IsBusy);
        BackToLoginCommand = new RelayCommand(BackToLogin);
    }

    public event EventHandler? LoginSucceeded;

    private string _emailOrUsername = string.Empty;
    public string EmailOrUsername
    {
        get => _emailOrUsername;
        set => SetProperty(ref _emailOrUsername, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private string _verificationCode = string.Empty;
    public string VerificationCode
    {
        get => _verificationCode;
        set => SetProperty(ref _verificationCode, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private string _successMessage = string.Empty;
    public string SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                LoginCommand.NotifyCanExecuteChanged();
                VerifyCodeCommand.NotifyCanExecuteChanged();
                ResendCodeCommand.NotifyCanExecuteChanged();
            }
        }
    }

    private bool _isVerificationMode;
    public bool IsVerificationMode
    {
        get => _isVerificationMode;
        set => SetProperty(ref _isVerificationMode, value);
    }

    public IAsyncRelayCommand LoginCommand { get; }
    public IAsyncRelayCommand VerifyCodeCommand { get; }
    public IAsyncRelayCommand ResendCodeCommand { get; }
    public IRelayCommand BackToLoginCommand { get; }

    private async Task LoginAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(EmailOrUsername))
            {
                ErrorMessage = "Email or username is required.";
                return;
            }

            if (!IsValidPassword(Password))
            {
                ErrorMessage = "Password must be at least 8 characters and contain uppercase, lowercase, number, and special character.";
                return;
            }

            var result = await _authService.LoginAsync(new AdminLoginRequestDto
            {
                EmailOrUsername = EmailOrUsername,
                Password = Password
            });

            if (result.RequiresVerification)
            {
                IsVerificationMode = true;
                SuccessMessage = "Verification code sent. Please enter the code.";
                return;
            }

            if (!result.Succeeded || string.IsNullOrWhiteSpace(result.Token))
            {
                ErrorMessage = result.Message;
                return;
            }

            await CompleteLoginAsync(result);
        }
        catch (Exception ex)
        {
            ErrorMessage = CleanApiError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task VerifyCodeAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(VerificationCode))
            {
                ErrorMessage = "Verification code is required.";
                return;
            }

            var result = await _authService.VerifyCodeAsync(new AdminVerifyCodeRequestDto
            {
                EmailOrUsername = EmailOrUsername,
                Code = VerificationCode
            });

            if (!result.Succeeded || string.IsNullOrWhiteSpace(result.Token))
            {
                ErrorMessage = result.Message;
                return;
            }

            await CompleteLoginAsync(result);
        }
        catch (Exception ex)
        {
            ErrorMessage = CleanApiError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ResendCodeAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(EmailOrUsername))
            {
                ErrorMessage = "Email or username is required.";
                return;
            }

            await _authService.ResendCodeAsync(new AdminResendCodeRequestDto
            {
                EmailOrUsername = EmailOrUsername
            });

            SuccessMessage = "Verification code resent.";
        }
        catch (Exception ex)
        {
            ErrorMessage = CleanApiError(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void BackToLogin()
    {
        IsVerificationMode = false;
        VerificationCode = string.Empty;
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;
    }

    private async Task CompleteLoginAsync(AdminLoginResponseDto result)
    {
        await _tokenStore.SaveTokenAsync(result.Token);

        _session.SetSession(result.Token, result.Admin);

        SuccessMessage = "Login successful.";

        LoginSucceeded?.Invoke(this, EventArgs.Empty);
    }

    private static bool IsValidPassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password)
            && password.Length >= 8
            && password.Any(char.IsUpper)
            && password.Any(char.IsLower)
            && password.Any(char.IsDigit)
            && password.Any(ch => !char.IsLetterOrDigit(ch));
    }

    private static string CleanApiError(string message)
    {
        return message
            .Replace("\"", string.Empty)
            .Replace("\\r", string.Empty)
            .Replace("\\n", string.Empty)
            .Trim();
    }
}