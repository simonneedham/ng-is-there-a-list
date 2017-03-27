using IsThereAList.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace IsThereAList.Controllers
{
    [Route("api/[controller]")]
    public class ListItemController : Controller
    {
        private readonly IsThereAListContext _cxt;
        private int _currentUserId = 0;

        public ListItemController(IsThereAListContext cxt)
        {
            _cxt = cxt;
        }

        [HttpGet("{listItemId}")]
        public ListItem Get(int listItemId)
        {
            return _cxt
                .ListItems
                .Include(i => i.List)
                .Include(i => i.UserPurchased)
                .Where(w => w.ListItemId == listItemId)
                .SingleOrDefault();
        }

        [HttpPost()]
        public void Post([FromBody] ListItem listItem)
        {
            var list = _cxt.Lists.GetList(listItem.ListId);

            if (list == null)
                throw new Exception(String.Format("A list with id {0} could not be found.", listItem.ListId));

            if (list.UserIdOwner != listItem.UserIdUpdated)
                throw new Exception("You can only add items to your own lists!");

            listItem.Updated = DateTime.UtcNow;
            listItem.UserIdUpdated = _currentUserId;

            _cxt.ListItems.Add(listItem);
            _cxt.SaveChangesAsync();
        }

        [HttpPut("{listItemId}")]
        public void Put(int listItemId, [FromBody] ListItem listItem)
        {
            var dbListItem = _cxt
                .ListItems
                .Where(w => w.ListItemId == listItemId)
                .SingleOrDefault();

            listItem.ListItemId = listItemId;

            _cxt.Entry(dbListItem).CurrentValues.SetValues(listItem);
            dbListItem.UserIdUpdated = _currentUserId;
            dbListItem.Updated = DateTime.UtcNow;

            _cxt.SaveChanges();
        }

        [HttpPut("{listItemId}/[Action]")]
        public void Purchase(int listItemId)
        {
            var listItem = _cxt
                .ListItems
                .Include(i => i.List)
                .Include(i => i.UserPurchased)
                .Where(w => w.ListItemId == listItemId)
                .SingleOrDefault();

            if (listItem.List.UserIdOwner == _currentUserId)
                throw new Exception("You can't claim presents from your own list! You crazy person.");

            if (listItem.HasBeenPurchased && listItem.UserPurchased.UserId != _currentUserId)
                throw new Exception(String.Format("This item has already been claimed by {0}. Hard cheese.", listItem.UserPurchased.FirstName));

            if (!listItem.UserIdPurchased.HasValue)
                listItem.UserIdPurchased = _currentUserId;
            else
                listItem.UserIdPurchased = null;

            listItem.UserIdUpdated = _currentUserId;
            _cxt.SaveChanges();
        }

        [HttpDelete("{listItemId}")]
        public void Delete(int listItemId)
        {
            var listItem = _cxt
                .ListItems
                .Include(i => i.List)
                .Where(w => w.ListItemId == listItemId)
                .SingleOrDefault();

            if (listItem == null)
                throw new Exception(String.Format("List Item {0} not found", listItemId));

            if (listItem.List.UserIdOwner != _currentUserId)
                throw new Exception("You can only delete items in your own lists! you crazy person.");

            listItem.Deleted = true;
            listItem.UserIdUpdated = _currentUserId;
            _cxt.SaveChanges();
        }
    }
}
