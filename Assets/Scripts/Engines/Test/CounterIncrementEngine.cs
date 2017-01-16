using Nodes.Test;
using Svelto.ES;
using Svelto.Ticker;
using System;
using UnityEngine;

namespace Engines.Test {
    public class CounterIncrementEngine : INodesEngine, IIntervaledTickable, IQueryableNodeEngine {

        readonly Type[] _acceptedNodes = {
            typeof(TestNode)
        };

        public IEngineNodeDB nodesDB { set; private get; }
        public Type[] AcceptedNodes() {
            return _acceptedNodes;
        }

        // Where we'd do work each time a node is registered.
        public void Add(INode obj) { }

        // Where we'd do work each time a node is unregistered.
        public void Remove(INode obj) { }

        [IntervaledTick(1.0f)]
        public void IntervaledTick () {
            var testNodeList = nodesDB.QueryNodes<TestNode>();

            foreach(TestNode testNode in testNodeList) {
                testNode.testComponent.counter++;
            }
        }
    }
}
