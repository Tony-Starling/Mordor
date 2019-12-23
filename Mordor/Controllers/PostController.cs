using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mordor.Models;

namespace Mordor.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationContext database;
        private readonly CloudService cloud;
        public PostController(ApplicationContext DataBase, CloudService Cloud)
        {
            database = DataBase;
            cloud = Cloud;
        }

        public async Task<IActionResult> PostList()
        {
            return View(await database.Posts.ToListAsync());

        }

        public async Task<IActionResult> Index(string UserName)
        {
            int Id = database.GetUserByUserName(UserName).Id;
            var userpostlist = await database.GetUserWithPostsAsync(Id);
            return View(userpostlist);
        }


        public IActionResult Read(int PostId, int? Chapterid  )
        {
            var post = database.GetPostWithChapters(PostId).Result;
            if (Chapterid != null)
            {
                ViewBag.Chapterid = Chapterid;
            }
            else
            {
                ViewBag.Chapterid = post.PostChapters.First().Id;
            }
            return View(post);
        }

        public IActionResult Create()
        {
            ViewBag.Sections = new SelectList(database.GetSectionsListAsync().Result, "Id", "SectionName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, int SectionId)
        {
            if (ModelState.IsValid)
            {
                post.Section = database.GetSectionByIdAsync(SectionId).Result;
                post.AppUser = database.GetUserByUserName(User.Identity.Name);
                post.DateTime = System.DateTime.Today;
                database.Add(post);
                await database.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return View(post);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = await database.GetPostWithChapters(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewBag.Mode = "Edit";

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Tags,Published,DateTime")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    database.Update(post);
                    await database.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Post/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await database.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await database.Posts.FindAsync(id);
            database.Posts.Remove(post);
            await database.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        private bool PostExists(int id)
        {
            return database.Posts.Any(e => e.Id == id);
        }


        [HttpPost]
        public ActionResult UploadFiles(IEnumerable<IFormFile> files)
        {
            var image = cloud.Upload(files.First()).Result;
            return Json(image.Uri);
        }

    }
}