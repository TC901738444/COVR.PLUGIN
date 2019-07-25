using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Reflection;
using System.Linq;

namespace COVR.PLUGIN
{
    public class PluginManager
    {
        public PluginManager()
        {
        }
        public object GetClass(string contractName)
        {
            if (partsDic.ContainsKey(contractName))
            {
                var dic = partsDic[contractName];
                var dict = dic.First().Value;
                return GetClass(dict.First().Value);
            }
            return null;
        }
        public object GetClass(string contractName, string className)
        {
            if (partsDic.ContainsKey(contractName))
            {
                var dic = partsDic[contractName];
                if (dic.ContainsKey(className))
                {
                    return GetClass(dic[className].First().Value);
                }
            }
            return null;
        }
        public object GetClass(string contractName, string className, string ver)
        {
            if (partsDic.ContainsKey(contractName))
            {
                var dic = partsDic[contractName];
                if (dic.ContainsKey(className))
                {
                    return GetClass(dic[className][ver]);
                }
            }
            return null;
        }

        private object GetClass(MyPart part)
        {
            ComposablePart composablePart = part.part.CreatePart();
            if (part.is_singleInstance)
            {
                string str = part.contractName + "|" + part.name + "|" + part.ver;
                if (!singleInstance.ContainsKey(str))
                {
                    singleInstance.Add(str, composablePart.GetExportedValue(part.exportDef));
                }
                return singleInstance[str];
            }
            return composablePart.GetExportedValue(part.exportDef);
        }
        public string[] Names
        {
            get
            {
                List<string> name = new List<string>();
                foreach (var T in partsDic)
                {
                    foreach (var part in T.Value)
                    {
                        foreach (var count in part.Value)
                        {
                            name.Add(T.Key + "|" + part.Key + "|" + count.Key);
                        }
                    }
                }
                return name.ToArray();
            }
        }

        AggregateCatalog catelog = new AggregateCatalog();
        public void AddPlugin(Assembly assembly)
        {
            catelog.Catalogs.Add(new AssemblyCatalog(assembly));
            UpdatePluginList();
        }

        public void AddPlugin()
        {
            catelog.Catalogs.Add(new AssemblyCatalog(Assembly.GetCallingAssembly()));
            UpdatePluginList();
        }

        public void AddPlugin(string pluginPath)
        {
            if (!Path.IsPathRooted(pluginPath))
            {
                pluginPath = Environment.CurrentDirectory + "\\" + pluginPath;
            }
            Directory.CreateDirectory(pluginPath);
            catelog.Catalogs.Add(new DirectoryCatalog(pluginPath));
            UpdatePluginList();
        }
        class MyPart
        {
            public string contractName = "";
            public string name = "";
            public string ver = "";
            public bool is_singleInstance = false;
            public ComposablePartDefinition part;
            public ExportDefinition exportDef;
        }

        Dictionary<string, Dictionary<string, Dictionary<string, MyPart>>> partsDic = new Dictionary<string, Dictionary<string, Dictionary<string, MyPart>>>();
        Dictionary<string, object> singleInstance = new Dictionary<string, object>();

        private void UpdatePluginList()
        {
            partsDic.Clear();
            foreach (var T in catelog)
            {
                foreach (var def in T.ExportDefinitions)
                {
                    MyPart myPart = new MyPart();
                    myPart.contractName = def.ContractName;
                    myPart.part = T;
                    myPart.exportDef = def;
                    if (def.Metadata.ContainsKey("Name"))
                        myPart.name = def.Metadata["Name"].ToString();
                    if (def.Metadata.ContainsKey("Ver"))
                        myPart.ver = def.Metadata["Ver"].ToString();
                    if (def.Metadata.ContainsKey("System.ComponentModel.Composition.CreationPolicy"))
                    {
                        if (def.Metadata["System.ComponentModel.Composition.CreationPolicy"].ToString() == "Shared")
                        {
                            myPart.is_singleInstance = true;
                        }
                    }
                    if (!partsDic.ContainsKey(myPart.contractName))
                    {
                        partsDic[myPart.contractName] = new Dictionary<string, Dictionary<string, MyPart>>();
                    }
                    if (!partsDic[myPart.contractName].ContainsKey(myPart.name))
                    {
                        partsDic[myPart.contractName][myPart.name] = new Dictionary<string, MyPart>();
                    }
                    partsDic[myPart.contractName][myPart.name].Add(myPart.ver, myPart);
                }
            }
        }
    }
}

/*
 * [Export(Typeof(IInterface))]
 * [ExportMetadata("Mark", "class name")]
 * 
 * [ImportingConstructor]   // 用于指定使用哪个构造函数在Importing时
 * [PartNotDiscoverable]    // 满足条件但是不想被作为部件导出的
 */
