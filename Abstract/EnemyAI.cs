using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

namespace Game
{
    public abstract class EnemyAI : EnemySettings
    {
        private Vector3 _newPos;

        protected virtual void Patrol()
        {
            if (Vector3.Distance(_trfm.position, _newPos) > 3)
            {
                _anim.SetBool("move", true);
                if (_agent.velocity.magnitude < 0.5)
                {
                    _anim.SetBool("move", false);
                    if (Timer(2f))
                    {
                        RngWanderPos();
                        return;
                    }
                }
                else SetTime(0);
                _agent.SetDestination(_newPos);
            }
            else
            {
                RngWanderPos();
            }
        }

        protected virtual void RngWanderPos()
        {
            NavMeshHit nHit;
            Vector3 rngPoint = (Random.insideUnitSphere * 5 + _trfm.position);
            NavMesh.SamplePosition(rngPoint, out nHit, 10, -1);
            _newPos = nHit.position;
        }
    }
}
