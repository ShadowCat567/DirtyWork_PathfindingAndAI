using System.Collections;
using System.Collections.Generic;

namespace BehTree
{
    //Selector composite node, acts like OR operator
    public class SelNode : Node
    {
        public SelNode() : base() { }
        public SelNode(List<Node> children) : base(children) { }

        //go through each of the nodes, stop on any node that returns success or running
        //if a node returns success or running, then return success or running
        //if a node returns failure, keep going until either no nodes remain or a node return success or running
        //if no nodes return success or running, return failure
        public override NodeState Evaluate()
        {
            foreach (Node child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
