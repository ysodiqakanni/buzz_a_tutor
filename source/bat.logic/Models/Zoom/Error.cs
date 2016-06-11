﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bat.logic.Models.Zoom
{
    public class Error
    {
        public ErrorObject error { get; set; }
    }

    public class ErrorObject
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}
