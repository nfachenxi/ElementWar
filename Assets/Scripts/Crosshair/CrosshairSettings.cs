using UnityEngine;

namespace Crosshair
{
    /// <summary>
    /// 准星样式枚举
    /// </summary>
    public enum CrosshairStyle
    {
        Cross,    // 十字线
        Dot,      // 点
        Circle,   // 圆形
        Chevron   // V形
    }

    /// <summary>
    /// 准星配置 ScriptableObject
    /// 可在 Editor 中实时调整参数并预览
    /// </summary>
    [CreateAssetMenu(fileName = "CrosshairSettings", menuName = "ElementWar/Crosshair Settings")]
    public class CrosshairSettings : ScriptableObject
    {
        #region 样式

        [Header("样式")]
        [Tooltip("准星形状")]
        public CrosshairStyle style = CrosshairStyle.Cross;

        [Tooltip("准星颜色")]
        public Color color = Color.white;

        [Tooltip("线条长度（像素）")]
        [Range(2f, 64f)]
        public float lineLength = 12f;

        [Tooltip("线条宽度（像素）")]
        [Range(1f, 8f)]
        public float lineThickness = 2f;

        [Tooltip("中心留空距离（像素）")]
        [Range(0f, 32f)]
        public float gapSize = 4f;

        #endregion

        #region 中心点

        [Header("中心点")]
        [Tooltip("是否显示中心点")]
        public bool showCenterDot = false;

        [Tooltip("中心点大小（像素）")]
        [Range(1f, 16f)]
        public float centerDotSize = 3f;

        #endregion

        #region 描边

        [Header("描边")]
        [Tooltip("是否启用描边")]
        public bool outlineEnabled = false;

        [Tooltip("描边颜色")]
        public Color outlineColor = Color.black;

        [Tooltip("描边宽度（像素）")]
        [Range(0.5f, 4f)]
        public float outlineWidth = 1f;

        #endregion

        #region 散布

        [Header("散布")]
        [Tooltip("最小散布范围（像素）")]
        [Range(0f, 128f)]
        public float minSpread = 8f;

        [Tooltip("最大散布范围（像素）")]
        [Range(0f, 256f)]
        public float maxSpread = 64f;

        [Tooltip("每次射击增加的散布量（像素）")]
        [Range(0f, 32f)]
        public float spreadPerShot = 4f;

        [Tooltip("散布恢复速度（每秒像素）")]
        [Range(0f, 128f)]
        public float recoverySpeed = 24f;

        #endregion

        #region 预览（仅 Editor）

        [Header("预览（仅 Editor 生效）")]
        [Tooltip("拖动此滑块预览不同散布阶段的准星形态")]
        [Range(0f, 1f)]
        public float previewSpread = 0f;

        #endregion
    }
}
