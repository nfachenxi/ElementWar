using Base;

namespace Enemy.State
{
    public class ZombieAttackState : EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            EnemyModel.PlayerStateAnimation("Attack");
        }
    }
}