﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BookCatalogue.API.DTO
{
    public class BookDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public DateTime PublishDate { get; set; }
        public string Publisher { get; set; }
    }
}
