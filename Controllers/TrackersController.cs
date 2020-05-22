using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using VirusTracker.Models;
using VirusTracker.Services;
using VirusTracker.ViewModels;

namespace VirusTracker.Controllers
{

    public class TrackersController : Controller
    {
        private const string c_CONTRIVEDCOOKIENAME = "TrackingCookieAndDetails";
        private const string c_TrackingCookieWithID = "TrackingCookieWithID";
        private const string c_NAMECOOKIENAME = "TrackingCookie";
        private readonly ICookieService _cookieService;


        public string Place { get; set; }

        private readonly ILogger<TrackersController> _logger;
        private readonly IWebHostEnvironment _env;
        private IHttpContextAccessor _httpacc;
        private readonly TracingContext _context;
        // private readonly UserManager<ApplicationUser> _userManager;


        public TrackersController(TracingContext context, ILogger<TrackersController> logger,
            IWebHostEnvironment env,
              IHttpContextAccessor httpacc,
              ICookieService cookieService
       //      UserManager<ApplicationUser> userManager
       )
        {
            _context = context;
            _logger = logger;
            _env = env;
            _httpacc = httpacc;
            _cookieService = cookieService;
            //  _userManager = userManager;
        }

        // GET: Trackers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tracker.OrderByDescending(o => o.DateIn).ToListAsync());
        }

        // GET: Trackers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracker = await _context.Tracker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tracker == null)
            {
                return NotFound();
            }

            return View(tracker);
        }

        //https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-3.1#how-model-binding-works
        // GET: Trackers/Create

        [HttpGet()]
        public IActionResult Create()
        {   //https://localhost:44394/Trackers/Create?Place=Vision_College
            //only update place with teh query if there is something in the string
            string nullCheck = null;
            nullCheck = HttpContext.Request.Query["Place"].ToString();

            if (!string.IsNullOrEmpty(nullCheck))
            {
                Place = HttpContext.Request.Query["Place"].ToString();

                ViewData["Place"] = Place;

            }

            //get the cookies back
            var name = _cookieService.Get<string>(c_NAMECOOKIENAME);  //sample
            var contrived = _cookieService.Get<ContrivedValues>(c_CONTRIVEDCOOKIENAME);  //sample

            //  var contrived = _cookieService.GetOrSet<ContrivedValues>(c_CONTRIVEDCOOKIENAME, () => new ContrivedValues { Name = "n", Phone = "p", Place = Place });

            if (contrived != null)
            {
                var viewModel = new TrackerCookiesVM
                {//2 classes
                    Name = name,
                    Contrived = contrived
                };

                ViewData["cookieName"] = viewModel.Contrived.Name;
                ViewData["cookiePhone"] = viewModel.Contrived.Phone;
                ViewData["cookiePlace"] = viewModel.Contrived.Place;
            }
            else
            {
                ViewData["cookieName"] = "Name";
                ViewData["cookiePhone"] = "";
                ViewData["cookiePlace"] = "";

            }


            return View();
        }

        // POST: Trackers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598. 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Phone, Place")] Tracker tracker)
        {

            //https://localhost:44394/Trackers/Create?Place=Vision_College


            Models.Business data = _context.Business.FirstOrDefault(m => m.BusinessName == tracker.Place);
            Guid aSPNetUsersIdfk = data.ASPNetUsersIdfk;
            tracker.ASPNetUsersIdfk = aSPNetUsersIdfk.ToString();
            tracker.Id = Guid.NewGuid();
            tracker.DateIn = DateTime.Now;
            tracker.DateOut = DateTime.Now.AddHours(2); //incase they forget to log out
            SaveCookies(tracker);


            _context.Add(tracker);
            await _context.SaveChangesAsync();



            //pass through the model.  https://docs.microsoft.com/en-us/aspnet/core/mvc/views/overview?view=aspnetcore-3.1
            //  return View(nameof(LogoutTracker));
            // return View(tracker);
            //https://exceptionnotfound.net/asp-net-core-demystified-action-results/

            //  return NoContent();
            return RedirectToAction("Index"); //works

            //     return RedirectToAction("LogoutTracker"); 
        }

        private void SaveCookies(Tracker tracker)
        {
            var contrived = new ContrivedValues
            {
                Id = tracker.Id,
                Name = tracker.Name,
                Phone = tracker.Phone,
                Place = tracker.Place

            };

            //set the cookies
            //  var request = new string[] {tracker.Id, tracker.Name, tracker.Phone, tracker.Place };
            _cookieService.Set(c_NAMECOOKIENAME, tracker.Name);
            _cookieService.Set(c_CONTRIVEDCOOKIENAME, contrived);
        }

        #region Session data not being used at present
        //set user info into session
        // LogoutTrackerModel logoutDetails = new LogoutTrackerModel();

        //    var data = _context.Tracker.FirstOrDefaultAsync(m => m.Name == tracker.Name);

        /*  logoutDetails.ID = tracker.Id.ToString();
          logoutDetails.Name = tracker.Name;
          logoutDetails.Phone = tracker.Phone;*/
        // HttpContext.Session.SetString("LoginDetails", JsonConvert.SerializeObject(logoutDetails));
        #endregion


        // GET: Trackers/LogoutTracker
        //  [HttpGet()]
        public IActionResult LogoutTracker()
        {
            //get the cookie
            var contrived = _cookieService.Get<ContrivedValues>(c_CONTRIVEDCOOKIENAME);

            var viewModel = new TrackerCookiesVM
            { Contrived = contrived };

            ViewData["cookieName"] = viewModel.Contrived.Name;
            ViewData["cookiePhone"] = viewModel.Contrived.Phone;
            ViewData["cookiePlace"] = viewModel.Contrived.Place;
            ViewData["Id"] = viewModel.Contrived.Id;
            return View("LogoutTracker");
        }



        // GET: Trackers/LogoutTracker
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutTracker(Guid id, [Bind("Id,ASPNetUsersIdfk, BusinessName,Name,Phone,DateIn,DateOut")] Tracker tracker)
        {

            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    tracker.DateOut = DateTime.Now;
                    _context.Update(tracker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrackerExists(tracker.Id))
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
            return View();
        }





        // GET: Trackers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracker = await _context.Tracker.FindAsync(id);
            if (tracker == null)
            {
                return NotFound();
            }
            return View(tracker);
        }

        // POST: Trackers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ASPNetUsersIdfk, BusinessName,Name,Phone,DateIn,DateOut")] Tracker tracker)
        {
            if (id != tracker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tracker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrackerExists(tracker.Id))
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
            return View(tracker);
        }

        // GET: Trackers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracker = await _context.Tracker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tracker == null)
            {
                return NotFound();
            }

            return View(tracker);
        }

        // POST: Trackers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tracker = await _context.Tracker.FindAsync(id);
            _context.Tracker.Remove(tracker);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrackerExists(Guid id)
        {
            return _context.Tracker.Any(e => e.Id == id);
        }
    }
}
