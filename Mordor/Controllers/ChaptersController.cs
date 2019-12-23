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
    public class ChaptersController : Controller
    {
        private readonly ApplicationContext database;
        private readonly CloudService cloud;
        public ChaptersController(ApplicationContext Database, CloudService Cloud)
        {
            database = Database;
            cloud = Cloud;
        }
        public async Task<IActionResult> Index(int PostId)
        {
            Post post = database.GetPostWithChapters(PostId).Result;
            ViewBag.PostId = PostId;
            return View(post.PostChapters);
        }




        public IActionResult Create(int PostId)
        {
            ViewBag.PostId = PostId;
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Head,ImageUrl,Text")] Chapter chapter,int PostId)
        {
            if (ModelState.IsValid)
            {
                Post post = await database.GetPostWithChapters(PostId);
                post.PostChapters.Add(chapter);
                await database.SaveChangesAsync();

                return Redirect(Request.Headers["Referer"].ToString());
            }
            return View(chapter);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await database.Chapters.FindAsync(id);
            if (chapter == null)
            {
                return NotFound();
            }
            return View(chapter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Head,ImageUrl,Text")] Chapter chapter)
        {
            if (id != chapter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    database.Update(chapter);
                    await database.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChapterExists(chapter.Id))
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
            return View(chapter);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapter = await database.Chapters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chapter == null)
            {
                return NotFound();
            }

            return View(chapter);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chapter = await database.Chapters.FindAsync(id);
            database.Chapters.Remove(chapter);
            await database.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        private bool ChapterExists(int id)
        {
            return database.Chapters.Any(e => e.Id == id);
        }

        [HttpPost]
        public ActionResult UploadFiles(IEnumerable<IFormFile> files)
        {
            var image = cloud.Upload(files.First()).Result;
            return Json(image.Uri);
        }
    }
}
