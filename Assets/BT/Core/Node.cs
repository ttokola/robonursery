using System;
using UnityEngine;

namespace BT
{
    public abstract class Node : INode
    {
        private Status status;

        public Status Tick()
        {
            OnPreUpdate();

            status = Update();

            OnPostUpdate();

            return status;
        }

        private void OnPreUpdate()
        {
            //Debug.LogFormat("{0} OnPreUpdate, status={1}",
            //    GetType().Name, status);

            if (status != Status.Running)
            {
                OnEnter();
            }
        }

        private void OnPostUpdate()
        {
            //Debug.LogFormat("{0} OnPostUpdate, status={1}",
            //    GetType().Name, status);

            if (status != Status.Running)
            {
                OnLeave();
            }
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnLeave()
        {
        }

        protected abstract Status Update();
    }
}