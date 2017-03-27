using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsThereAList.Models
{
    public static class DbInitializer
    {
        public static void Initialise(IsThereAListContext context)
        {
            if (!context.ListTypes.Any())
            {
                context.ListTypes.Add(new ListType { Code = "BDY", Name = "Birthday" });
                context.ListTypes.Add(new ListType { Code = "XMS", Name = "Christmas" });
                context.SaveChanges();
            }

            if(context.Users.SingleOrDefault(u => u.UserId == 0) == null)
            {
                var sql = @"set identity_insert dbo.[user] on;
                            insert into dbo.[user](UserId, FirstName, LastName, SendEmails, DobDay, DobMonth)
                            values(0, 'System', 'Account', 0, 1, 1);
                            set identity_insert dbo.[user] off;";

                context.Database.ExecuteSqlCommand(sql);
            }
        }
    }
}
