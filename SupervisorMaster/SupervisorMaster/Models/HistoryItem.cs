using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SupervisorMaster.Models
{
    public class Agenda
    {
        public string Topic { get; set; }
        public string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public ObservableCollection<HistoryItem> Historys { get; set; }
        public string Color { get; set; }
    }

    public class HistoryItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime VisitedTime { get; set; }

    }

    public class HistoryItemView : HistoryItem
    {
        public string Host { get; set; }
    }

    public class SettingItem
    {
        public string Name { get; set; }
        public string Image { get; set; }
    }
}
