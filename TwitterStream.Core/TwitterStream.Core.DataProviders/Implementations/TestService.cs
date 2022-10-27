using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterStream.Core.Interfaces;

namespace TwitterStream.Core.Implementations
{
    public interface ITest
    {
        void Test();
    }
    public class TestService : ITest
    {
        public TestService()
        {
        }

        public void Test() { Console.WriteLine("Hello from Test Service"); }
    }
    
}
