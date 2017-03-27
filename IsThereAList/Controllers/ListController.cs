using IsThereAList.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IsThereAList.Controllers
{
    [Route("api/[controller]")]
    public class ListController: Controller
    {
        private readonly IsThereAListContext _cxt;
        private int _currentUserId = 0;
        private static DateTime _now = DateTime.UtcNow;
        private static DateTime _today = new DateTime(_now.Year, _now.Month, _now.Day);
        private static DateTime _activeDate = _today.AddDays(-28);

        public ListController(IsThereAListContext cxt)
        {
            _cxt = cxt;
        }

        [HttpGet("{listId}")]
        public List Get(int listId)
        {
            return _cxt
                .Lists
                .GetListAndListItems(listId, _currentUserId);
        }

        [HttpGet("[action]")]
        public IEnumerable<List> Active()
        {
            return _cxt
                .Lists
                .GetActiveLists(_currentUserId);
        }

        [HttpPut("{listId}")]
        public void Put(int listId, [FromBody] List list)
        {
            var listDb = _cxt.Lists.Find(listId);
            var owner = _cxt.Users.Find(_currentUserId);

            list.SetEffectiveDateAndName(list.ListType.Code, owner);
            _cxt.SaveChangesAsync();
        }

        [HttpDelete("{listId}")]
        public void Delete(int listId)
        {
            var list = _cxt
                .Lists
                .Include(i => i.ListItems)
                .Where(w => w.ListId == listId)
                .SingleOrDefault();

            if (list != null)
            {
                if (list.UserIdOwner != _currentUserId)
                    throw new Exception("You can only delete a list of your own!");

                foreach (var li in list.ListItems)
                    _cxt.ListItems.Remove(li);

                _cxt.Lists.Remove(list);
            }

            _cxt.SaveChanges();
        }
    }
}
