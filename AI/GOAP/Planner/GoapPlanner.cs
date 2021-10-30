using System;
using System.Collections.Generic;
using System.Text;
using GDG.ModuleManager;
using GDG.Utils;

namespace GDG.AI
{
    public class GoapPlanner<TGoal, TAction> : IGoapPlanner<TGoal, TAction> where TGoal : Enum where TAction : Enum
    {
        private Queue<IGoapAction<TAction>> actionQueue = new Queue<IGoapAction<TAction>>();
        private IGoapAgent<TGoal, TAction> agent;
        private ActionTree actionTree = new ActionTree();
        public GoapPlanner(IGoapAgent<TGoal, TAction> agent)
        {
            this.agent = agent;
        }
        //生成一个最优计划
        public Queue<IGoapAction<TAction>> GeneratePlan()
        {
            actionQueue.Clear();
            var node = BuildGraph(agent.CurrentGoal);
            
            if(actionTree.TopNode.childs.Count == 0)
            {
                Log.Warning("There are no GoapAction which can be planned !");
                return null;
            }
            else
            {
                if(node == null)
                {
                    Log.Warning("There are no GoapAction which can achieve the goal !");
                    return null;                    
                }
                while(node.action==null)
                {
                    actionQueue.Enqueue(node.action);
                    node = node.parent;
                }
            }
            return actionQueue;
        }
        //建立Action行为树，并返回符合目标Effect的根节点
        private ActionNode BuildGraph(IGoapGoal<TGoal> goal)
        {
            actionTree.Clear();
            actionTree.InitTop(agent, goal);
            ActionNode minCostNode = null;

            Queue<ActionNode> nodeQueue = new Queue<ActionNode>();

            nodeQueue.Enqueue(actionTree.TopNode);
            foreach(var node in actionTree.TopNode.childs)
            {
                nodeQueue.Enqueue(actionTree.GetNode(node));
            }
            int maxPriority = int.MinValue;
            bool isFound = false;

            //BFS构建行为树
            while(nodeQueue.Count>0)
            {
                var currentNode = nodeQueue.Dequeue();
                if(currentNode.action==null)
                    continue;

                //查找最高优先级
                if(maxPriority > currentNode.action.Priority)
                    continue;
                else
                    maxPriority = currentNode.action.Priority;

                isFound = false;
                //建立子节点
                foreach(var action in agent.Actions)
                {
                    //是否有动作能够满足当前节点的Effect
                    if(action!= currentNode.action && action.Effect.ContainsState(currentNode.action.Effect))
                    {
                        var node = actionTree.CreateNode(currentNode, action);
                        currentNode.childs.Add(node.id);
                        nodeQueue.Enqueue(node);
                        isFound = true;
                    }
                }
                //如果没有找到，则说明这是根节点
                if(!isFound)
                {
                    if(agent.AgentState.ContainsState(currentNode.action.Effect))
                    {
                        if(minCostNode == null)
                            minCostNode = currentNode;
                        minCostNode = minCostNode.cost < currentNode.cost ? minCostNode : currentNode;
                    }
                }
            }
            return minCostNode;
        }
        
        //目标导向的行为树
        private class ActionTree
        {
            public int MaxId { get; private set; }
            public ActionNode TopNode { get; private set; }
            private Dictionary<int,ActionNode> nodesDic = new Dictionary<int,ActionNode>();
            public ActionTree()
            {
                TopNode = ObjectPool.Instance.Pop<ActionNode>();
                TopNode.id = 0;
                TopNode.cost = ActionNode.Min_Cost;
                TopNode.action = null;
                TopNode.parent = null;
                TopNode.depth = 1;
            }
            public ActionNode GetNode(int id)
            {
                if(nodesDic.TryGetValue(id,out ActionNode node))
                {
                    return node;
                }
                return null;
            }
            public void InitTop(IGoapAgent<TGoal, TAction> agent,IGoapGoal<TGoal> goal)
            {
                agent.AgentState.GetDifferentStates(goal.Effect);
                var differentStates = agent.AgentState.GetDifferentStates(goal.Effect);
                foreach(var action in agent.Actions)
                {
                    if(action.Effect.ContainsPairs(differentStates))
                    {
                        TopNode.childs.Add(CreateNode(TopNode,action).id);
                    }
                }
            }
            public ActionNode CreateNode(ActionNode parentNode, IGoapAction<TAction> action)
            {
                var node = ObjectPool.Instance.Pop<ActionNode>();
                node.id = ++MaxId;
                if(parentNode!=null)
                {
                    node.depth = parentNode.depth + 1;
                }
                else
                    node.depth = 1;
                node.action = action;
                node.cost = parentNode.cost + action.Cost;
                nodesDic.Add(node.id,node);
                return node;
            }
            public void Clear()
            {
                foreach (var node in nodesDic.Values)
                {
                    node.Reset();
                    ObjectPool.Instance.Push<ActionNode>(node);
                }
                TopNode.Reset();
                nodesDic.Clear();
                MaxId = 0;
            }
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("\nroot\n");
                Queue<ActionNode> nodeQueue = new Queue<ActionNode>();

                
                foreach(var node in TopNode.childs)
                {
                    nodeQueue.Enqueue(GetNode(node));
                    sb.Append($"{GetNode(node).action.ActionType}   ");
                }
                sb.Append($"\n");
                var currentDepth = 2;
                ActionNode currentNode;
                while(nodeQueue.Count>0)
                {
                    currentNode = nodeQueue.Dequeue();
                    if(currentNode.depth != currentDepth)
                    {
                        sb.Append($"\n");
                        currentDepth = currentNode.depth;
                    }
                    foreach (var node in currentNode.childs)
                    {
                        nodeQueue.Enqueue(GetNode(node));
                        sb.Append($"{GetNode(node).action.ActionType}   ");
                    }
                }
                return sb.ToString();
            }
        }
        private class ActionNode
        {
            public const int DEFAULT_ID = 0;
            public const int Min_Cost = 0;
            public int id = DEFAULT_ID;
            public int cost = 0;
            public int depth = 0;
            public ActionNode parent;
            public IGoapAction<TAction> action;
            public List<int> childs = new List<int>();
            public void Reset()
            {
                id = DEFAULT_ID;
                parent = null;
                action = null;
                cost = Min_Cost;
                childs.Clear();
            }
        }
    }
}