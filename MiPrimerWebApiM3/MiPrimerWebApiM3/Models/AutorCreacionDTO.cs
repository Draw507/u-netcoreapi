﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MiPrimerWebApiM3.Models
{
    public class AutorCreacionDTO
    {
        [Required]
        public string Nombre { get; set; }
    }
}
