using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin;

namespace testPlugin_2
{
    [ExportMetadata("Name", "test12")]
    [ExportMetadata("Ver", "1.1")]
    [Export(typeof(ITest))]
    class Test1 : ITest
    {
        public void Print()
        {
            Console.WriteLine("Test2:Print -> {0}", _String);
        }

        string _String = "I'm Test 2!";
        public void SetString(string name)
        {
            _String = name;
        }
    }
}
