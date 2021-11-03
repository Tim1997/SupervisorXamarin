using Firebase.Auth;
using Firebase.Database.Query;
using Newtonsoft.Json;
using SupervisorMaster.Helpers;
using SupervisorMaster.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.UI;
using XF.Material.Forms.UI.Dialogs;
using XF.Material.Forms.UI.Dialogs.Configurations;

namespace SupervisorMaster.ViewModels.Settings
{
    public class WebsiteViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        #region Properties
        private ObservableCollection<WebsiteView> websites;
        public ObservableCollection<WebsiteView> Websites { get => websites; set => SetProperty(ref websites, value); }

        #endregion

        #region Command  
        public ICommand PageAppearingCommand => new Command(async () =>
        {
            await LoadItemAsync();
        });

        public ICommand AddWebsiteCommand => new Command(async () =>
        {
            await ShowDialog("Add website", async x =>
            {
                var web = new WebsiteView { Url = x };
                await FirebaseDatabase
                .Child(User.LocalId)
                .Child("Websites")
                .PostAsync(web);

                Websites.Add(web);
            });
        });

        public ICommand RemoveWebsiteCommand => new Command<string>(async (p) =>
        {
            var loadingDialog = await MaterialDialog.Instance.LoadingDialogAsync(message: "Loading");
            IsBusy = true;

            try
            {
                var web = (await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("Websites")
                    .OnceAsync<WebsiteView>()).FirstOrDefault(x => x.Object.Url == p);

                await FirebaseDatabase.Child(User.LocalId).Child("Websites").Child(web.Key).DeleteAsync();
                Websites.Remove(Websites.FirstOrDefault(x => x.Url == web.Object.Url));
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

        public ICommand EditWebsiteCommand => new Command<string>(async (old) =>
        {
            await ShowDialog("Edit website", async update =>
            {
                var web = (await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("Websites")
                    .OnceAsync<WebsiteView>()).FirstOrDefault(x => x.Object.Url == old);

                if (web == null) return;

                await FirebaseDatabase.Child(User.LocalId).Child("Websites").Child(web.Key)
                .PutAsync(new WebsiteView { Url = update });
                var item = Websites.FirstOrDefault(x => x.Url == old);
                if (item != null)
                {
                    item.Url = update;
                }
            }, old);
        });
        #endregion
        public WebsiteViewModel() : base("Website")
        {
            Websites = new ObservableCollection<WebsiteView>();
        }

        #region Method
        private async Task ShowDialog(string title, Action<string> action, string input = null)
        {
            var config = new MaterialInputDialogConfiguration()
            {
                InputType = MaterialTextFieldInputType.Url,
                BackgroundColor = Color.FromHex("#1b1b1b"),
            };

            var outer = await MaterialDialog.Instance.InputAsync(title: title,
                                                                 message: "Please enter url website you want block",
                                                                 inputText: input,
                                                                 inputPlaceholder: "Website block",
                                                                 configuration: config);
            if(!string.IsNullOrEmpty(outer))
            {
                if (!CommonHelper.UrlValidation(outer.Trim()))
                {
                    await MaterialDialog.Instance.SnackbarAsync(message: "Your url is wrong",
                                     msDuration: MaterialSnackbar.DurationLong);
                    return;
                }

                if (!Websites.Any(x => x.Url.Trim() == outer.Trim()))
                {
                    action?.Invoke(outer);
                }
                else
                {
                    await MaterialDialog.Instance.SnackbarAsync(message: "Url already exists",
                                     msDuration: MaterialSnackbar.DurationLong);
                }
            }


        }

        private void AddWebsite(List<WebsiteView> lwebsite)
        {
            Websites?.Clear();

            foreach (var web in lwebsite)
            {
                Websites.Add(web);
            }
        }

        private async Task LoadItemAsync()
        {
            var loadingDialog = await MaterialDialog.Instance.LoadingDialogAsync(message: "Loading");
            IsBusy = true;

            try
            {
                var lWebsite = (await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("Websites")
                    .OnceAsync<WebsiteView>()).Select(x => new WebsiteView
                    { Url = x.Object.Url, Id = x.Object.Id });
                AddWebsite(lWebsite.ToList());
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
        }
        #endregion
    }
}
