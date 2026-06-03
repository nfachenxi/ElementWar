using System;
using UnityEngine;
using Utils;

namespace Base
{
    public enum EnemyState
    {
        Idle, Move, Attack, Dead
    }
    
    public abstract class EnemyBase : MonoBehaviour, IStateMachineOwner
    {
        [HideInInspector]
        public Animator animator;
        protected StateMachine StateMachine;

        protected virtual void Awake()
        {
            StateMachine = new StateMachine(this);
            animator = GetComponent<Animator>();
        }

        protected virtual void Start()
        {
            SwitchState(EnemyState.Idle);
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="state"></param>
        public abstract void SwitchState(EnemyState state);

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

    }
}