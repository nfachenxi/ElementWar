using Base;

namespace Enemy.State
{
    public class ZombieMoveState : EnemyStateBase
    {
        public override void Enter()
        {
            base.Enter();
            EnemyModel.PlayerStateAnimation("Move");
        }
    }
}