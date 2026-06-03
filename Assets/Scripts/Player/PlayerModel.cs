using Player.State;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Utils;

namespace Player
{
    public enum PlayerState
    {
        Idle,
        Move,
        Hover,
        Aiming
    }
    
    /// <summary>
    /// 角色模型
    /// </summary>
    public class PlayerModel : MonoBehaviour, IStateMachineOwner
    {
        [Tooltip("角色武器")] public PlayerWeapon weapon;
        
        [HideInInspector]
        public Animator animator;
        public CharacterController cc;
        private StateMachine _stateMachine; // 动画状态机
        private PlayerState _currentState; // 当前状态

        #region 约束相关

        public TwoBoneIKConstraint rightHandConstraint; // 正常情况下的右手约束
        public MultiAimConstraint rightHandAimConstraint; // 瞄准情况下的右手约束
        public MultiAimConstraint bodyAimConstraint; // 瞄准情况下的身体约束

        #endregion
        
        #region 垂直速度相关
        
        [Tooltip("重力")] public float gravity = -15;
        [Tooltip("跳跃高度")] public float jumpHeight = 1.5f;
        [HideInInspector] public float verticalSpeed; // 当前垂直方向的速度
        [Tooltip("悬空的判定高度")]public float fallHeight = 0.2f;
        
        #endregion

        #region 玩家在地面时前三帧速度的缓存

        private static readonly int CacheSize = 3;
        private Vector3[] _speedCache = new Vector3[CacheSize]; // 动画前三帧的玩家速度
        private int _speedCacheIndex = 0; // 缓存保存的位置
        private Vector3 _averageDeltaMovement; // 平均速度

        #endregion
        
        private void Awake()
        {
            _stateMachine = new StateMachine(this);
            animator = GetComponent<Animator>();
            cc = GetComponent<CharacterController>();
        }
        
        // Start is called before the first frame update
        void Start()
        {
            SwitchState(PlayerState.Idle);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="state">状态</param>
        public void SwitchState(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Idle:
                    _stateMachine.EnterState<PlayerIdleState>();
                    break;
                case PlayerState.Move:
                    _stateMachine.EnterState<PlayerMoveState>();
                    break;
                case PlayerState.Hover:
                    _stateMachine.EnterState<PlayerHoverState>();
                    break;
                case PlayerState.Aiming:
                    _stateMachine.EnterState<PlayerAimingState>();
                    break;
            }
            _currentState = state;
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="animationName">动画名称</param>
        /// <param name="transition">过渡时间</param>
        /// <param name="layer">动画层</param>
        public void PlayerStateAnimation(string animationName, float transition = 0.25f, int layer = 0)
        {
            animator.CrossFadeInFixedTime(animationName, transition, layer);
        }

        /// <summary>
        /// 是否悬空
        /// </summary>
        /// <returns></returns>
        public bool IsHover()
        {
            return !Physics.Raycast(transform.position, Vector3.down, fallHeight);
        }

        /// <summary>
        /// 计算模型前三帧平均速度
        /// </summary>
        /// <param name="newSpeed">当前速度</param>
        private void UpdateAverageCacheSpeed(Vector3 newSpeed)
        {
            _speedCache[_speedCacheIndex++] = newSpeed;
            _speedCacheIndex %= CacheSize;
            // 计算缓存池中的平均速度
            Vector3 sum = Vector3.zero;
            foreach (Vector3 cache in _speedCache)
                sum += cache;
            _averageDeltaMovement = sum / CacheSize;
        }

        private void OnAnimatorMove()
        {
            Vector3 playerDeltaMovement = animator.deltaPosition; // 获取动画控制器当前帧的位置信息
            if (_currentState != PlayerState.Hover)
            {
                UpdateAverageCacheSpeed(animator.velocity);
            }
            else
            {
                playerDeltaMovement = _averageDeltaMovement * Time.deltaTime;
            }
            playerDeltaMovement.y = verticalSpeed * Time.deltaTime;
            cc.Move(playerDeltaMovement);
        }
    }
}
