using System;
using System.Globalization;
using UnityEngine;

namespace Gazeus.Match3Challenge.Project.Scripts.Helpers
{
    public static class TimerHelpers
    {
        public static string FormatTime(this float seconds, bool showMilliseconds = false)
        {
            seconds = Mathf.Max(0f, seconds);
            var ts = TimeSpan.FromSeconds(seconds);

            if (showMilliseconds)
            {
                return ts.TotalHours >= 1
                    ? ts.ToString(@"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture)
                    : ts.ToString(@"mm\:ss\.fff", CultureInfo.InvariantCulture);
            }

            return ts.TotalHours >= 1
                ? ts.ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture)
                : ts.ToString(@"mm\:ss", CultureInfo.InvariantCulture);
        }
    }
}