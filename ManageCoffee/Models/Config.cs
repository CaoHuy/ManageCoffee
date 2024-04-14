using System;
using System.Collections.Generic;

#nullable disable

namespace ManageCoffee.Models
{
    public partial class Config
    {
        public int ConfigId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
