﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolSync
{
    public class School
    {
        public string SchoolCode { get; set; }
        public string Name { get; set; }
        public string RegionNumber { get; set; }
        public string RegionName { get; set; }
        public string ParishNumber { get; set; }
        public string ParishName { get; set; }
        public string SchoolTypeCode { get; set; }
        public string SchoolTypeName { get; set; }
        public string Gender { get; set; }
        
        
        [JsonIgnore]
        public string ErrorMessage { get; set; }
    }
}
