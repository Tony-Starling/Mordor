using Microsoft.AspNetCore.Mvc;
using Mordor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mordor.ViewComponents
{
    public class ChaptersViewComponent : ViewComponent
    {
        private readonly ApplicationContext _context;

        public ChaptersViewComponent(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int PostId)
        {
            return View( await _context.GetPostWithChapters(PostId));
        }
    }
}
