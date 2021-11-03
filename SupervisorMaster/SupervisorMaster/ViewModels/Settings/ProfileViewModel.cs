using Firebase.Auth;
using Firebase.Database.Query;
using Newtonsoft.Json;
using Plugin.Media;
using SupervisorMaster.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.UI.Dialogs;

namespace SupervisorMaster.ViewModels.Settings
{
    public class ProfileViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        #region Properties
        private User currentUser;
        private string password;
        private string image;
        public User CurrentUser { get => currentUser; set => SetProperty(ref currentUser, value); }
        public string Password { get => password; set => SetProperty(ref password, value); }
        public string Image { get => image; set => SetProperty(ref image, value); }


        #endregion

        #region Command 
        public ICommand PageAppearingCommand => new Command(async () =>
        {
            var loadingDialog = await MaterialDialog.Instance.LoadingDialogAsync(message: "Loading");
            IsBusy = true;

            try
            {
                var users = await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("User")
                    .OnceAsync<User>();
                var user = users.FirstOrDefault(x => x.Object.Email == User.Email);

                CurrentUser = user == null ? User : user.Object;
                Image = User.PhotoUrl == null ? "logo" : User.PhotoUrl;
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
                IsBusy = false;
                await loadingDialog.DismissAsync();
            }
        });
        public ICommand ImageCommand => new Command(async () =>
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });

                if (file == null)
                    return;
                IsBusy = true;
                var loadingDialog = await MaterialDialog.Instance.LoadingDialogAsync(message: "Waiting to upload");

                var imgurl = await FirebaseStorage
                    .Child(User.LocalId)
                    .Child("avanta.png")
                    .PutAsync(file.GetStream());

                Image = imgurl;
                var auth = await FirebaseAuthProvider.UpdateProfileAsync(FisebaseToken, CurrentUser.DisplayName, Image);
                User = auth.User;

                await loadingDialog.DismissAsync();
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
                IsBusy = false;
            }
        });
        public ICommand PasswordCommand => new Command(async () =>
        {
            await FirebaseAuthProvider.ChangeUserPassword(FisebaseToken, Password);
            await Shell.Current.GoToAsync($"..");

            await MaterialDialog.Instance.SnackbarAsync(message: "Change password success",
                                     msDuration: MaterialSnackbar.DurationLong);
        });
        public ICommand SaveCommand => new Command(async () =>
        {
            var users = await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("User").OnceAsync<User>();

            if (CurrentUser.DisplayName != null)
            {
                var auth = await FirebaseAuthProvider.UpdateProfileAsync(FisebaseToken, CurrentUser.DisplayName, Image);
                User = auth.User;
            }

            var user = users.FirstOrDefault(x => x.Object.Email == User.Email);
            if(user == null)
            {
                await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("User")
                    .PostAsync<User>(CurrentUser);
            }   
            else
            {
                await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("User")
                    .Child(user.Key)
                    .PutAsync<User>(CurrentUser);
            }    

            await MaterialDialog.Instance.SnackbarAsync(message: "Update profile success",
                                     msDuration: MaterialSnackbar.DurationLong);
        });

        #endregion

        public ProfileViewModel() : base("Profile")
        {
            
        }
    }
}
