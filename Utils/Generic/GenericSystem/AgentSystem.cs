using UnityEngine;
using GDG.ECS;
using System;
using GDG.ModuleManager;
using GDG.Utils;
namespace GDG.Utils
{
    public class AgentSystem : AbsSystem<AgentSystem>
    {
        public override void OnStart()
        {

        }
        public override void OnFixedUpdate()
        {
            if(GDGTools.FlowFieldController.FlowFieldCount==0)
                return;

            Select((Entity entity,GameObjectComponent goc,AgentComponent agent) =>
                {
                    if(agent == null)
                        return;
                    if (agent.isAgent)
                    {
                        var rig = goc.gameObject.GetComponent<Rigidbody>();
                        var trans = goc.gameObject.transform;
                        var flowField = GDGTools.FlowFieldController.GetFlowField(agent.flowFieldName);
                        if (flowField != null && rig !=null)
                            rig.velocity = flowField.GetFieldDirection(trans.position) * agent.speed;
                    }
                })
                .Excute();
        }

        public override void OnUpdate()
        {

        }
    }
}