﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FreeSqlDemo.Bussiness.DTO.Terant
{
    public class TerantInput
    {
        [Required]
        public string Name { get; set; }
        
    }
}
