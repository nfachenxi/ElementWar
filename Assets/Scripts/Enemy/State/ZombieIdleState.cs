using Base;

namespace Enemy.State
{
    public class ZombieIdleState : EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            EnemyModel.PlayerStateAnimation("Idle");
        }
    }
}