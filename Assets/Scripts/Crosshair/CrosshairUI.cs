using UnityEngine;
using UnityEngine.UI;

namespace Crosshair
{
    /// <summary>
    /// 准星 UI 组件
    /// 挂载在 Canvas GameObject 上，实现设计时预览 + 运行时动态散布
    /// </summary>
    [ExecuteAlways]
    public class CrosshairUI : MonoBehaviour
    {
        #region 单例

        public static CrosshairUI Instance { get; private set; }

        #endregion

        #region 引用

        [Header("配置")]
        [Tooltip("准星配置资产")]
        public CrosshairSettings settings;

        #endregion

        #region 子对象引用

        private RectTransform _topLine;
        private RectTransform _bottomLine;
        private RectTransform _leftLine;
        private RectTransform _rightLine;
        private RectTransform _centerDot;
        private RectTransform _circle;

        private Image _topImage;
        private Image _bottomImage;
        private Image _leftImage;
        private Image _rightImage;
        private Image _centerDotImage;
        private Image _circleImage;

        #endregion

        #region 运行时状态

        private float _currentSpread;
        private CrosshairSettings _lastSettings;
        private CanvasGroup _canvasGroup;

        #endregion

        #region 生命周期

        private void Awake()
        {
            if (Application.isPlaying)
                Instance = this;
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
                Instance = this;

            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            EnsureChildObjects();
            ApplyAllSettings();
        }

        private void OnDisable()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Update()
        {
            if (settings == null) return;

            // 检测 settings 资产是否被替换
            if (_lastSettings != settings)
            {
                ApplyAllSettings();
                _lastSettings = settings;
            }

            // 运行时：散布恢复
            if (Application.isPlaying)
            {
                RecoverSpread();
            }

            // 根据当前散布更新线条位置
            float spread = GetEffectiveSpread();
            UpdateLinePositions(spread);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (isActiveAndEnabled)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this != null && isActiveAndEnabled)
                    {
                        EnsureChildObjects();
                        ApplyAllSettings();
                    }
                };
            }
        }
#endif

        #endregion

        #region 子对象创建

        private void EnsureChildObjects()
        {
            _topLine    = EnsureLine("Crosshair_Top",    out _topImage);
            _bottomLine = EnsureLine("Crosshair_Bottom", out _bottomImage);
            _leftLine   = EnsureLine("Crosshair_Left",   out _leftImage);
            _rightLine  = EnsureLine("Crosshair_Right",  out _rightImage);
            _centerDot  = EnsureChild("Crosshair_CenterDot", out _centerDotImage);
            _circle     = EnsureChild("Crosshair_Circle",    out _circleImage);
        }

        private RectTransform EnsureLine(string name, out Image image)
        {
            RectTransform rt = EnsureChild(name, out image);
            image.raycastTarget = false;
            return rt;
        }

        private RectTransform EnsureChild(string name, out Image image)
        {
            Transform existing = transform.Find(name);
            if (existing != null)
            {
                RectTransform rt = existing as RectTransform;
                image = existing.GetComponent<Image>();
                if (image == null)
                    image = existing.gameObject.AddComponent<Image>();
                return rt;
            }

            GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(transform, false);
            RectTransform result = go.transform as RectTransform;
            image = go.GetComponent<Image>();
            image.raycastTarget = false;
            return result;
        }

        #endregion

        #region 外观

        private void ApplyAllSettings()
        {
            if (settings == null) return;

            ApplyVisibility();
            ApplyColors();
            ApplyLineSizes();
        }

        private void ApplyVisibility()
        {
            bool showCross = settings.style == CrosshairStyle.Cross || settings.style == CrosshairStyle.Chevron;
            bool showDot   = settings.style == CrosshairStyle.Dot   || settings.showCenterDot;
            bool showCircle = settings.style == CrosshairStyle.Circle;

            SetActive(_topLine, showCross);
            SetActive(_bottomLine, showCross);
            SetActive(_leftLine, showCross);
            SetActive(_rightLine, showCross);
            SetActive(_centerDot, showDot);
            SetActive(_circle, showCircle);
        }

        private void SetActive(RectTransform rt, bool active)
        {
            if (rt != null) rt.gameObject.SetActive(active);
        }

        private void ApplyColors()
        {
            Color c = settings.color;
            Color outlineC = settings.outlineColor;

            SetImageColor(_topImage, c);
            SetImageColor(_bottomImage, c);
            SetImageColor(_leftImage, c);
            SetImageColor(_rightImage, c);
            SetImageColor(_centerDotImage, c);
            SetImageColor(_circleImage, new Color(c.r, c.g, c.b, 0.3f));
        }

        private void SetImageColor(Image img, Color c)
        {
            if (img != null) img.color = c;
        }

        private void ApplyLineSizes()
        {
            if (_centerDot != null)
                _centerDot.sizeDelta = new Vector2(settings.centerDotSize, settings.centerDotSize);
        }

        #endregion

        #region 线条位置更新

        private void UpdateLinePositions(float spread)
        {
            float totalOffset = settings.gapSize + spread;

            SetLine(_topLine,    new Vector2(0,  totalOffset),  new Vector2(settings.lineThickness, settings.lineLength), new Vector2(0.5f, 0f));
            SetLine(_bottomLine, new Vector2(0, -totalOffset),  new Vector2(settings.lineThickness, settings.lineLength), new Vector2(0.5f, 1f));
            SetLine(_leftLine,   new Vector2(-totalOffset, 0),  new Vector2(settings.lineLength, settings.lineThickness), new Vector2(1f, 0.5f));
            SetLine(_rightLine,  new Vector2( totalOffset, 0),  new Vector2(settings.lineLength, settings.lineThickness), new Vector2(0f, 0.5f));

            // 圆形模式
            if (_circle != null && _circle.gameObject.activeSelf)
            {
                float size = totalOffset * 2f + settings.lineThickness;
                _circle.sizeDelta = new Vector2(size, size);
            }
        }

        private void SetLine(RectTransform lineRt, Vector2 anchoredPos, Vector2 sizeDelta, Vector2 pivot)
        {
            if (lineRt == null) return;

            lineRt.anchorMin = Vector2.one * 0.5f;
            lineRt.anchorMax = Vector2.one * 0.5f;
            lineRt.pivot = pivot;
            lineRt.anchoredPosition = anchoredPos;
            lineRt.sizeDelta = sizeDelta;
        }

        #endregion

        #region 运行时散布

        private float GetEffectiveSpread()
        {
            if (Application.isPlaying)
                return _currentSpread;

            // Editor 模式：previewSpread 在 minSpread~maxSpread 之间插值
            return Mathf.Lerp(settings.minSpread, settings.maxSpread, settings.previewSpread);
        }

        private void RecoverSpread()
        {
            if (_currentSpread > settings.minSpread)
            {
                _currentSpread -= settings.recoverySpeed * Time.deltaTime;
                if (_currentSpread < settings.minSpread)
                    _currentSpread = settings.minSpread;
            }
        }

        /// <summary>
        /// 增加散布（射击时调用）
        /// </summary>
        public void AddSpread(float amount)
        {
            _currentSpread += amount;
            if (_currentSpread > settings.maxSpread)
                _currentSpread = settings.maxSpread;
        }

        /// <summary>
        /// 获取当前散布值（像素），供武器系统计算实际散布角度
        /// </summary>
        public float GetCurrentSpreadPixels()
        {
            return _currentSpread;
        }

        /// <summary>
        /// 显示准星
        /// </summary>
        public void Show()
        {
            if (_canvasGroup != null)
                _canvasGroup.alpha = 1f;

            // 确保散布不低于最小值
            if (settings != null && _currentSpread < settings.minSpread)
                _currentSpread = settings.minSpread;
        }

        /// <summary>
        /// 隐藏准星
        /// </summary>
        public void Hide()
        {
            if (_canvasGroup != null)
                _canvasGroup.alpha = 0f;
        }

        #endregion
    }
}
