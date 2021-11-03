using Firebase.Auth;
using Newtonsoft.Json;
using SupervisorMaster.Helpers;
using SupervisorMaster.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace SupervisorMaster.ViewModels
{
    public class RegisterViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        #region Properties
        private string email;
        private string password;
        private string errorEmail;
        private string errorPassword;
        private string displayName;
        private bool isFirstRun;
        private bool isErrorEmail;
        private bool isErrorPassword;

        public string Email
        {
            get => email;
            set
            {
                SetProperty(ref email, value);
                IsErrorEmail = string.IsNullOrEmpty(email);
                if (isFirstRun) IsErrorEmail = false;
                if (IsErrorEmail) ErrorEmail = "Email can not be empty";
            }
        }
        public string Password
        {
            get => password;
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
        public string DisplayName { get => displayName; set => SetProperty(ref displayName, value); }

        #endregion  

        #region Command
        public ICommand SignupCommand => new Command(async () =>
        {
            if (IsErrorPassword || IsErrorEmail)
                return;

            if (!isFirstRun && !IsErrorEmail && !CommonHelper.EmailValidation(Email))
            {
                ErrorEmail = "Your email must be in abcd@gmail.com format";
                IsErrorEmail = true;
                return;
            }

            var loadingDialog = await MaterialDialog.Instance.LoadingDialogAsync(message: "Waiting to sign up");
            IsBusy = true;

            try
            {
                var auth = await FirebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(Email, Password, DisplayName, true);
                var content = await auth.GetFreshAuthAsync();
                await Shell.Current.GoToAsync($"..");
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
        #endregion

        public RegisterViewModel() : base("Sign up")
        {
            isFirstRun = true;
        }
    }
}
