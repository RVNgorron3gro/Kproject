using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MasterList", menuName = "Data/Player/MasterList", order = 1)]
public class MasterList : ScriptableObject
{
    [System.Serializable]
    public struct Contained
    {
        [Header("Required")]
        public string title;
        public Sprite image;
        public Defs.ItemCategory category;
        public int stackLimit;
        public float weight;
        public Effector costEffector;
        public Effector selfEffector;
        public Effector selfOnHitEffector;
        public Effector targetEffector;

        [Header("Weapon Specific")]
        public Defs.WeaponType weaponType;

        //General
        public GameObject weaponObject;
        public AudioClip[] windupSound;
        public AudioClip[] strikeSound;
        public AudioClip[] hitSound;
        public float windupTime;
        public float strikeTime;
        public float recoveryTime;

        //Projectile
        public GameObject projectileObject;
        public AudioClip[] flightSound;
        public float range;
        public float speed;
        [Tooltip("0 for infinity")] public int targetLimit;

        //Shield
        public AudioClip[] blockSound;
        [Range(0, 360)]
        public int blockAngle;

        [Header("Consumeable Specific")]
        public float usageDelay;
        public bool isProjectileWeapon;
    }
    public List<Contained> c;
}