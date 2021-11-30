using Firebase.Auth;
using Newtonsoft.Json;
using SupervisorMaster.Helpers;
using SupervisorMaster.Models;
using SupervisorMaster.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using XF.Material.Forms.UI;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace SupervisorMaster.ViewModels
{
    public class LoginViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        #region Properties
        private string email;
        private string password;
        private string errorEmail;
        private string errorPassword;
        private bool isFirstRun;
        private bool isErrorEmail;
        private bool isErrorPassword;

        public string Email { get => email; 
            set 
            { 
                SetProperty(ref email, value);
                IsErrorEmail = string.IsNullOrEmpty(email);
                if (isFirstRun) IsErrorEmail = false;
                if (IsErrorEmail) ErrorEmail = "Email can not be empty";
            } 
        }
        public string Password { get => password; 
            set 
            { 
                SetProperty(ref password, value); IsErrorPassword = string.IsNullOrEmpty(password); 
                if (isFirstRun) { isFirstRun = false; IsErrorPassword = false; }
                if (IsErrorPassword) ErrorPassword = "Password can not be empty";
            } 
        }
        public string ErrorEmail { get => errorEmail; set => SetProperty(ref errorEmail, value); }
        public string ErrorPassword { get => errorPassword; set => SetProperty(ref errorPassword, value); }
        public bool IsErrorEmail { get => isErrorEmail; set => SetProperty(ref isErrorEmail, value); }
        public bool IsErrorPassword { get => isErrorPassword; set => SetProperty(ref isErrorPassword, value); }
        #endregion  

        #region Command
        public ICommand LoginCommand => new Command(async () =>
        {
            if (IsErrorPassword || IsErrorEmail)
                return;

            if (!isFirstRun && !IsErrorEmail && !CommonHelper.EmailValidation(Email))
            {
                ErrorEmail = "Your email must be in abcd@gmail.com format";
                IsErrorEmail = true;
                return;
            }

            var loadingDialog = await MaterialDialog.Instance.LoadingDialogAsync(message: "Waiting to login");
            IsBusy = true;

            try
            {
                var auth = await FirebaseAuthProvider.SignInWithEmailAndPasswordAsync(Email, Password);
                var content = await auth.GetFreshAuthAsync();
                if (!content.User.IsEmailVerified)
                {
                    var json = JsonConvert.SerializeObject(new ResponseFirebase { error = new Error { message = "Email is not verified" } });
                    throw new FirebaseAuthException(null, null, json, null);
                }

                FisebaseToken = content.FirebaseToken;
                User = content.User;
                await Shell.Current.GoToAsync($"//{nameof(HistoryPage)}");
                //await Shell.Current.GoToAsync($"//{nameof(ImagePage)}");
            }
            catch (FirebaseAuthException ex)
            {
                if(ex.ResponseData == "N/A")
                    await MaterialDialog.Instance.SnackbarAsync(message: "Internet connection error",
                                     msDuration: MaterialSnackbar.DurationLong);
                else
                {
                    var response = JsonConvert.DeserializeObject<ResponseFirebase>(ex.ResponseData);

                    await MaterialDialog.Instance.SnackbarAsync(message: response.error.message,
                                         msDuration: MaterialSnackbar.DurationLong);
                }    
            }
            finally
            {
                await loadingDialog.DismissAsync();
                IsBusy = false;
            }
        });

        public ICommand ForgetPasswordCommand => new Command(async () =>
        {
            var loadingDialog = await MaterialDialog.Instance.LoadingDialogAsync(message: "Loading");
            IsBusy = true;

            try
            {
                await ShowDialog("Reset password", async (outer) =>
                {
                    await FirebaseAuthProvider.SendPasswordResetEmailAsync(Email);
                });
                
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.ResponseData == "N/A")
                    await MaterialDialog.Instance.SnackbarAsync(message: "Internet connection error",
                                     msDuration: MaterialSnackbar.DurationLong);
                else
                {
                    var response = JsonConvert.DeserializeObject<ResponseFirebase>(ex.ResponseData);

                    await MaterialDialog.Instance.SnackbarAsync(message: response.error.message,
                                         msDuration: MaterialSnackbar.DurationLong);
                }
            }
            finally
            {
                await loadingDialog.DismissAsync();
                IsBusy = false;
            }
        });

        public ICommand SignupCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync($"{nameof(RegisterPage)}");
        });
        #endregion

        public LoginViewModel() : base("Login")
        {
            Shell.Current.FlyoutIsPresented = false;
            isFirstRun = true;

            Email = "timbkhn@gmail.com";
            Password = "123456";
        }

        #region Method
        private async Task ShowDialog(string title, Action<string> action, string input = null)
        {
            var config = new MaterialInputDialogConfiguration()
            {
                InputType = MaterialTextFieldInputType.Email,
                BackgroundColor = Color.FromHex("#1b1b1b"),
            };

            var outer = await MaterialDialog.Instance.InputAsync(title: title,
                                                                 message: "Please write email to receive password reset",
                                                                 inputText: input,
                                                                 inputPlaceholder: "Email",
                                                                 configuration: config);
            if (!string.IsNullOrEmpty(outer))
            {
                if (!CommonHelper.EmailValidation(outer.Trim()))
                {
                    await MaterialDialog.Instance.SnackbarAsync(message: "Wrong email format",
                                     msDuration: MaterialSnackbar.DurationLong);
                    return;
                }

                action?.Invoke(outer);
            }
        }
        #endregion
    }
}
