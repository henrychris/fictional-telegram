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
        public static async Task SeedUsers(DataContext context)
        {
            // check if database has any data
            if (await context.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeed.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            // create users of class AppUser
            foreach (var user in users)
            {
                context.Users.Add(user);
            }

            var epumpData = await File.ReadAllTextAsync("Data/EpumpDataSeed.json");
            var epumpDataList = JsonSerializer.Deserialize<List<EpumpData>>(epumpData);

            // create epump data of class EpumpData
            foreach (var item in epumpDataList)
            {
                context.EpumpData.Add(item);
                // Update the user table with the epump FK
                if (await context.Users.AnyAsync(x => x.ChatId == item.ChatId))
                {
                    var userToUpdate = await context.Users.FirstOrDefaultAsync(x => x.ChatId == item.ChatId);
                    userToUpdate.EpumpDataId = item.ID;
                }
            }

            await context.SaveChangesAsync();
        }
    }
}