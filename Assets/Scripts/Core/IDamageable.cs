using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Core
{
    /// <summary>
    /// 可受伤接口 - 任何可以被攻击的对象都实现这个
    /// </summary>
    public interface IDamageable 
    {
        void TakeDamage(float damage);
        bool IsAlive { get; }
        Transform Transform { get; }
    }
}
