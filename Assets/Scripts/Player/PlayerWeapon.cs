using Crosshair;
using UnityEngine;

namespace Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [Tooltip("子弹生成位置")] public Transform bulletSpawnPoint;
        [Tooltip("子弹预制体")] public PlayerWeaponBullet BulletEffectPrefab;
        [Tooltip("枪管火花预制体")] public GameObject bulletSparkPrefab;
        [Tooltip("子弹发射间隔")] public float bulletInterval = 0.15f;

        [Header("准星")]
        [Tooltip("准星配置资产")]
        public CrosshairSettings crosshairSettings;

        [Header("射线检测")]
        [Tooltip("射线检测的层级")]
        public LayerMask aimLayerMask = ~0;
        [Tooltip("射线检测的最大距离")]
        public float maxRayDistance = 1000f;

        private float _lastFireTime;

        /// <summary>
        /// 从枪口发射子弹，方向指向屏幕中心射线命中点（弹道收敛于准星指向）
        /// </summary>
        public void Fire()
        {
            if (Time.time - _lastFireTime < bulletInterval)
                return;

            Camera cam = Camera.main;
            if (cam == null) return;

            _lastFireTime = Time.time;

            // 直接发射一次性射线，获取屏幕中心命中点（消除间接耦合和时序延迟）
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, aimLayerMask))
                targetPoint = hit.point;
            else
                targetPoint = ray.origin + ray.direction * maxRayDistance;

            // 弹道方向：从枪口指向命中点
            Vector3 direction = targetPoint - bulletSpawnPoint.position;
            direction.Normalize();

            // 散布偏移
            if (crosshairSettings != null && CrosshairUI.Instance != null)
            {
                float spreadPixels = CrosshairUI.Instance.GetCurrentSpreadPixels();
                if (spreadPixels > 0f)
                {
                    float screenFraction = spreadPixels / Screen.height;
                    float vFovRad = cam.fieldOfView * Mathf.Deg2Rad;
                    float spreadAngle = screenFraction * vFovRad;

                    Vector3 randomOffset = Random.insideUnitCircle;
                    Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
                    if (perpendicular.sqrMagnitude < 0.01f)
                        perpendicular = Vector3.Cross(direction, Vector3.right).normalized;
                    Vector3 upPerp = Vector3.Cross(direction, perpendicular).normalized;

                    float angleOffset = Random.value * spreadAngle;
                    Vector3 randomDir = randomOffset.x * perpendicular + randomOffset.y * upPerp;
                    direction = (direction + randomDir * Mathf.Tan(angleOffset)).normalized;

                    CrosshairUI.Instance.AddSpread(crosshairSettings.spreadPerShot);
                }
            }

            // 从枪口生成子弹
            PlayerWeaponBullet bulletEffect = Instantiate(BulletEffectPrefab, bulletSpawnPoint.position, Quaternion.identity);
            bulletEffect.transform.forward = direction;

            // 枪口火花
            GameObject spark = Instantiate(bulletSparkPrefab, bulletSpawnPoint.position, Quaternion.identity);
            spark.transform.forward = direction;

#if UNITY_EDITOR
            // 调试可视化：红=弹道, 黄=枪口→命中点, 绿=相机→命中点
            Debug.DrawRay(bulletSpawnPoint.position, direction * 20f, Color.red, 2f);
            Debug.DrawLine(bulletSpawnPoint.position, targetPoint, Color.yellow, 2f);
            Debug.DrawLine(ray.origin, targetPoint, Color.green, 2f);
#endif
        }
    }
}
