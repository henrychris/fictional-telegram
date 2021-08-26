using System.Threading.Tasks;
using app.Entities;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
// using Newtonsoft.Json;

namespace app.Data
{
    public class Seed
    {
        public static async Task SeedDataBase(DataContext context)
        {
            // check if database has any data
            if (await context.Users.AnyAsync()) return;

            await SeedUsers(context);
            await SeedEpumpData(context);
            await SeedLoginData(context);

            await context.SaveChangesAsync();
        }

        public static async Task SeedUsers(DataContext context)
        {
            var userData = await File.ReadAllTextAsync("Data/UserSeed.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            // create users of class AppUser
            foreach (var user in users)
            {
                await context.Users.AddAsync(user);
            }
            await context.SaveChangesAsync();
        }

        public static async Task SeedEpumpData(DataContext context)
        {
            var epumpData = await File.ReadAllTextAsync("Data/EpumpDataSeed.json");
            var epumpDataList = JsonSerializer.Deserialize<List<EpumpData>>(epumpData);

            // create epump data of class EpumpData
            for (int i = 0; i < epumpDataList.Count; i++)
            {
                EpumpData item = epumpDataList[i];
                await context.EpumpData.AddAsync(item);

                // Update the user table with the epump FK
                var check = await context.Users.AnyAsync(u => u.ChatId == item.ChatId);
                if (check)
                {
                    var userToUpdate = await context.Users.FirstOrDefaultAsync(x => x.ChatId == item.ChatId);
                    userToUpdate.EpumpDataId = item.ID;
                }
            }
        }

        public static async Task SeedLoginData(DataContext context)
        {
            var loginData = await File.ReadAllTextAsync("Data/TelegramLogin.json");
            var loginStatusList = JsonSerializer.Deserialize<List<LoginStatusTelegram>>(loginData);

            // create login status of class LoginStatus
            foreach (var item in loginStatusList)
            {
                await context.loginStatusTelegram.AddAsync(item);
            }

            var loginData2 = await File.ReadAllTextAsync("Data/EpumpLogin.json");
            var loginStatusList2 = JsonSerializer.Deserialize<List<LoginStatusEpump>>(loginData2);

            // create login status of class LoginStatus
            foreach (var item in loginStatusList2)
            {
                await context.loginStatusEpump.AddAsync(item);
            }
        }
    }
}