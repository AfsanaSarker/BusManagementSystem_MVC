using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace DurantoBus.Models
{
    public class Client
    {
        public Client()
        {
            this.BookingEntries = new List<BookingEntry>();

        }
        public int ClientId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        [Required, DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateofBirth { get; set; }
        public bool IsActive { get; set; }
        public string Picture { get; set; }
        public virtual ICollection<BookingEntry> BookingEntries { get; set; }


    }
    public class Station
    {
        public Station()
        {
            this.BookingEntries = new List<BookingEntry>();

        }
        public int StationId { get; set; }
        [Display(Name = "Station Name")]
        public string StationName { get; set; }
        //
        public virtual ICollection<BookingEntry> BookingEntries { get; set; }


    }
    public class BookingEntry
    {
        public int BookingEntryId { get; set; }
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        [ForeignKey("Station")]
        public int StationId { get; set; }
        //
        public virtual Client Client { get; set; }
        public virtual Station Station { get; set; }
    }
    public class StationDbContext : DbContext
    {
        public StationDbContext(DbContextOptions<StationDbContext> options) : base(options)
        {

        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<BookingEntry> BookingEntries { get; set; }


    }
}
