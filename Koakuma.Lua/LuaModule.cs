using Koakuma.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Koakuma.Shared.Messages;
using MoonSharp.Interpreter;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Koakuma.Lua
{
    public class LuaModule : IModule
    {
        private Script script;
        private KoakumaProxy proxy;

        public ModuleConfig Config { get; set; }

        public ModuleFeatures Features { get { return ModuleFeatures.Default; } }

        public string ID { get { return "lua.script"; } }

        public IKoakuma Koakuma { get; set; }

        public void Load()
        {
            proxy = new KoakumaProxy(this);
            script = new Script();

            var oldPath = script.Globals["package", "path"] as string;
            script.Globals["package", "path"] = $"{oldPath};{Config.Get("script_root", "").TrimEnd('/', '\\')}/?";
            var mainFile = Path.Combine(Config.Get("script_root", ""), Config.Get("main_file", ""));

            UserData.RegisterType<ModuleID>();
            UserData.RegisterType<KoakumaProxy>();
            
            script.Globals["koakuma"] = proxy;

            Koakuma.Logger.Log(LogLevel.Verbose, script.DoFile(mainFile, codeFriendlyName: mainFile));
        }

        public void OnMessage(ModuleID from, BaseMessage msg, byte[] payload)
        {
            proxy.CallOnMessage(from, msg, payload);
        }

        public void Reload()
        {
        }

        public void Unload()
        {
        }
    }
}
