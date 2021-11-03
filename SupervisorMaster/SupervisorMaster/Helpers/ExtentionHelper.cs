using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SupervisorMaster.Helpers
{
    public static class ExtentionHelper
    {
        static readonly CultureInfo cul = CultureInfo.GetCultureInfo("en-US");

        public static string ToMoney(this int number)
        {
            return number.ToString("#,###", cul.NumberFormat);
        }
    }
}
