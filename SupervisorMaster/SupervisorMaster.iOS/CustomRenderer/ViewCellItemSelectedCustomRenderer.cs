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

[assembly: ExportRenderer(typeof(CustomViewCell), typeof(ViewCellItemSelectedCustomRenderer))]
namespace SupervisorMaster.iOS.CustomRenderer
{
    public class ViewCellItemSelectedCustomRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            return cell;
        }
    }
}