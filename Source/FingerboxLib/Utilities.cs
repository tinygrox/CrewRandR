using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.Localization;

namespace CrewRandR
{
    public class Utilities
    {
        public static double GetDayLength
        {
            get
            {
                if (GameSettings.KERBIN_TIME)
                {
                    return 21600;
                }
                else
                {
                    return 86400;
                }
            }
        }

        // Taken from KCT - TODO - Talk to Magico13 about attribution?
        public static string GetColonFormattedTime(double time)
        {
            if (time > 0)
            {
                StringBuilder formatedTime = new StringBuilder();
                formatedTime.AppendFormat("{0,2:00}:", Math.Floor(time / GetDayLength));
                time = time % GetDayLength;
                formatedTime.AppendFormat("{0,2:00}:", Math.Floor(time / 3600));
                time = time % 3600;
                formatedTime.AppendFormat("{0,2:00}:", Math.Floor(time / 60));
                time = time % 60;
                formatedTime.AppendFormat("{0,2:00}", time);

                return formatedTime.ToString();
            }
            else
            {
                return "00:00:00:00";
            }
        }

        public static string GetFormattedTime(double time)
        {
            if (time > 0)
            {
                StringBuilder formatedTime = new StringBuilder();
                formatedTime.Append(Localizer.Format("#CrewRR_TimeFormat_Days", String.Format("{0,2:0}", Math.Floor(time / GetDayLength))) + " "); // .AppendFormat("{0,2:0} days, ", Math.Floor(time / GetDayLength));
                time = time % GetDayLength;
                formatedTime.Append(Localizer.Format("#CrewRR_TimeFormat_Hours", String.Format("{0,2:0}", Math.Floor(time / 3600))) + " "); // AppendFormat("{0,2:0} hours, ", Math.Floor(time / 3600));
                time = time % 3600;
                formatedTime.Append(Localizer.Format("#CrewRR_TimeFormat_Minutes", String.Format("{0,2:0}", Math.Floor(time / 60))) + " "); // AppendFormat("{0,2:0} minutes, ", Math.Floor(time / 60));
                time = time % 60;
                formatedTime.Append(Localizer.Format("#CrewRR_TimeFormat_Seconds", String.Format("{0,2:0}", time))); // AppendFormat("{0,2:0} seconds", time);

                return formatedTime.ToString();
            }
            else
            {
                return Localizer.Format("#CrewRR_TimeFormat_Default"); // "0 days,  0 hours,  0 minutes,  0 seconds"
            }
        }
    }
}
