﻿namespace DestroyIt
{
    public enum Tag
    {
        ClingingDebris = 0,
        ClingPoint = 7,
        Concrete = 1,
        Glass = 2,
        MaterialTransferred = 3,
        Metal = 4,
        Paper = 5,
        Wood = 6,
        Powered = 8,
        Pooled = 9,
        Untagged = 10,
        DestructibleGroup = 11,
        Rubber = 12,
        Stuffing = 13,
        Plastic = 14
    }

    [System.Flags]
    public enum HitBy
    {
        Bullet = (1 << 0),
        Cannonball = (1 << 1),
        Axe = (1 << 2)
    }

    public enum FacingDirection
    {
        None,
        FollowedObject,
        FixedPosition
    }

    public enum WeaponType
    {
        Cannonball = 0,
        Rocket = 1,
        Gun,
        Nuke,
        Melee,
        RepairWrench
    }

    public enum MoveDirection
    {
        Up,
        Down
    }

    public enum DestructionType
    {
        //TODO: Possible future limited destruction types for things beyond specified camera distance:
        //None,
        //FiftyPercentDebris,
        ParticleEffect = 0
    }
}