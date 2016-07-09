﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extget.Common;

namespace Extget.Worker
{
    public interface IHandler {
        Task<Response> GetAsync(Request uri);
    }
}
