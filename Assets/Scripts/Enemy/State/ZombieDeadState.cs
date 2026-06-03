using Base;

namespace Enemy.State
{
    public class ZombieDeadState : EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            EnemyModel.PlayerStateAnimation("Dead");
        }
    }
}