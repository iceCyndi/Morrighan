using UnityEngine;
using System;
using System.Collections.Generic;

//using Mogo.FSM;
using Mogo.GameData;
using Mogo.Util;
using Mogo.RPC;
//using Mogo.Task;

namespace Mogo.Game
{
    public partial class EntityParent
    {
        public float tmpTime = 0;
        virtual public void TurnTo(float x, float y, float z)
        {

        }

        virtual public void MoveTo(float x, float z, float dx, float dy, float dz)
        {
            if (!Transform)
                return;

        }

        virtual public void MoveToByAngle(float angleY, float _time, bool isNeedRotation)
        {

        }

        virtual public void MoveTo(float x, float z)
        {
           // if (currentMotionState == MotionState.DEAD) return;
            MoveTo(x, 0, z);
        }

        virtual public bool MoveToByNav(Vector3 v, int stopDistance)
        {
            return false;
        }

        virtual public void MoveTo(float x, float y, float z)
        {

            Vector3 v = new Vector3(x, y, z);
            if (!(this is EntityMyself) && this is EntityPlayer)
            {
                if (motor.MoveToByNav(v))
                {
                    //Debug.LogWarning("motor.MoveToByNav:" + v);
                    Move();
                }
                else
                {
                    //Debug.LogError("can not find the way!");
                }
            }
            else
            {
                //Mogo.Util.LoggerHelper.Debug("MoveTo in entityParent:" + v);
                if (this is EntityMyself)
                {
                    if (Mathf.Abs(x - Transform.position.x) < 0.3f && Mathf.Abs(z - Transform.position.z) < 0.3f)
                    {
                        return;
                    }
                }
                motor.MoveTo(v);
                Move();
            }

        }

        virtual public void OnMoveTo(GameObject arg1, Vector3 arg2)
        {
            if (arg1 == null) return;
            if (arg1.transform == Transform)
            {
                Idle();
            }
        }

        virtual protected void OnMoveToFalse(GameObject param1, Vector3 param2, float param3)
        {
            //ShowText(2, "OnMoveToFalse");
            //LoggerHelper.Debug("OnMoveToFalse");
            if (this == null) return;
            if (Transform == null) return;
            if (param1 == null) return;
            if (param1.transform == Transform)
            {
                Idle();
            }
        }

        virtual public void Move()
        {
            if ((this is EntityMyself) && (this as EntityMyself).deathFlag == 1)
            {
                return;
            }
        }
    }
}
