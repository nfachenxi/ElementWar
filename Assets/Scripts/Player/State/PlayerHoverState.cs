using Base;

namespace Player.State
{
    /// <summary>
    /// 悬空状态
    /// </summary>
    public class PlayerHoverState : PlayerStateBase
    {
        public override void Enter()
        {
            base.Enter();
            PlayerModel.PlayerStateAnimation("Hover");
        }

        public override void Update()
        {
            base.Update();

            #region 检测角色是否落到地面上

            if (PlayerModel.cc.isGrounded)
                PlayerModel.SwitchState(PlayerState.Idle);

            #endregion
        }
    }
}