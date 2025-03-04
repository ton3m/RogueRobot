using Assets.CourseGame.Develop.Gameplay.Entities;
using Assets.CourseGame.Develop.Utils.Reactive;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.CourseGame.Develop.Gameplay.AI.Sensors
{
    public class NearestDamageableTargetSelector : ITargetSelector
    {
        private Transform _center;
        private ReactiveVariable<int> _team;

        public NearestDamageableTargetSelector(Transform center, ReactiveVariable<int> team)
        {
            _center = center;
            _team = team;
        }

        public bool TrySelectTarget(IEnumerable<Entity> targets, out Entity finndedTarget)
        {
            IEnumerable<Entity> damageableTargets = targets
                .Where(target =>
                    target.TryGetTakeDamageRequest(out var reques)
                    && target.TryGetIsDead(out var isDead)
                    && target.TryGetIsSpawningProcess(out var spawningProcess)
                    && spawningProcess.Value == false
                    && isDead.Value == false
                    && target.TryGetTeam(out ReactiveVariable<int> team)
                    && team.Value != _team.Value);

            if(damageableTargets.Any() == false)
            {
                finndedTarget = null;
                return false;
            }

            Entity closestTarget = damageableTargets.First();
            float minDistance = GetDistanceTo(closestTarget);

            foreach (Entity entity in damageableTargets)
            {
                float distance = GetDistanceTo(entity);

                if(distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = entity;
                }
            }

            finndedTarget = closestTarget;
            return true;
        }

        private float GetDistanceTo(Entity target) => (_center.position - target.GetTransform().position).magnitude;
    }
}
