using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp.Data
{
    public enum Summ
    {
        voicetime = 0,
        unmute_time = 1,
        video_time = 2,
        users_15time = 3,
        users_4time = 4,
        afk_time = 5
    }
    public enum Ammount
    {
        reactions = 6,
        messages = 7
    }
    public class UserService
    {
        public UserService() { }

        public async Task<DateTimeOffset> GetMinDate()
        {
            bool init = false;
            DateTime minTime = DateTime.Now.AddDays(-1);
            foreach (FileInfo file in new DirectoryInfo(Path.Combine(Startup.Root, "Json")).EnumerateFiles())
            {

                if (file.Name.Split('_')[0].ParseToDate(out DateTime time))
                {
                    if (minTime > time || !init)
                    {
                        minTime = time;
                        init = true;
                    }
                }
            }
            return minTime;
        }
        public async Task<DateTimeOffset> GetMaxDate()
        {
            bool init = false;
            DateTime maxTime = DateTime.Now.AddDays(3);
            foreach (FileInfo file in new DirectoryInfo(Path.Combine(Startup.Root, "Json")).EnumerateFiles())
            {

                if (file.Name.Split('_')[0].ParseToDate(out DateTime time))
                {
                    if (maxTime < time || !init) 
                    { 
                        maxTime = time;
                        init = true;
                    }
                }
            }
            return maxTime;
        }
        public async Task<UserRow[]> GetData(DateTimeOffset? from, DateTimeOffset? to, int value)
        {
            UserRow[] data;
            Dictionary<string, UserData[]> users = await GetUsers(from, to);
            if (value < 0 || value > 9) throw new ArgumentOutOfRangeException("value is out of range [0-9]");
            else if (value < 8) data = value < 6 ? GetSummData(users, (Summ)value) : GetAmmountData(users, (Ammount)value);
            else data = value == 8 ? GetRelationalData(users) : GetPercentageData(users);
            data = data.OrderBy(x => x.PbClass).ThenBy(x => x.Index).Reverse().ToArray();
            return data;
        }

        private async Task<Dictionary<string, UserData[]>> GetUsers(DateTimeOffset? from, DateTimeOffset? to)
        {
            DateTimeOffset _from = from == null ? DateTimeOffset.MinValue : (DateTimeOffset)from;
            DateTimeOffset _to = to == null ? DateTimeOffset.MaxValue : (DateTimeOffset)to;
            Dictionary<string, List<UserData>> data = new Dictionary<string, List<UserData>>();
            foreach (FileInfo file in new DirectoryInfo(Path.Combine(Startup.Root, "Json")).EnumerateFiles())
            {
                if(file.Name.Split('_')[0].ParseToDate(out DateTime time))
                {
                    if(time >= _from && time <= _to)
                    {
                        UserData[]  coll = await ReadUserData(file);
                        foreach(UserData ud in coll)
                        {
                            ud.DataTime = time;
                            if(!data.TryGetValue(ud.Username, out _))
                            {
                                data.Add(ud.Username, new List<UserData>() { ud });
                                continue;
                            }
                            data[ud.Username].Add(ud);
                        }
                    }
                }
            }
            Dictionary<string, UserData[]> users = new Dictionary<string, UserData[]>();
            foreach(string key in data.Keys)
            {
                users.Add(key, data[key].ToArray());
            }
            return users;
        }
        private UserRow[] GetAmmountData(Dictionary<string, UserData[]> users, Ammount value)
        {
            UserRow[] rows = new UserRow[users.Keys.Count];
            int i = 0;
            foreach(string key in users.Keys)
            {
                UserRow row = new UserRow(key);
                foreach(UserData ud in users[key])
                {
                    switch (value)
                    {
                        case Ammount.messages:
                            row.Progress += ud.Messages.Count();
                            break;
                        case Ammount.reactions:
                            row.Progress += ud.Reactions;
                            break;
                    }
                }
                Console.WriteLine(row.Progress);
                rows[i++] = row;
            }
            double max = rows.Select(x => x.Progress).OrderBy(x => x).Last();
            if (max == 0) max = 100;
            foreach (UserRow row in rows)
            {
                row.Description = $"{row.Progress} ед.";
                row.Index = row.Progress * 100 / max;
                row.Progress = Math.Round(row.Progress * 100 / max);
                Console.WriteLine($"Ammount - max: {max} progress: {row.Progress}");
            }
            return rows;
        }
        private UserRow[] GetSummData(Dictionary<string, UserData[]> users, Summ value )
        {
            UserRow[] rows = new UserRow[users.Keys.Count];
            int i = 0;
            foreach (string key in users.Keys)
            {
                UserRow row = new UserRow(key);
                foreach (UserData ud in users[key])
                {
                    switch (value)
                    {
                        case Summ.afk_time:
                            row.Progress += ud.AfkTime;
                            break;
                        case Summ.unmute_time:
                            row.Progress += ud.UnmuteTime;
                            break;
                        case Summ.users_15time:
                            row.Progress += ud.Users15time;
                            break;
                        case Summ.users_4time:
                            row.Progress += ud.Users4time;
                            break;
                        case Summ.video_time:
                            row.Progress += ud.VideoTime;
                            break;
                        case Summ.voicetime:
                            row.Progress += ud.Voicetime;
                            break;
                    }
                }
                Console.WriteLine(row.Progress);
                rows[i++] = row;
            }
            double max = rows.Select(x => x.Progress).OrderBy(x => x).Last();
            if (max == 0) max = 100;
            foreach (UserRow row in rows)
            {
                int hours = Math.Abs(row.Progress) >= 60 ? (int)Math.Floor(Math.Abs(row.Progress) / 60) : 0;
                int minutes = (int)(Math.Abs(row.Progress) >= 60 ? Math.Ceiling(Math.Abs(row.Progress) % 60) : Math.Abs(row.Progress));
                string mm = minutes > 9 ? $"{minutes}" : $"0{minutes}";
                string hh = hours > 9 ? $"{hours}" : $"0{hours}";
                row.Description = $"{hh}:{mm}";
                row.Index = Math.Round(row.Progress * 100 / max);
                row.Progress = Math.Round(Math.Abs(row.Progress) * 100 / max);
            }
            return rows;
        }
        private UserRow[] GetRelationalData(Dictionary<string, UserData[]> users)
        {
            UserRow[] rows = new UserRow[users.Keys.Count];
            int i = 0;
            foreach (string key in users.Keys)
            {
                UserRow row = new UserRow(key);
                List<double> d = new List<double>();
                foreach (UserData ud in users[key])
                {
                    d.Add((new DateTime(ud.FirstConnection.Year, ud.FirstConnection.Month, ud.FirstConnection.Day, 12, 0, 0) - ud.FirstConnection).TotalMinutes);
                }
                row.Progress = d.Sum() / d.Count;
                Console.WriteLine(row.Progress);
                rows[i++] = row;
            }
            double max = rows.Select(x => Math.Abs(x.Progress)).OrderBy(x => x).Last();
            if (max == 0) max = 100;
            foreach (UserRow row in rows)
            {
                int hours = Math.Abs(row.Progress) >= 60 ? (int)Math.Floor(Math.Abs(row.Progress) / 60) : 0;
                int minutes = (int)(Math.Abs(row.Progress) >= 60 ? Math.Ceiling(Math.Abs(row.Progress) % 60) : Math.Abs(row.Progress));
                string mm = minutes > 9 ? $"{minutes}" : $"0{minutes}";
                string hh = hours > 9 ? $"{hours}" : $"0{hours}";
                if (row.Progress > 0) row.PbClass = "progress-bar bg-danger";
                row.Description = (row.Progress > 0 ? "" : "-") + (hours == 0 ? $"{mm} минут" : $"{hh}:{mm}");
                row.Index = -Math.Round(row.Progress * 100 / max);
                row.Progress = Math.Round(Math.Abs(row.Progress) * 100 / max);
            }
            return rows;
        }
        private UserRow[] GetPercentageData(Dictionary<string, UserData[]> users)
        {
            UserRow[] rows = new UserRow[users.Keys.Count];
            int i = 0;
            foreach (string key in users.Keys)
            {
                UserRow row = new UserRow(key);
                foreach (UserData ud in users[key])
                {
                    row.Progress += ud.MidDayActivity;
                }
                rows[i++] = row;
                Console.WriteLine(row.Progress);
            }
            double max = 15;
            foreach (UserRow row in rows)
            {
                row.Description = $"{row.Progress}%";
                row.Index = Math.Round(row.Progress * 100 / max);
                row.Progress = Math.Round(row.Progress * 100 / max);
                Console.WriteLine($"Percentage - max: {max} progress: {row.Progress}");
            }
            return rows;
        }
        private async Task<UserData[]> ReadUserData(FileInfo file)
        {
            List<UserData> users = new List<UserData>();
            string[] parts = file.Name.Split('-');
            WeirdDateSerializer.ReadDate = new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2].Split('_')[0]));
            using (FileStream stream = file.OpenRead())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    users.AddRange(JsonConvert.DeserializeObject<JsonCollection>(await reader.ReadToEndAsync()).Collection);
                }
            }
            return users.ToArray();
        }
    }
}
