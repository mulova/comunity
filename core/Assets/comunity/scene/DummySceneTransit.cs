using UnityEngine;
using System.Collections;

namespace core
{
    public class DummySceneTransit : SceneTransit
    {
        public override void StartTransit(object data)
        {
        }
        public override void EndTransit()
        {
        }
        public override bool inProgress
        {
            get
            {
                return false;
            }
        }
    }
}