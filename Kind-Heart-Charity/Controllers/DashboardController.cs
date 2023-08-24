using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kind_Heart_Charity.Data;
using Kind_Heart_Charity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.DotNet.MSIdentity.Shared;

namespace Kind_Heart_Charity.Controllers
{
    public class DashboardController : Controller
    {
        private readonly Kind_Heart_CharityContext _context;

        public DashboardController(Kind_Heart_CharityContext context)
        {
            _context = context;
        }


      

        [Authorize]
        public async Task<IActionResult> Donations()
        {
           

            return _context.Donations != null ?
                View(await _context.Donations.ToListAsync()) :
                Problem("Entity set 'Kind_Heart_CharityContext.Donations' is null.");
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            

            List<DynamicPagesDBDTO> data = new List<DynamicPagesDBDTO>();
            _context.DynamicPages
                .Where(x => x.IsDeleted == false)
                .ToList()
                .ForEach(x => data.Add(new DynamicPagesDBDTO() { Id = x.Id, PageName = x.PageName, SectionName = x.SectionName }));

            ViewBag.DynamicPages = data;
            return View();
        }

        [Authorize]
        private bool DonationsExists(int id)
        {
            return (_context.Donations?.Any(e => e.Id == id)).GetValueOrDefault();
        }



    }
}
