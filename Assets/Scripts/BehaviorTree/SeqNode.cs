using System.Collections;
using System.Collections.Generic;

namespace BehTree
{
    //Sequence composite node, acts like AND operator, only succeeds if all child node succeed
    public class SeqNode : Node
    {
        public SeqNode() : base() { }
        public SeqNode(List<Node> children) : base(children) { }

        //fails if any child fails, is in running state if any child is running
        //suceeds if all children succeed
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