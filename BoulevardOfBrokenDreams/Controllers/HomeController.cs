﻿using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoulevardOfBrokenDreams.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private MumuDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(MumuDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        // GET: api/<HomeController>
        [HttpGet]
        public IEnumerable<HomeProjectDTO> Get()
        {
            var projects = from p in _db.Projects.Where(x=>x.StatusId ==1)
                           select new HomeProjectDTO
                           {
                               ProjectId = p.ProjectId,
                               ProjectName = p.ProjectName,
                               ProjectGoal = p.ProjectGoal,
                               StartDate = p.StartDate,
                               EndDate = p.EndDate,
                               Thumbnail = "https://" + _httpContextAccessor.HttpContext.Request.Host.Value + "/resources/mumuThumbnail/Projects_Products_Thumbnail/" + p.Thumbnail,
                               TotalAmount = ((from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.Price).Sum()) + ((from order in _db.Orders
                                                                                    join orderDetail in _db.OrderDetails on order.OrderId equals orderDetail.OrderId
                                                                                    where orderDetail.ProjectId == p.ProjectId
                                                                                    select order.Donate ?? 0).FirstOrDefault()),
                               SponsorCount = (from orderDetail in _db.OrderDetails
                                               where orderDetail.ProjectId == p.ProjectId
                                               select orderDetail.OrderId).Count(),
                           };
            Random rd = new Random();
            var randomProjects = projects.ToList().OrderBy(x => rd.Next()).Take(9);//tolist才能用隨機 不然EF沒辦法把random轉成sql語法
            return randomProjects;
        }

        // GET api/<HomeController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<HomeController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<HomeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<HomeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}