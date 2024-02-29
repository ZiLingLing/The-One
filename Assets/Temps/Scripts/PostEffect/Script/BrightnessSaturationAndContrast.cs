using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Roguelike
{
    public class BrightnessSaturationAndContrast : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter m_brightness = new ClampedFloatParameter(1f, 0, 3);
        public ClampedFloatParameter m_saturation = new ClampedFloatParameter(1f, 0, 3);
        public ClampedFloatParameter m_contrast = new ClampedFloatParameter(1f, 0, 3);

        public bool IsActive()
        {
            return active;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }

}
