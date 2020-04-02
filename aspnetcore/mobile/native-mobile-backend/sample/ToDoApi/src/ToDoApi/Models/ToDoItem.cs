﻿using System.ComponentModel.DataAnnotations;

namespace ToDoApi.Models {
    public class ToDoItem {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Notes { get; set; }

        public bool Done { get; set; }
    }
}