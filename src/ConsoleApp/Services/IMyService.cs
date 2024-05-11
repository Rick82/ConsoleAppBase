using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppBase.Services
{
    public interface IMyService
    {
        Task PerformLongTaskAsync();
    }
}
