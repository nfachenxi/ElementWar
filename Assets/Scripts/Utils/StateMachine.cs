using System;
using System.Collections.Generic;
using Base;

namespace Utils
{
    public interface IStateMachineOwner {} // 状态机宿主机

    /// <summary>
    /// 角色状态机
    /// </summary>
    public class StateMachine
    {
        private StateBase _currentState; // 当前状态
        private IStateMachineOwner _owner;
        private Dictionary<Type, StateBase> _stateDic = new  Dictionary<Type, StateBase>(); // 状态字典

        public StateMachine(IStateMachineOwner owner)
        {
            this._owner = owner;
        }

        /// <summary>
        /// 进入动画状态
        /// </summary>
        /// <typeparam name="T">状态类</typeparam>
        public void EnterState<T>() where T : StateBase, new()
        {
            // 防止重复进入同一个动画状态
            if (_currentState is T) return; 
            _currentState?.Exit();
            _currentState = LoadState<T>();
            _currentState.Enter();
        }

        /// <summary>
        /// 尝试从字典中取出状态
        /// </summary>
        /// <typeparam name="T">状态类</typeparam>
        /// <returns>状态实例</returns>
        private StateBase LoadState<T>() where T : StateBase, new()
        {
            Type stateType = typeof(T);
            // 如果状态字典里没有该状态
            if (!_stateDic.TryGetValue(stateType, out StateBase state))
            {
                state = new T();
                state.Init(_owner);
                _stateDic.Add(stateType, state); // 将新创建的状态记录到字典里
            }
            return state;
        }

        /// <summary>
        /// 退出状态机
        /// </summary>
        public void Stop()
        {
            _currentState?.Exit();
            foreach(var state in _stateDic.Values)
                state?.Destory();
            _stateDic.Clear();
        }
    }
}
