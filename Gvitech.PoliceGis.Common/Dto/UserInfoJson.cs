﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mmc.Mspace.Common.Dto
{
    public  class UserInfoJson
    {
        public string status { get; set; }
        public string message { get; set; }

        public UserInfo data { get; set; }
    }
}
