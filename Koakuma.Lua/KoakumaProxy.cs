using Koakuma.Shared.Messages;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Koakuma.Lua
{
    class KoakumaProxy
    {
        private LuaModule mod;

        public delegate void MessageEventHandler(ModuleID id, string jsonMsg, byte[] payload = null);

        public KoakumaProxy(LuaModule mod)
        {
            this.mod = mod;
        }

        public event MessageEventHandler OnMessageReceived;

        public void SendMessage(ModuleID receiver, string jsonMsg, byte[] payload = null)
        {
            mod.Koakuma.SendRawMessage(receiver, JObject.Parse(jsonMsg), payload);
        }

        internal void CallOnMessage(ModuleID from, BaseMessage msg, byte[] payload)
        {
            if(OnMessageReceived != null)
            {
                var jsonStr = msg.JObject.ToString();
                foreach(var d in OnMessageReceived.GetInvocationList())
                {
                    try
                    {
                        (d as MessageEventHandler)(from, jsonStr, payload);
                    }
                    catch(InterpreterException e)
                    {
                        mod.Koakuma.Logger.Log(Shared.LogLevel.Error, "proxy", e.DecoratedMessage);
                    }
                    catch(Exception e)
                    {
                        mod.Koakuma.Logger.Log(Shared.LogLevel.Error, "proxy", e);
                    }
                }
            }
        }
    }
}
