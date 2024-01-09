using System.Collections;
using System.Collections.Generic;

namespace BehTree
{
    public enum NodeState //enum that defines what states a node can be in
    {
        RUNNING, SUCCESS, FAILURE
    }

    //base class for Behavior Tree node
    public class Node
    {
        protected NodeState state;
        public Node parent;
        protected List<Node> children = new List<Node>();

        public Node() //constructor for a leaf node
        {
            parent = null;
        }

        public Node(List<Node> children) //constructor for a node with children
        {
            foreach (Node child in children)
            {
                Attach(child);
            }
        }

        private void Attach(Node child) //attaches child nodes to parent node
        {
            child.parent = this;
            children.Add(child);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE; //evaluates the state a node is in
    }
}
