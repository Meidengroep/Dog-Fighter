using UnityEngine;
using System.Collections;

public enum Layers
{
    ////////////////
    /// Warning! ///
    ////////////////
    /// Do not change existing names and numbers through refractoring only!
    /// These definitions are associated with the layers in Unity.
    /// Changes to existing values must be changed in both places. 

    ///////////////////////
    /// Built in layers ///
    ///////////////////////
    Default = 0,
    TransparentFX = 1,
    IgnoreRayCast = 2,
    Water = 4,

    /////////////////////
    /// Custom layers ///
    /////////////////////

    Team1Actor = 8,
    Team1Mothership = 9,
    Team1Projectile = 10,
    Team2Actor = 11,
    Team2Mothership = 12,
    Team2Projectile = 13,
    Obstacles = 14,
    Scenery = 15,
    Boundary = 16,
}
