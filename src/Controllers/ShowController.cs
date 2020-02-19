using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Booking.Models;
using System;
using System.Linq;

namespace Booking.Controllers
{
    [Route("api/v1/shows")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        public AppDbContext _appDbContext { get; set; }

        public ShowController(AppDbContext appDbContext) {
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public IActionResult post([FromBody]Show show) {
            
            if (show == null) {
                return BadRequest();
            }

            if (show.EndTime == null || show.StartTime == null || show.Title == null ||
                show.Summary == null || show.Price < 0) 
            {
                return BadRequest();
            }

            if (show.StartTime <= DateTime.Now) {
                return Conflict();
            }

            if (show.StartTime >= show.EndTime) {
                return BadRequest();
            }

            if (!isSalonAvailable(show.SalonId)) {
                return Conflict();
            }

            const int maxSummaryLenght = 250;
            const int maxTitleLenght = 40;
            if (show.Summary.Length > maxSummaryLenght || show.Title.Length > maxTitleLenght) {
                return BadRequest();
            }

            const int maxPrice = 100;
            if (show.Price > maxPrice) {
                return BadRequest();
            }

            const int  minShowTime = 30;
            const int maxShowTime = 120;
            int showLenght = (show.EndTime - show.StartTime).Hours * 60 + (show.EndTime - show.StartTime).Minutes;
            if (showLenght < minShowTime || showLenght > maxShowTime){
                Console.Write(showLenght);
                return BadRequest();
            }   

            _appDbContext.shows.Add(show);
            _appDbContext.SaveChanges();
            return Created("show created",show);
        }

        public bool isSalonAvailable(int salonId) {
            var salonIds = _appDbContext.salons.Select(s => s.Id);
            foreach (int id in salonIds) {
                if (salonId == id) {
                    return true;
                }
            }
            return false;
        }
    }
}