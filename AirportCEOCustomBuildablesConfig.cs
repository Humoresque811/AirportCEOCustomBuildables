using BepInEx.Configuration;
using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AirportCEOCustomBuildables;

internal class AirportCEOCustomBuildablesConfig
{
    public static ConfigEntry<bool> useRandomRotation { get; private set; }

    internal static void SetUpConfig()
    {
        useRandomRotation = AirportCEOCustomBuildables.ConfigReference.Bind<bool>("General", "Use Random Rotation", true, "When placing new custom buildables that are square" +
            ", should they be randomly rotated. Enabled is recomended.");
    }
}
