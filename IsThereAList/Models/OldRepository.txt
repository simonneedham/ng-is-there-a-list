using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IsThereAList.Models
{
    public static class Repository
    {
        private static DateTime _now = DateTime.UtcNow;
        private static DateTime _today = new DateTime(_now.Year, _now.Month, _now.Day);
        private static DateTime _activeDate = _today.AddDays(-28);

        static Repository()
        {

        }

        public static async Task<IEnumerable<ApplicationUser>> GetUnauthorisedUsersAsync()
        {
            using (var db = new ApplicationDbContext())
            {
                var users = await db.Users
                                    //.Where(w => !w.Roles.Any(iur => iur.Role.Name == "Admin"))
                                    .Where(w => !w.Roles.Any(iur => iur.Role.Name == "Admin") &&
                                                !w.Roles.Any(iur => iur.Role.Name == "User"))
                                    .OrderBy(ob => ob.LastName)
                                    .ToListAsync();

                return users;
            }
        }

        public static async Task AuthoriseUsersAsync(string[] applicationUserIds)
        {
            using (var db = new ApplicationDbContext())
            {
                var userRole = await db.Roles.Where(w => w.Name == "User").SingleOrDefaultAsync();
                if(userRole == null) throw new AppException("Could not find User role. Database has not been seeded correctly.");

                var roleId = userRole.Id;

                var users = await db.Users.Include(i => i.Roles)
                                          .Where(w => applicationUserIds.Contains(w.Id) &&
                                                      !w.Roles.Any(iur => iur.Role.Name == "User") &&
                                                      !w.Roles.Any(iur => iur.Role.Name == "Admin"))
                                          .ToListAsync();

                foreach(var u in users)
                    u.Roles.Add(new IdentityUserRole { RoleId = roleId, UserId = u.Id });

                await db.SaveChangesAsync();
            }
        }

        public static async Task<IEnumerable<List>> GetActiveListsAsync(string currentUserId)
        {
            using (var db = new ApplicationDbContext())
            {
                return  await db.Lists.Include(i => i.ListType)
                                       .Where(w => w.OwnerId != currentUserId &&
                                                   w.EffectiveDate >= _activeDate)
                                       .OrderByDescending(obd => obd.EffectiveDate)
                                       .OrderBy(ob => ob.Owner.FirstName)
                                       .ToListAsync();
                                         
            }
        }

        public static async Task<List> GetListAsync(int listId)
        {
            using (var db = new ApplicationDbContext())
            {
                return await db.Lists.Include(i => i.ListType)
                                     .Include(i => i.Owner)
                                     .Where(w => w.ListId == listId)
                                     .SingleOrDefaultAsync();
            }
        }

        public static async Task<List> GetListAndListItemsAsync(int listId, string currentUserId)
        {
            using (var db = new ApplicationDbContext())
            {
                return await db.Lists.Include(i => i.ListItems)
                                     .Include(i => i.ListItems.Select(li => li.UserPurchased))
                                     .Where(
                                            w => w.ListId == listId &&
                                            (
                                                w.OwnerId != currentUserId || 
                                                (
                                                    (!w.ListItems.Any()) ||
                                                    (w.ListItems.Any(li => li.Deleted == false))
                                                )
                                            )
                                     )
                                     .SingleOrDefaultAsync();
            }
        }

        public static async Task<List<ListType>> GetListTypesNotCreatedAsync(string currentUserId)
        {
            using(var db = new ApplicationDbContext())
            {
                var owner = await db.Users.Where(w => w.Id == currentUserId).SingleOrDefaultAsync();

                var allListTypes = db.ListTypes.AsQueryable();

                var listTypesMadeThisYear = db.Lists.Where(w => w.OwnerId == currentUserId &&
                                                                w.EffectiveDate.Year == _today.Year)
                                                    .Select(s => s.ListType)
                                                    .Distinct()
                                                    .AsQueryable();

                var nextXmas = new DateTime(_today.Year, 12, 25);
                var nextBDay = new DateTime(_today.Year, owner.DobMonth, owner.DobDay);

                var listTypesExpired = db.ListTypes
                                         .Where(w => (w.Code=="XMY" && _today > nextXmas) ||
                                                     (w.Code=="BDY") && _today > nextBDay )
                                         .Distinct()
                                         .AsQueryable();

                var listTypesNotCreated = await allListTypes.Where(alt => !listTypesMadeThisYear.Any(ltmty => ltmty.ListTypeId == alt.ListTypeId) &&
                                                                    !listTypesExpired.Any(lte => lte.ListTypeId == alt.ListTypeId))
                                                            .ToListAsync();

                return listTypesNotCreated;
            }
        }

        public static async Task CreateNewListAsync(int listTypeId, ApplicationUser owner)
        {
            using(var db = new ApplicationDbContext())
            {
                var list = new List
                                {
                                    ListTypeId = listTypeId,
                                    OwnerId = owner.Id,
                                    Updated = DateTime.UtcNow
                                };

                var listType = await db.ListTypes.Where(w => w.ListTypeId == listTypeId).SingleOrDefaultAsync();

                list.SetEffectiveDateAndName(listType.Code, owner);

                db.Lists.Add(list);
                await db.SaveChangesAsync();
            }
        }

        public static async Task UpdateListAsync(int listId, int newListTypeId, ApplicationUser owner)
        {
            using (var db = new ApplicationDbContext())
            {
                var list = await db.Lists.FindAsync(listId);
                var listType = await db.ListTypes.Where(w => w.ListTypeId == newListTypeId).SingleOrDefaultAsync();

                list.SetEffectiveDateAndName(listType.Code, owner);
                await db.SaveChangesAsync();
            }
        }

        public static async Task DeleteListAsync(int id, string currentUserId)
        {
            using (var db = new ApplicationDbContext())
            {
                var list = await db.Lists.Include(i => i.ListItems)
                                         .Where(w => w.ListId == id)
                                         .SingleOrDefaultAsync();

                if(list != null)
                {
                    if (list.OwnerId != currentUserId)
                        throw new AppException("You can only delete a list of your own!");

                    foreach (var li in list.ListItems)
                        db.ListItems.Remove(li);

                    db.Lists.Remove(list);
                }

                await db.SaveChangesAsync();
            }
        }

        public static async Task<List<List>> GetOwnerListsAsync(string ownerUserId)
        {
            using (var db = new ApplicationDbContext())
            {
                return await db.Lists.Include(i => i.ListType)
                                       .Where(w => w.OwnerId == ownerUserId &&
                                                   w.EffectiveDate >= _activeDate)
                                       .OrderByDescending(obd => obd.EffectiveDate)
                                       .OrderBy(ob => ob.Owner.FirstName)
                                       .ToListAsync();

            }
        }

        public static async Task<ListItem> GetListItemAsync(int listItemId)
        {
            using( var db = new ApplicationDbContext())
            {
                return await db.ListItems.Include(i => i.List)
                                         .Include(i => i.UserPurchased)
                                         .Where(w => w.ListItemId == listItemId)
                                         .SingleOrDefaultAsync();
            }
        }

        public static async Task CreateNewListItemAsync(ListItem listItem, string currentUserId)
        {
            using( var db = new ApplicationDbContext())
            {
                var list = await GetListAsync(listItem.ListId);

                if (list == null)
                    throw new AppException(String.Format("A list with id {0} could not be found.", listItem.ListId));

                if (list.OwnerId != listItem.ApplicationUserIdUpdated)
                    throw new AppException("You can only add items to your own lists!");

                listItem.Updated = DateTime.UtcNow;
                listItem.ApplicationUserIdUpdated = currentUserId;

                db.ListItems.Add(listItem);
                await db.SaveChangesAsync();
            }
        }

        public static async Task UpdateListItemAsync(ListItem listItem, string currentUserId)
        {
            using( var db = new ApplicationDbContext())
            {
                var dbListItem = await db.ListItems.Where(w => w.ListItemId == listItem.ListItemId)
                                                   .SingleOrDefaultAsync();

                db.Entry(dbListItem).CurrentValues.SetValues(listItem);
                dbListItem.ApplicationUserIdUpdated = currentUserId;
                dbListItem.Updated = DateTime.UtcNow;

                await db.SaveChangesAsync();
            }
        }

        public static async Task DeleteListItemAsync(int listItemId, string currentUserId)
        {
            using( var db = new ApplicationDbContext())
            {
                var listItem = await db.ListItems.Include(i => i.List)
                                                 .Where(w => w.ListItemId == listItemId)
                                                 .SingleOrDefaultAsync();

                if (listItem == null)
                    throw new AppException(String.Format("List Item {0} not found", listItemId));

                if(listItem.List.OwnerId != currentUserId)
                    throw new AppException("You can only delete items in your own lists! you crazy person.");

                listItem.Deleted = true;
                listItem.ApplicationUserIdUpdated = currentUserId;
                await db.SaveChangesAsync();
            }
        }

        public static async Task PurchaseListItem(int listItemId, string currentUserId)
        {
            using( var db = new ApplicationDbContext())
            {
                var listItem = await db.ListItems.Include(i => i.List)
                                                 .Include(i => i.UserPurchased)
                                                 .Where(w => w.ListItemId == listItemId)
                                                 .SingleOrDefaultAsync();

                if (listItem.List.OwnerId == currentUserId)
                    throw new AppException("You can't claim presents from your own list! You crazy person.");

                if (listItem.HasBeenPurchased && listItem.UserPurchased.Id != currentUserId)
                    throw new AppException(String.Format("This item has already been claimed by {0}. Hard cheese.", listItem.UserPurchased.FirstName));

                if (String.IsNullOrEmpty(listItem.ApplicationUserIdPurchased))
                    listItem.ApplicationUserIdPurchased = currentUserId;
                else
                    listItem.ApplicationUserIdUpdated = null;

                listItem.ApplicationUserIdUpdated = currentUserId;
                await db.SaveChangesAsync();
            }
        }
    }
}