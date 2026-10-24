using System;
using System.Globalization;
using UnityEngine;

namespace ShanHaiSpiritTrail
{
    // A time belongs to the score from the same completed run. Older score-only
    // records stay visible without inventing a matching completion time.
    public static class LevelRecord
    {
        private static string ScoreKey(bool innovation) => innovation ? "SpiritTrail.TrialBest" : "SpiritTrail.ClassicBest";
        private static string TimeKey(bool innovation) => ScoreKey(innovation) + ".Time";
        public static int Score(bool innovation) => PlayerPrefs.GetInt(ScoreKey(innovation), 0);

        public static double Seconds(bool innovation)
        {
            string value = PlayerPrefs.GetString(TimeKey(innovation), "");
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double seconds)
                && seconds >= 0 && !double.IsNaN(seconds) && !double.IsInfinity(seconds) ? seconds : -1;
        }

        public static string TimeText(bool innovation) => !PlayerPrefs.HasKey(ScoreKey(innovation))
            ? "00:00:00" : FormatTime(Seconds(innovation));

        public static bool Submit(bool innovation, int score, double seconds)
        {
            if (score < 0) throw new ArgumentOutOfRangeException(nameof(score));
            if (seconds < 0 || double.IsNaN(seconds) || double.IsInfinity(seconds))
                throw new ArgumentOutOfRangeException(nameof(seconds));
            int previousScore = Score(innovation);
            double previousTime = Seconds(innovation);
            if (score < previousScore || (score == previousScore && previousTime >= 0 && seconds >= previousTime)) return false;
            PlayerPrefs.SetInt(ScoreKey(innovation), score);
            PlayerPrefs.SetString(TimeKey(innovation), seconds.ToString("R", CultureInfo.InvariantCulture));
            PlayerPrefs.Save();
            return true;
        }

        public static string FormatTime(double seconds)
        {
            if (seconds < 0 || double.IsNaN(seconds) || double.IsInfinity(seconds)) return "--:--:--";
            long centiseconds = (long)Math.Floor(seconds * 100 + .000001);
            return (centiseconds / 6000).ToString("D2", CultureInfo.InvariantCulture) + ":"
                + (centiseconds / 100 % 60).ToString("D2", CultureInfo.InvariantCulture) + ":"
                + (centiseconds % 100).ToString("D2", CultureInfo.InvariantCulture);
        }
    }
}
