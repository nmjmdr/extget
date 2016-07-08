using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extget.Worker
{
    public interface IHandler {
        Task<Response> GetAsync(Uri uri);
    }
}
