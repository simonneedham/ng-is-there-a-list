using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IsThereAList.Models
{
    public static class ListExtensions
    {
        private static DateTime _now = DateTime.UtcNow;
        private static DateTime _today = new DateTime(_now.Year, _now.Month, _now.Day);
        private static DateTime _activeDate = _today.AddDays(-28);

        public static IEnumerable<List> GetActiveLists(this DbSet<List> lists, int userIdCurrent)
        {
            return lists
                .Include(i => i.ListType)
                .Where(w => w.UserIdOwner != userIdCurrent &&
                            w.EffectiveDate >= _activeDate)
                .OrderByDescending(obd => obd.EffectiveDate)
                .OrderBy(ob => ob.Owner.FirstName)
                .ToList();
        }

        public static List GetList(this DbSet<List> lists, int listId)
        {
                return lists
                    .Include(i => i.ListType)
                    .Include(i => i.Owner)
                    .Where(w => w.ListId == listId)
                    .SingleOrDefault();
        }

        public static List GetListAndListItems(this DbSet<List> lists, int listId, int userIdCurrent)
        {
                return lists
                    .Include(i => i.ListItems)
                    .Include(i => i.ListItems.Select(li => li.UserPurchased))
                    .Where(
                        w => w.ListId == listId &&
                        (
                            w.UserIdOwner != userIdCurrent ||
                            (
                                (!w.ListItems.Any()) ||
                                (w.ListItems.Any(li => li.Deleted == false))
                            )
                        )
                    )
                    .SingleOrDefault();
        }

        public static List<List> GetOwnerLists(this DbSet<List> lists, int userIdOwner)
        {
                return lists
                    .Include(i => i.ListType)
                    .Where(w => w.UserIdOwner == userIdOwner &&
                                w.EffectiveDate >= _activeDate)
                    .OrderByDescending(obd => obd.EffectiveDate)
                    .OrderBy(ob => ob.Owner.FirstName)
                    .ToList();
        }
    }
}
