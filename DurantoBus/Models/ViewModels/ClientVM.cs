using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Http;

namespace DurantoBus.Models.ViewModels
{
    public class ClientVM
    {
        public ClientVM()
        {
            this.StationList = new List<int>();

        }
        public int ClientId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        [Required, DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateofBirth { get; set; }
        public bool IsActive { get; set; }
        public string Picture { get; set; }
        public IFormFile PicturePath { get; set; }
        public  List<int> StationList { get; set; }
    }
}
