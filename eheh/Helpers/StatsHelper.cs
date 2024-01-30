using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace eheh.Helpers
{
    public static class StatsHelper
    {
        public static string SteamId
        {
            get;
            set;
        }

        public static string GetSteamId()
        {
            if (!string.IsNullOrEmpty(SteamId))
            {
                return SteamId;
            }
            try
            {
                string[] files = Directory.GetFiles(".\\players2\\", "*.stat");
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = files[i].Replace(".\\players2\\", string.Empty);
                }
                List<string> list = new List<string>();
                for (int j = 0; j < files.Length; j++)
                {
                    if (files[j].StartsWith("iw5"))
                    {
                        string[] array = files[j].Split("_".ToCharArray());
                        if (array.Length == 5)
                        {
                            list.Add(array[4].Substring(7, 8));
                        }
                    }
                }
                if (list.Count >= 1)
                {
                    using (Dictionary<string, int>.Enumerator enumerator = (from x in list
                                                                            group x by x).ToDictionary((IGrouping<string, string> g) => g.Key, (IGrouping<string, string> g) => g.Count()).GetEnumerator())
                    {
                        if (!enumerator.MoveNext())
                        {
                            return "0";
                        }
                        return enumerator.Current.Key;
                    }
                }
                return "0";
            }
            catch
            {
                return "0";
            }
        }
    }
}
