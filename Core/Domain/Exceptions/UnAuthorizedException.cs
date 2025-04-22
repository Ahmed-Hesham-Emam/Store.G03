﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
    {
    public class UnAuthorizedException(string message = "Invalid Email and/or Password") : Exception(message)
        {
        }
    }
