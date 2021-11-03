using Firebase.Auth;
using Firebase.Database.Query;
using SupervisorMaster.Models;
using SupervisorMaster.ViewModels.Settings;
using SupervisorMaster.Views;
using SupervisorMaster.Views.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.UI;
using XF.Material.Forms.UI.Dialogs;

namespace SupervisorMaster.ViewModels
{
    public class SettingsViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        #region Properties
        private User currentUser;
        public User CurrentUser { get => currentUser; set => SetProperty(ref currentUser, value); }

        #endregion

        #region Command 
        public ICommand PageAppearingCommand => new Command(() =>
        {
            CurrentUser = User;

            if (CurrentUser.PhotoUrl == null)
                CurrentUser.PhotoUrl = "logo";
            if (CurrentUser.DisplayName == null)
                CurrentUser.DisplayName = CurrentUser.Email;
        });
        public ICommand EditProfileCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync(nameof(ProfilePage));
        });
        public ICommand TimerCommand => new Command(async () =>
        {
            try
            {
                var timers = await FirebaseDatabase.Child(User.LocalId)
                    .Child("Timer")
                    .OnceAsync<TimerView>();
                var timer = timers.FirstOrDefault(x => x.Object.Email == User.Email).Object;

                var response = await TimerPage.ShowDialog(Shell.Current.Navigation, timer);
                if (response.Result)
                {
                    var isClock = bool.Parse(response.Objs[0].ToString());
                    var clock = TimeSpan.Parse(response.Objs[1].ToString());
                    var minute = int.Parse(response.Objs[2].ToString());
                    var isOnce = bool.Parse(response.Objs[3].ToString());
                    UploadTimer(new TimerView { Clock = clock, Email = User.Email, IsClock = isClock, IsOnce = isOnce, Minute = minute });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        });
        public ICommand ScreenshotCommand => new Command(async () =>
        {
            var values = new int[] { 0, 5, 15, 30 };
            var choices = new string[]
            {
                "None",
                "5 minutes",
                "15 minutes",
                "30 minutes"
            };
            var view = new MaterialRadioButtonGroup()
            {
                Choices = choices,
            };

            var screenshots = await FirebaseDatabase.Child(User.LocalId)
                    .Child("Screenshot")
                    .OnceAsync<ScreenshotView>();

            var screenshot = screenshots.FirstOrDefault(x => x.Object.Email == User.Email).Object;
            if (screenshot != null)
                view.SelectedIndex = values.ToList().FindIndex(x => x == screenshot.Time);

            bool? wasConfirmed = await MaterialDialog.Instance.ShowCustomContentAsync(view, "How long do you want to automatically take screenshots?", "Screenshot");
            if(wasConfirmed == true)
                UploadScreenshot(values[view.SelectedIndex]);
            
        });
        public ICommand WebsiteCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync(nameof(WebsitePage));
        });
        public ICommand LogoutCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync("//LoginPage");
        });

        #endregion

        public SettingsViewModel() : base("")
        {

        }

        #region Firebase
        public async void UploadTimer(TimerView model)
        {
            var timerviews = await FirebaseDatabase.Child(User.LocalId)
                    .Child("Timer").OnceAsync<TimerView>();

            var tview = timerviews.FirstOrDefault(x => x.Object.Email == User.Email);

            if (tview == null)
            {
                await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("Timer")
                    .PostAsync<TimerView>(model);
            }
            else
            {
                await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("Timer")
                    .Child(tview.Key)
                    .PutAsync<TimerView>(model);
            }
        }

        public async void UploadScreenshot(int timescreenshot)
        {
            var screenshotviews = await FirebaseDatabase.Child(User.LocalId)
                    .Child("Screenshot").OnceAsync<ScreenshotView>();

            var scview = screenshotviews.FirstOrDefault(x => x.Object.Email == User.Email);

            if (scview == null)
            {
                await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("Screenshot")
                    .PostAsync<ScreenshotView>(new ScreenshotView { Time = timescreenshot, Email = User.Email });
            }
            else
            {
                await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("Screenshot")
                    .Child(scview.Key)
                    .PutAsync<ScreenshotView>(new ScreenshotView { Time = timescreenshot, Email = User.Email });
            }
        }
        #endregion
    }
}
