using COVR.PLUGIN;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin
{
    class Program
    {
        static void Main(string[] args)
        {
            PluginManager manager = new PluginManager();
            manager.AddPlugin("plugin");
            var names = manager.Names;
            foreach (string name in names)
            {
                string[] pluin = name.Split('|');
                var c = (Plugin.ITest)manager.GetClass(pluin[0], pluin[1], pluin[2]);
                var c1 = (Plugin.ITest)manager.GetClass(pluin[0], pluin[1], pluin[2]);
                if (c == c1)
                {
                    Console.WriteLine("Same!!!");
                }
                c.SetString("instance one!");
                c1.SetString("instance two!");
                c.Print();
                c1.Print();
            }
        }
    }
}