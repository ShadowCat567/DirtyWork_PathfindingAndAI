using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehTree
{
    public abstract class BTree : MonoBehaviour
    {
        private Node root = null;

        protected void Start()
        {
            root = SetUpTree();
        }

        private void Update()
        {
            if (root != null) //evaluate the state of each node in the tree starting from the root
            {
                root.Evaluate();
            }
        }

        protected abstract Node SetUpTree(); //sets up the behavior tree
    }
}
