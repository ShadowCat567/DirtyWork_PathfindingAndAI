using System.Collections;
using System.Collections.Generic;

namespace BehTree
{
    //Selector composite node, acts like OR operator
    public class SelNode : Node
    {
        public SelNode() : base() { }
        public SelNode(List<Node> children) : base(children) { }

        //return success once we find a node that succeeds, return running once we find a
        //node that is running. Only return failure if we don't find any nodes that are
        //succeeding or running
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
