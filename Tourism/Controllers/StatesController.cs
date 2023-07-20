using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tourism.DataAccess;
using Tourism.Models;
namespace Tourism.Controllers
{
    public class StatesController : Controller
    {
        private readonly TourismContext _context;

        public StatesController(TourismContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var states = _context.States.ToList();
            return View(states);
        }

        [HttpPost]
        public IActionResult Index(State state)
        {
            _context.States.Add(state);
            _context.SaveChanges();
            var stateId = state.Id;

            return RedirectToAction("show", new{id = stateId});
            //even though show action has the route specified
            //this still sends me to /states/show/stateId for some reason, spent too much time trying to figure this out :(
        }


        [Route("/states/{stateId:int}")]
        public IActionResult Show(int stateId)
        {
            var state = _context.States.Find(stateId);
            return View(state);
        }


        public IActionResult New()
        {
            return View();
        }


    }
}
