using Player;
using UnityEngine;
using Utils;

namespace Base
{
    /// <summary>
    /// 玩家状态基类
    /// </summary>
    public class PlayerStateBase : StateBase
    {
        protected PlayerController PlayerController;
        protected PlayerModel PlayerModel; // 当前状态的角色模型
        public override void Init(IStateMachineOwner owner)
        {
            PlayerController = PlayerController.INSTANCE;
            PlayerModel = (PlayerModel) owner;
        }

        public override void Enter()
        {
            MonoManager.INSTANCE.AddUpdateAction(Update);
        }
 
        public override void Exit()
        {
            MonoManager.INSTANCE.RemoveUpdateAction(Update);
        }

        public override void Destory()
        {

        }

        public override void Update()
        {
            #region 重力计算

            if (!PlayerModel.cc.isGrounded) // 模型不在地面
            {
                PlayerModel.verticalSpeed += PlayerModel.gravity * Time.deltaTime; // 施加重力
                if (PlayerModel.IsHover())
                    PlayerModel.SwitchState(PlayerState.Hover);
            }
            else // 模型在地面上
                PlayerModel.verticalSpeed = PlayerModel.gravity * Time.deltaTime; // 重置垂直速度
            #endregion

            #region 瞄准状态监听

            if (PlayerController.isAiming || PlayerController.isFire)
                PlayerModel.SwitchState(PlayerState.Aiming);

            #endregion
        }

        /// <summary>
        /// 当前模型是否被玩家控制
        /// </summary>
        public bool IsBeControl()
        {
            return PlayerModel == PlayerController.currentPlayerModel;
        }

        /// <summary>
        /// 切换到跳跃状态
        /// </summary>
        public void SwitchToHover()
        {
            // 计算跳跃力度
            PlayerModel.verticalSpeed = Mathf.Sqrt(-2 * PlayerModel.gravity * PlayerModel.jumpHeight);
            // 切换到悬空状态
            PlayerModel.SwitchState(PlayerState.Hover);
        }
    }
}