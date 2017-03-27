using IsThereAList.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsThereAList.Controllers
{
    [Route("api/[controller]")]
    public class ListTypeController
    {
        private readonly IsThereAListContext _cxt;

        public ListTypeController(IsThereAListContext cxt)
        {
            _cxt = cxt;
        }

        [HttpGet]
        public IEnumerable<ListType> Get()
        {
            return _cxt
                .ListTypes
                .AsNoTracking()
                .OrderBy(lt => lt.Name)
                .ToList();
        }

        [HttpGet("{listTypeId}")]
        public ListType Get(int listTypeId)
        {
            return _cxt
                .ListTypes
                .AsNoTracking()
                .Where(lt => lt.ListTypeId == listTypeId)
                .SingleOrDefault();
        }

        [HttpGet("[Action]")]
        public IEnumerable<ListType> NotCreated()
        {
            int currentUserId = 0;
            var today = DateTime.Today;

            var owner = _cxt.Users.Where(w => w.UserId == currentUserId).SingleOrDefault();

            var allListTypes = _cxt.ListTypes.AsQueryable();

            var listTypesMadeThisYear = _cxt
                .Lists
                .Where(w => w.UserIdOwner == currentUserId &&
                            w.EffectiveDate.Year == today.Year)
                .Select(s => s.ListType)
                .Distinct()
                .AsQueryable();

            var nextXmas = new DateTime(today.Year, 12, 25);
            var nextBDay = new DateTime(today.Year, owner.DobMonth, owner.DobDay);

            var listTypesExpired = _cxt.ListTypes
                                       .Where(w => (w.Code == "XMY" && today > nextXmas) ||
                                                   (w.Code == "BDY") && today > nextBDay)
                                       .Distinct()
                                       .AsQueryable();

            var listTypesNotCreated = allListTypes.Where(alt => !listTypesMadeThisYear.Any(ltmty => ltmty.ListTypeId == alt.ListTypeId) &&
                                                                !listTypesExpired.Any(lte => lte.ListTypeId == alt.ListTypeId))
                                                  .ToList();

            return listTypesNotCreated;
        }
    }
}
