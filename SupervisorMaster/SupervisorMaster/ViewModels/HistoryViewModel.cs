using Firebase.Auth;
using Firebase.Database.Query;
using Newtonsoft.Json;
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
using XF.Material.Forms.UI.Dialogs;

namespace SupervisorMaster.ViewModels
{
    public class HistoryViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        #region Properties
        private string[] Colors = { "#B96CBD", "#49A24D", "#FDA838", "#F75355", "#00C6AE", "#455399", "#2c35d4" };
        private ObservableCollection<Agenda> agendas;

        public ObservableCollection<Agenda> Agendas { get => agendas; set => SetProperty(ref agendas, value); }
        #endregion

        #region Command 
        public ICommand LoadAgendasCommand { get; set; }

        public ICommand PageAppearingCommand => new Command(async () =>
        {
            IsBusy = true;
        });

        public ICommand RemoveCommand => new Command(async () =>
        {
            bool? wasOK = await MaterialDialog.Instance.ConfirmAsync("Do you want to remove all data?", "Warning");
            if (wasOK == true)
            {
                await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("Historys").DeleteAsync();

                Agendas?.Clear();
            }
        });
        #endregion

        public HistoryViewModel() : base("")
        {
            Init();
            /////////////////////////////////

            LoadAgendasCommand = new Command(async () => await ExecuteLoadAgendasCommand());
        }

       
        #region Method
        void Init()
        {
            Agendas = new ObservableCollection<Agenda>();
        }

        async Task ExecuteLoadAgendasCommand()
        {
            await LoadItemAsync();
        }

        private void AddAgenda(List<HistoryItemView> histories)
        {
            Agendas?.Clear();

            var groups = histories.GroupBy(x => x.Host);
            foreach (var group in groups)
            {
                var agenda = new Agenda();
                agenda.Historys = new ObservableCollection<HistoryItem>();

                DateTime start = new DateTime(9999, 1, 1), end = new DateTime(1000, 1, 1);
                foreach (var item in group.OrderBy(x => x.VisitedTime))
                {
                    agenda.Historys.Add(new HistoryItem
                    {
                        Title = item.Title,
                        Url = item.Url,
                        VisitedTime = item.VisitedTime,
                    });

                    if (start > item.VisitedTime)
                        start = item.VisitedTime;
                    if (end < item.VisitedTime)
                        end = item.VisitedTime;
                }

                agenda.Color = Colors[new Random().Next(0, 6)];
                agenda.DateStart = start;
                agenda.DateEnd = end;
                agenda.Topic = group.Key;
                agenda.Description = $"The number of website visits is {group.Count()}";

                if (agenda.Historys.Count != 0)
                    Agendas.Add(agenda);
            }
        }

        private async Task LoadItemAsync()
        {
            IsBusy = true;

            try
            {
                var lHistory = (await FirebaseDatabase
                    .Child(User.LocalId)
                    .Child("Historys")
                    .OnceAsync<HistoryItem>()).Select(x => new HistoryItemView
                    {
                        VisitedTime = x.Object.VisitedTime,
                        Title = x.Object.Title,
                        Url = x.Object.Url,
                        Host = new Uri(x.Object.Url).Host
                    });
                AddAgenda(lHistory.ToList());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
