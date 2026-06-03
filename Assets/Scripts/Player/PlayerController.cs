using Base;
using Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace Player
{
    /// <summary>
    /// 玩家控制器
    /// </summary>
    public class PlayerController : SingleMonoBase<PlayerController>
    {
        public PlayerModel currentPlayerModel; // 当前所控制的角色模型
        private Transform _cameraTransform;

        [Tooltip("正常视角相机")]
        public CinemachineFreeLook freeLookCamera;
        [Tooltip("瞄准视角相机")]
        public CinemachineFreeLook aimingCamera;
        
        public Camera mainCamera; // 对主相机的引用
        
        #region 玩家输入相关
        
        private MyInputSystem _input; // 输入系统
        [HideInInspector] public Vector2 moveInput; // 移动输入
        [HideInInspector] public bool isSprint; // 冲刺输入
        [HideInInspector] public bool isAiming; // 瞄准输入
        [HideInInspector] public bool isJumping; // 跳跃输入
        [HideInInspector] public bool isFire; // 开火输入
        
        #endregion

        #region 瞄准相关
        
        [Tooltip("瞄准目标")] public Transform aimTarget;
        [Tooltip("射线检测的最大距离")] public float maxRayDistance = 1000f;
        [Tooltip("射线检测的层级")] public LayerMask aimLayerMask = ~0;

        #endregion

        #region 开火抖动

        private CinemachineImpulseSource _impulseSource;

        #endregion
        
        [Tooltip("转向速度")]
        public float rotationSpeed = 300;

        [HideInInspector]
        public Vector3 localMovement; // 本地空间下的玩家移动方向
        [HideInInspector]
        public Vector3 worldMovement; // 世界空间下的玩家移动方向

        
        
        protected override void Awake()
        {
            base.Awake();
            _input = new MyInputSystem();
            ExitAim();
        }
        
        // Start is called before the first frame update
        void Start()
        {
            if (Camera.main != null)
            {
                mainCamera = Camera.main;
                _cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogError("PlayerController无法在场景中找到主摄像机");
            }
            Cursor.lockState = CursorLockMode.Locked;
            ExitAim();
            _impulseSource = aimingCamera.GetComponent<CinemachineImpulseSource>();
        }

        // Update is called once per frame
        void Update()
        {
            # region 更新玩家输入
            
            moveInput = _input.Player.Move.ReadValue<Vector2>().normalized;
            isSprint = _input.Player.IsSprint.IsPressed();
            isAiming = _input.Player.IsAiming.IsPressed();
            isJumping = _input.Player.IsJumping.IsPressed();
            isFire =  _input.Player.Fire.IsPressed();
            
            # endregion

            #region 计算玩家移动方向

            // 获取相机的方向向量
            Vector3 cameraForwardProjection = new Vector3(_cameraTransform.forward.x, 0, _cameraTransform.forward.z);
            // 计算世界空间下的方向向量
            worldMovement = cameraForwardProjection * moveInput.y + _cameraTransform.right * moveInput.x;
            // 将世界空间下的方向向量转化为模型本地空间下的方向向量
            localMovement = currentPlayerModel.transform.InverseTransformVector(worldMovement);

            #endregion
        }

        /// <summary>
        /// 进入瞄准
        /// </summary>
        public void EnterAim()
        {
            // 同步瞄准相机与自由相机的旋转角度
            aimingCamera.m_XAxis.Value = freeLookCamera.m_XAxis.Value;
            aimingCamera.m_YAxis.Value = freeLookCamera.m_YAxis.Value;

            // 启动瞄准约束
            currentPlayerModel.rightHandAimConstraint.weight = 1;
            currentPlayerModel.bodyAimConstraint.weight = 1;
            currentPlayerModel.rightHandConstraint.weight = 0;

            // 设置相机优先级，使瞄准相机生效
            freeLookCamera.Priority = 0;
            aimingCamera.Priority = 100;

            // 显示准星
            if (Crosshair.CrosshairUI.Instance != null)
                Crosshair.CrosshairUI.Instance.Show();
        }

        /// <summary>
        /// 退出瞄准
        /// </summary>
        public void ExitAim()
        {
            // 同步自由相机与瞄准相机的旋转角度
            freeLookCamera.m_XAxis.Value = aimingCamera.m_XAxis.Value;
            freeLookCamera.m_YAxis.Value = aimingCamera.m_YAxis.Value;

            // 关闭瞄准约束
            currentPlayerModel.rightHandAimConstraint.weight = 0;
            currentPlayerModel.bodyAimConstraint.weight = 0;
            currentPlayerModel.rightHandConstraint.weight = 1;

            // 设置相机优先级，使自由相机生效
            aimingCamera.Priority = 0;
            freeLookCamera.Priority = 100;

            // 隐藏准星
            if (Crosshair.CrosshairUI.Instance != null)
                Crosshair.CrosshairUI.Instance.Hide();
        }

        /// <summary>
        /// 抖动屏幕
        /// </summary>
        public void ShakeCamera()
        {
            _impulseSource.GenerateImpulse();
        }
        
        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }
    }
}
