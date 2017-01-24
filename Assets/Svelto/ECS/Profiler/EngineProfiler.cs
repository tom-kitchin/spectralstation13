using System;
using System.Collections.Generic;
using System.Diagnostics;

//This profiler is based on the Entitas Visual Debugging tool 
//https://github.com/sschmid/Entitas-CSharp

namespace Svelto.ECS.Profiler
{
    public sealed class EngineProfiler
    {
        static readonly Stopwatch _stopwatch = new Stopwatch();

        public static void MonitorAddDuration(Action<INodeEngine<INode>, INode> addingFunc, INodeEngine<INode> engine, INode node)
        {
            EngineInfo info;
            if (engineInfos.TryGetValue(engine.GetType(), out info))
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                addingFunc(engine, node);
                _stopwatch.Stop();

                info.AddAddDuration(_stopwatch.Elapsed.TotalMilliseconds);
            }
        }

        public static void MonitorRemoveDuration(Action<INodeEngine<INode>, INode> removeFunc, INodeEngine<INode> engine, INode node)
        {
            EngineInfo info;
            if (engineInfos.TryGetValue(engine.GetType(), out info))
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                removeFunc(engine, node);
                engine.Remove(node);
                _stopwatch.Stop();
            
                info.AddRemoveDuration(_stopwatch.Elapsed.TotalMilliseconds);
            }
        }

        public static void AddEngine(IEngine engine)
        {
            if (engineInfos.ContainsKey(engine.GetType()) == false)
            {
                engineInfos.Add(engine.GetType(), new EngineInfo(engine));
            }
        }

        public static void ResetDurations()
        {
            foreach (var engine in engineInfos)
            {
                engine.Value.ResetDurations();
            }
        }

        public static readonly Dictionary<Type, EngineInfo> engineInfos = new Dictionary<Type, EngineInfo>();
    }
}
