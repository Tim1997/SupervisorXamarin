using Firebase.Database.Query;
using SupervisorMaster.Models;
using SupervisorMaster.ViewModels.Image;
using SupervisorMaster.Views.Image;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SupervisorMaster.ViewModels
{
    public class ImageViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        #region Properties
        private ObservableCollection<ImageView> imageViews;
        private double widthCard, heightCard;

        public ObservableCollection<ImageView> ImageViews { get => imageViews; set => SetProperty(ref imageViews, value); }
        public double WidthCard { get => widthCard; set => SetProperty(ref widthCard, value); }
        public double HeightCard { get => heightCard; set => SetProperty(ref heightCard, value); }
        #endregion

        #region Command 
        public ICommand LoadImageViewsCommand { get; set; }
        public ICommand PageAppearingCommand => new Command(async () =>
        {
            IsBusy = true;
        });

        public ICommand ImageInfoCommand => new Command<ImageView>(async (imageview) =>
        {
            await Shell.Current.GoToAsync($"{nameof(ImageInfoPage)}?{nameof(ImageInfoViewModel.ImageName)}={imageview.ImageName}");
        });
        #endregion

        public ImageViewModel() : base("Image")
        {
            Init();
            /////////////////////////////
            LoadImageViewsCommand = new Command(async () => await ExecuteLoadImageViewsCommand());
        }

        #region Method
        void Init()
        {
            ImageViews = new ObservableCollection<ImageView>();
            var width = Application.Current.MainPage.Width;
            HeightCard = WidthCard = width * 0.35;
        }

        async Task ExecuteLoadImageViewsCommand()
        {
            IsBusy = true;

            try
            {
                ImageViews?.Clear();

                //var limage = (await FirebaseDatabase
                //    .Child(User.LocalId)
                //    .Child("Images")
                //    .OnceAsync<ImageView>()).Select(x => new ImageView
                //    {
                //        Id = x.Object.Id,
                //        ImageName = x.Object.ImageName,
                //        ImageUrl = x.Object.ImageUrl,
                //        UploadTime = x.Object.UploadTime,
                //    });

                //foreach (var item in limage)
                //{
                //    ImageViews.Add(item);
                //}

                for (int i = 0; i < 1; i++)
                {
                    ImageViews.Add(new ImageView
                    {
                        Id = Guid.NewGuid().ToString(),
                        ImageName = i.ToString(),
                        ImageUrl = User.PhotoUrl,
                        UploadTime = DateTime.Now,
                    });
                }

                var x = ImageViews[0].ImageUrl;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
            #endregion
        }
    }
}
