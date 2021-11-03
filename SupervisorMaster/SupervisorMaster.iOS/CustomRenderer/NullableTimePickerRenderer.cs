using Foundation;
using SupervisorMaster.Controls;
using SupervisorMaster.iOS.CustomRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NullableTimePicker), typeof(NullableTimePickerRenderer))]
namespace SupervisorMaster.iOS.CustomRenderer
{
	public class NullableTimePickerRenderer : TimePickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null && this.Control != null)
			{
				AddClearButton();
			}
		}

		private void AddClearButton()
		{
			var originalToolbar = this.Control.InputAccessoryView as UIToolbar;

			if (originalToolbar != null && originalToolbar.Items.Length <= 2)
			{
				var clearButton = new UIBarButtonItem("Clear", UIBarButtonItemStyle.Plain, ((sender, ev) =>
				{
					Element.Unfocus();
					Element.Time = TimeSpan.Zero;
					(this.Element as NullableTimePicker).ClearTime();
				}));

				var newItems = new List<UIBarButtonItem>();
				foreach (var item in originalToolbar.Items)
				{
					newItems.Add(item);
				}

				newItems.Insert(0, clearButton);

				originalToolbar.Items = newItems.ToArray();
			}
		}
	}
}