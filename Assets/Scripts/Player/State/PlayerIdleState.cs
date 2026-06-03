using Base;

namespace Player.State
{
    /// <summary>
    /// 玩家待机状态
    /// </summary>
    public class PlayerIdleState : PlayerStateBase
    {
        public override void Enter()
        {
            base.Enter();
            PlayerModel.PlayerStateAnimation("Idle");
        }

        public override void Update()
        {
            base.Update();
            if (IsBeControl())
            {
                #region 移动状态监听

                if (PlayerController.moveInput.magnitude != 0)
                    PlayerModel.SwitchState(PlayerState.Move);

                #endregion
                
                #region 浮空状态监听
                
                if (PlayerController.isJumping)
                    SwitchToHover();
                
                #endregion
            }
        }
    }
}