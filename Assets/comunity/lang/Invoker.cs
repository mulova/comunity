using UnityEngine;
using System;

namespace comunity
{
    public class Invoker : MonoBehaviour
    {
        private ActionQueue queue = new ActionQueue();
        
        void Update()
        {
            queue.Update();
            if (queue.isEmpty)
            {
                enabled = false;
            }
        }
        
        public void InvokeLater(Action action)
        {
            if (action != null)
            {
                enabled = true;
                queue.Add(action);
            }
        }
    }
}

