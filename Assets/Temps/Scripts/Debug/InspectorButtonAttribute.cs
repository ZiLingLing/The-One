using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public readonly string Name;

        public InspectorButtonAttribute()
        {

        }

        public InspectorButtonAttribute(string name)
        {
            Name = name;
        }
    }
}

