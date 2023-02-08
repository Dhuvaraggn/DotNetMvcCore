using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SelfTraining.Data;
using SelfTraining.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SelfTraining.Controllers
{
    [Authorize(Policy = "AdminRole")]
    public class TrackController : Controller
    {
        public ApplicationDbContext _db;
        public TrackController(ApplicationDbContext db)
        {
            _db = db;
        }
        // GET: /<controller>/
        public IActionResult Index(int? mailid)
        {
            if(mailid == 0)
            {
                return NotFound();
            }
            Mail maildetails = _db.Mails.Find(mailid);
            Mail md = _db.Mails.SingleOrDefault(x => x.ID == mailid);
            return View(maildetails);
        }
    }
}

