using System.Collections;
using System.Collections.Generic;

namespace BehTree
{
    //Sequence composite node, acts like AND operator, only succeeds if all child node succeed
    public class SeqNode : Node
    {
        public SeqNode() : base() { }
        public SeqNode(List<Node> children) : base(children) { }

        //goes through all child nodes, returns failure if at least one fails
        //if no child fails, but a child is running, return running
        //if no child fails and no child is running, return success
        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;

            foreach (Node child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

            state = anyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}