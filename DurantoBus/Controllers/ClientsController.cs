using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DurantoBus.Models;
using Microsoft.AspNetCore.Hosting;
using DurantoBus.Models.ViewModels;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore;

namespace DurantoBus.Controllers
{
    public class ClientsController : Controller
    {
        private readonly StationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;


        public ClientsController(StationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

      
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.Include(x => x.BookingEntries).ThenInclude(b => b.Station).ToListAsync());
        }

        public IActionResult AddNewStation(int? id)
        {
            ViewBag.Stations = new SelectList(_context.Stations.ToList(), "StationId", "StationName", id.ToString() ?? "");
            return PartialView("_AddNewStation");
        }

       
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
        public async Task<IActionResult> Create(ClientVM clientVM, int[] StationId)
        {
            if (ModelState.IsValid)
            {
                Client client = new Client()
                {
                    Name = clientVM.Name,
                    Age = clientVM.Age,
                    DateofBirth = clientVM.DateofBirth,
                    IsActive = clientVM.IsActive
                };
                //Img
                string webroot = _hostEnvironment.WebRootPath;
                string folder = "Images";
                string imgFileName = Path.GetFileName(clientVM.PicturePath.FileName);
                string fileToWrite = Path.Combine(webroot, folder, imgFileName);

                using (var stream = new FileStream(fileToWrite, FileMode.Create))
                {
                    await clientVM.PicturePath.CopyToAsync(stream);
                    client.Picture = "/" + folder + "/" + imgFileName;
                }
                foreach (var item in StationId)
                {
                    BookingEntry bookingEntry = new BookingEntry()
                    {
                        Client = client,
                        ClientId = client.ClientId,
                        StationId = item
                    };
                    _context.BookingEntries.Add(bookingEntry);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public IActionResult Edit(int id)
        {
            var clientVM = JsonConvert.DeserializeObject<ClientVM>(JsonConvert.SerializeObject(_context.Clients.First(x => x.ClientId == id)));
            var StationList = _context.BookingEntries.Where(x => x.ClientId == id).Select(x => x.StationId).ToList();
            clientVM.StationList = StationList;
            return View(clientVM);
        }
        [HttpPost]
        public IActionResult Edit(ClientVM clientVM, int[] StationId)
        {
            var client = JsonConvert.DeserializeObject<Client>(JsonConvert.SerializeObject(clientVM));

            if (clientVM.PicturePath != null)
            {
                var filePath = Path.Combine("Images", Guid.NewGuid() + Path.GetExtension(clientVM.PicturePath.FileName));
                clientVM.PicturePath.CopyTo(new FileStream(Path.Combine(_hostEnvironment.WebRootPath, filePath), FileMode.Create));
                client.Picture = filePath;
            }
            var existsStation = _context.BookingEntries.Where(x => x.ClientId == client.ClientId);
            foreach (var item in existsStation)
            {
                _context.BookingEntries.Remove(item);
            }

            foreach (var item in StationId)
            {
                _context.BookingEntries.Add(new BookingEntry { Client = client, ClientId = client.ClientId, StationId = item });
            }
            _context.Entry(client).State = EntityState.Modified;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            var client = _context.Clients.First(x => x.ClientId == id);
            var existsStation = _context.BookingEntries.Where(x => x.ClientId == id);
            foreach (var item in existsStation)
            {
                _context.BookingEntries.Remove(item);
            }
            _context.Entry(client).State = EntityState.Deleted;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }       
      
    }
}
