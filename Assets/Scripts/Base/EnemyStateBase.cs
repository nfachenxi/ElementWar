using UnityEngine;
using Utils;

namespace Base
{
    /// <summary>
    /// 敌人状态基类
    /// </summary>
    public class EnemyStateBase : StateBase
    {
        protected EnemyBase EnemyModel;
        public override void Init(IStateMachineOwner owner)
        {
            EnemyModel = (EnemyBase) owner;
        }

        public override void Enter()
        {
            
        }

        public override void Exit()
        {
            
        }

        public override void Destory()
        {
            
        }

        public override void Update()
        {
            
        }
    }
}