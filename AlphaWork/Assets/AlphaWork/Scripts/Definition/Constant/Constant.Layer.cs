using UnityEngine;

namespace AlphaWork
{
    public static partial class Constant
    {
        /// <summary>
        /// 层。
        /// </summary>
        public static class Layer
        {
            public const string DefaultLayerName = "Default";
            public static readonly int DefaultLayerId = LayerMask.NameToLayer(DefaultLayerName);

            public const string UILayerName = "UI";
            public static readonly int UILayerId = LayerMask.NameToLayer(UILayerName);

            public const string TargetableObjectLayerName = "Targetable Object";
            public static readonly int TargetableObjectLayerId = LayerMask.NameToLayer(TargetableObjectLayerName);

            public const string EffectLayerName = "EffectObject";
            public static readonly int EffectLayerId = LayerMask.NameToLayer(EffectLayerName);

            public const string WeaponLayerName = "WeaponAttach";
            public static readonly int WeaponLayerId = LayerMask.NameToLayer(WeaponLayerName);
        }
    }
}
