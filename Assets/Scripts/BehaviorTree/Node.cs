using System.Collections;
using System.Collections.Generic;

namespace BehTree
{
    public enum NodeState
    {
        RUNNING, SUCCESS, FAILURE
    }

    public class Node
    {
        protected NodeState state;
        public Node parent;
        protected List<Node> children = new List<Node>();

        private Dictionary<string, object> data = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                Attach(child);
            }
        }

        private void Attach(Node child)
        {
            child.parent = this;
            children.Add(child);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public void SetData(string key, object value)
        {
            data[key] = value;
        }

        //want to pull this data from anywhere in this branch
        public object GetData(string key)
        {
            object value = null;

            if (data.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                Node curNode = parent;

                while (curNode != null)
                {
                    value = curNode.GetData(key);
                    if (value != null)
                    {
                        return value;
                    }
                    curNode = curNode.parent;
                }

                return null;
            }
        }

        public bool ClearData(string key)
        {
            if (data.ContainsKey(key))
            {
                data.Remove(key);
                return true;
            }

            Node curNode = parent;

            while (curNode != null)
            {
                bool cleared = curNode.ClearData(key);
                if (cleared)
                {
                    return true;
                }
                curNode = curNode.parent;
            }

            return false;
        }
    }
}
