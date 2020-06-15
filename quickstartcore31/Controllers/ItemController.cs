

namespace quickstartcore31.Controllers
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models;

    public class ItemController : Controller
    {
        private readonly IDocumentDBRepository<Models.Item> respository;
        private readonly ILogger<ItemController>  _logger;

        public ItemController(IDocumentDBRepository<Models.Item> respository, ILogger<ItemController> logger)
        {
            this.respository = respository;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            Expression<Func<Item, bool>> filterCompleted = i => !i.Completed;
            Expression<Func<Item, bool>> all = (i) => true;
            var items = await respository.GetItemsAsync(all);
            _logger.LogInformation($"Fetching items... ");
            return View(items);
        }


#pragma warning disable 1998
        [ActionName("Create")]
        public async Task<IActionResult> CreateAsync()
        {
            return View();
        }
#pragma warning restore 1998

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("Id,Name,Description,Completed")] Item item)
        {
            if (ModelState.IsValid)
            {
                item.Id = Guid.NewGuid().ToString();
                await respository.CreateItemAsync(item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync([Bind("Id,Name,Description,Completed")] Item item)
        {
            if (ModelState.IsValid)
            {
                await respository.UpdateItemAsync(item.Id, item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Item item = await respository.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Item item = await respository.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {
            Item item = await respository.GetItemAsync(id);
            await respository.DeleteItemAsync(id, item);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            Item item = await respository.GetItemAsync(id);
            return View(item);
        }
    }
}
