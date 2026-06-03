using System;
using UnityEngine;

namespace Player
{
    public class PlayerWeaponBullet : MonoBehaviour
    {
        [Tooltip("子弹伤害")] public int damage = 10;
        [HideInInspector] public Rigidbody rb;
        [Tooltip("推力")] public float flyPower = 30f;
        [Tooltip("存活时间")] public float lifeTime = 10f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = transform.forward * flyPower;
            Destroy(gameObject, lifeTime);
        }
    }
}