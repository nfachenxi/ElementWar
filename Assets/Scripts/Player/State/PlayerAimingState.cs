using Base;
using UnityEngine;

namespace Player.State
{
    /// <summary>
    /// 瞄准状态
    /// </summary>
    public class PlayerAimingState : PlayerStateBase
    {
        #region 动画器相关属性

        private static readonly int AimingXHash = Animator.StringToHash("AimingX");
        private static readonly int AimingYHash = Animator.StringToHash("AimingY");
        private float _aimingX = 0;
        private float _aimingY = 0;
        private float _transitionSpeed = 5;
        
        #endregion
        
        public override void Enter()
        {
            base.Enter();
            PlayerModel.PlayerStateAnimation("Aiming");
            if (IsBeControl())
            {
                PlayerController.EnterAim();
                UpdateAimingTarget();
            }
        }

        public override void Update()
        {
            base.Update();

            if (IsBeControl())
            {
                // 让模型立刻旋转到相机方向
                PlayerModel.transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
                UpdateAimingTarget();
                
                #region 退出瞄准监听

                if (!PlayerController.isAiming && !PlayerController.isFire)
                {
                    PlayerModel.SwitchState(PlayerState.Idle);
                    return;
                }
                
                #endregion

                #region 开火监听

                if (PlayerController.isFire)
                {
                    PlayerModel.weapon.Fire();
                    PlayerController.ShakeCamera();
                }

                #endregion
            
            
                #region 处理移动输入

                _aimingX = Mathf.Lerp(_aimingX, PlayerController.moveInput.x, _transitionSpeed * Time.deltaTime);
                _aimingY =  Mathf.Lerp(_aimingY, PlayerController.moveInput.y, _transitionSpeed * Time.deltaTime);
                PlayerModel.animator.SetFloat(AimingXHash, _aimingX);
                PlayerModel.animator.SetFloat(AimingYHash, _aimingY);

                #endregion
            }
        }

        public override void Exit()
        {
            base.Exit();
            if (IsBeControl())
                PlayerController.ExitAim();
        }
        
        /// <summary>
        /// 从屏幕中心发射射线确认瞄准位置
        /// </summary>
        private void UpdateAimingTarget()
        {
            // 从主摄像机屏幕中心点（视口坐标 0.5, 0.5）发射一条射线
            Ray ray = PlayerController.mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            
            // 如果射线击中了物体（根据设置的LayerMask）
            if (Physics.Raycast(ray, out hit, PlayerController.maxRayDistance, PlayerController.aimLayerMask))
            {
                // 更新瞄准目标位置为射线击中的点
                PlayerController.aimTarget.position = hit.point;
            }
            else
            {
                // 如果射线没有击中任何物体，将目标点设置在射线的最大距离处
                PlayerController.aimTarget.position = ray.origin + ray.direction * PlayerController.maxRayDistance;
            }
        }


    }
    
}