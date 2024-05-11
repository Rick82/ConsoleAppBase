using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Services
{
    public interface IMyService
    {
        Task PerformLongTaskAsync();
    }
    public class MyService : IMyService
    {
        public async Task PerformLongTaskAsync()
        {
            await Task.Delay(5000);
        }
    }
}
