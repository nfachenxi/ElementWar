using System;
using Base;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// 任务处理管理器
    /// </summary>
    public class MonoManager : SingleMonoBase<MonoManager>
    {
        private Action _updateAction; //  任务集合

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task">事件</param>
        public void AddUpdateAction(Action task)
        {
            _updateAction += task;
        }

        public void RemoveUpdateAction(Action task)
        {
            _updateAction -= task;
        }

        private void Update()
        {
            _updateAction?.Invoke();
        }
    }
}