using System;
using Base;
using UnityEngine;

namespace Player.State
{
    /// <summary>
    /// 移动状态
    /// </summary>
    public class PlayerMoveState : PlayerStateBase
    {
        private static readonly int MoveBlend = Animator.StringToHash("MoveBlend");

        #region 动画器相关参数

        private float _moveBlend; // 混合器参数
        private readonly float _runThreshold = 0; // 奔跑阈值
        private readonly float _sprintThreshold = 1; // 冲刺阈值
        private readonly float _transitionSpeed = 5; // 过渡速度
        
        #endregion
        public override void Enter()
        {
            base.Enter();
            PlayerModel.PlayerStateAnimation("Move");
        }

        public override void Update()
        {
            base.Update();
            if (IsBeControl())
            {
                #region 浮空状态监听

                if (PlayerController.isJumping)
                {
                    SwitchToHover();
                    return;
                }
                
                #endregion
                
                #region 待机监听

                if (PlayerController.moveInput.magnitude == 0)
                {
                    PlayerModel.SwitchState(PlayerState.Idle);
                }

                #endregion

                #region 处理移动速度

                _moveBlend = Mathf.Lerp(_moveBlend, PlayerController.isSprint ? _sprintThreshold : _runThreshold,
                    _transitionSpeed * Time.deltaTime);
                PlayerModel.animator.SetFloat(MoveBlend, _moveBlend);

                #endregion

                #region 处理角色方向问题

                // 计算本地空间移动方向与模型正前方之间的夹角
                float rad = Mathf.Atan2(PlayerController.localMovement.x, PlayerController.localMovement.z);
                // 旋转到移动方向
                PlayerModel.transform.Rotate(0, rad * PlayerController.rotationSpeed * Time.deltaTime, 0);

                #endregion
            }
            
        }
        
    }
}
