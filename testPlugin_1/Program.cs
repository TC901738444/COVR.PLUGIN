using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin;

namespace testPlugin_1
{
    [ExportMetadata("Name", "test1")]
    [ExportMetadata("Ver", "1.0")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(Plugin.ITest))]
    class Test1 : ITest
    {
        public void Print()
        {
            Console.WriteLine("Test1:Print -> {0}", _String);
        }

        string _String = "";
        public void SetString(string name)
        {
            _String = name;
        }
    }
}
