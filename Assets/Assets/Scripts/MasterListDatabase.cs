using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterListDatabase : MonoBehaviour
{
    [HideInInspector] public static MasterListDatabase i;

    public MasterList masterList;
    public List<Item> database;

    void Awake()
    {
        if (!i)
        {
            i = this;
        }
        else
        {
            Debug.LogError(this + " : THERE ARE MULTIPLE INSTANCES OF THIS SCRIPT");
            Destroy(this);
        }
    }

    void Start()
    {
        ConstructDatabase();
    }

    public Item FetchItem(int ID)
    {
        return database[ID];
    }

    public Item FetchItem(string Name)
    {
        for (int count = 0; count < database.Count; count++)
        {
            if (database[count].Title == Name)
            {
                return database[count];
            }
        }
        return null;
    }

    public void ConstructDatabase()
    {
        for (int count = 0; count < masterList.c.Count; count++)
        {
            Item newItem = null;

            switch (masterList.c[count].category)
            {
                case Defs.ItemCategory.Weapon:
                    switch (masterList.c[count].weaponType)
                    {
                        case Defs.WeaponType.Melee:
                            newItem = new Item(count, masterList.c[count].title,
                                masterList.c[count].image,
                                Defs.ItemCategory.Weapon,
                                masterList.c[count].stackLimit,
                                masterList.c[count].weight,
                                Defs.WeaponType.Melee,
                                masterList.c[count].weaponObject,
                                masterList.c[count].windupSound,
                                masterList.c[count].strikeSound,
                                masterList.c[count].hitSound,
                                masterList.c[count].costEffector,
                                masterList.c[count].selfEffector,
                                masterList.c[count].selfOnHitEffector,
                                masterList.c[count].targetEffector,
                                masterList.c[count].windupTime,
                                masterList.c[count].strikeTime,
                                masterList.c[count].recoveryTime);
                            break;
                        case Defs.WeaponType.Projectile:
                            newItem = new Item(count, masterList.c[count].title,
                                masterList.c[count].image,
                                Defs.ItemCategory.Weapon,
                                masterList.c[count].stackLimit,
                                masterList.c[count].weight,
                                Defs.WeaponType.Projectile,
                                masterList.c[count].weaponObject,
                                masterList.c[count].windupSound,
                                masterList.c[count].strikeSound,
                                masterList.c[count].hitSound,
                                masterList.c[count].flightSound,
                                masterList.c[count].costEffector,
                                masterList.c[count].selfEffector,
                                masterList.c[count].selfOnHitEffector,
                                masterList.c[count].targetEffector,
                                masterList.c[count].windupTime,
                                masterList.c[count].strikeTime,
                                masterList.c[count].recoveryTime,
                                masterList.c[count].projectileObject,
                                masterList.c[count].range,
                                masterList.c[count].speed,
                                masterList.c[count].targetLimit);
                            break;
                        case Defs.WeaponType.Shield:
                            newItem = new Item(count, masterList.c[count].title,
                                masterList.c[count].image,
                                Defs.ItemCategory.Weapon,
                                masterList.c[count].stackLimit,
                                masterList.c[count].weight,
                                Defs.WeaponType.Shield,
                                masterList.c[count].weaponObject,
                                masterList.c[count].windupSound,
                                masterList.c[count].strikeSound,
                                masterList.c[count].costEffector,
                                masterList.c[count].selfEffector,
                                masterList.c[count].selfOnHitEffector,
                                masterList.c[count].targetEffector,
                                masterList.c[count].windupTime,
                                masterList.c[count].strikeTime,
                                masterList.c[count].recoveryTime,
                                masterList.c[count].blockSound,
                                masterList.c[count].blockAngle);
                            break;
                    }
                    break;
                case Defs.ItemCategory.Consumeable:
                    newItem = new Item(count, masterList.c[count].title,
                        masterList.c[count].image,
                        Defs.ItemCategory.Consumeable,
                        masterList.c[count].stackLimit,
                        masterList.c[count].weight,
                        masterList.c[count].costEffector,
                        masterList.c[count].selfEffector,
                        masterList.c[count].selfOnHitEffector,
                        masterList.c[count].targetEffector,
                        masterList.c[count].usageDelay,
                        masterList.c[count].isProjectileWeapon,
                        masterList.c[count].projectileObject,
                        masterList.c[count].range,
                        masterList.c[count].speed,
                        masterList.c[count].targetLimit);
                    break;
            }
            database.Add(newItem);
        }
    }
}

[System.Serializable]
public class Item
{
    //Required
    public int ID;
    public string Title;
    public Sprite Image;
    public Defs.ItemCategory Category;
    public int StackLimit;
    public float Weight;

    //Weapon Specific
    public Defs.WeaponType WeaponType;
    public GameObject WeaponObject;
    //General
    public AudioClip[] WindupSound;
    public AudioClip[] StrikeSound;
    public AudioClip[] HitSound;
    public Effector CostEffector;
    public Effector SelfEffector;
    public Effector SelfOnHitEffector;
    public Effector TargetEffector;
    public float WindupTime;
    public float StrikeTime;
    public float RecoveryTime;
    //Projectile
    public GameObject ProjectileObject;
    public AudioClip[] FlightSound;
    public float Range;
    public float Speed;
    public int TargetLimit;
    //Shield
    public AudioClip[] BlockSound;
    public int BlockAngle;

    //Consumeable Specific
    public float UsageDelay;
    public bool IsProjectileWeapon;

    //Weapon - Melee
    public Item(int id,
        string title,
        Sprite image,
        Defs.ItemCategory category,
        int stackLimit,
        float weight,
        Defs.WeaponType weaponType,
        GameObject weaponObject,
        AudioClip[] windupSound,
        AudioClip[] strikeSound,
        AudioClip[] hitSound,
        Effector costEffector,
        Effector selfEffector,
        Effector selfOnHitEffector,
        Effector targetEffector,
        float windupTime,
        float strikeTime,
        float recoveryTime)
    {
        //Required
        this.ID = id;
        this.Title = title;
        this.Image = image;
        this.Category = category;
        this.StackLimit = stackLimit;
        this.Weight = weight;

        //Effectors
        this.CostEffector = costEffector;
        this.SelfEffector = selfEffector;
        this.SelfOnHitEffector = selfOnHitEffector;
        this.TargetEffector = targetEffector;

        //Weapon Specific
        this.WeaponType = weaponType;
        this.WeaponObject = weaponObject;
        //General
        this.WindupSound = windupSound;
        this.StrikeSound = strikeSound;
        this.HitSound = hitSound;
        this.WindupTime = windupTime;
        this.StrikeTime = strikeTime;
        this.RecoveryTime = recoveryTime;
    }

    //Weapon - Projectile
    public Item(int id,
        string title,
        Sprite image,
        Defs.ItemCategory category,
        int stackLimit,
        float weight,
        Defs.WeaponType weaponType,
        GameObject weaponObject,
        AudioClip[] windupSound,
        AudioClip[] releaseSound,
        AudioClip[] hitSound,
        AudioClip[] flightSound,
        Effector costEffector,
        Effector selfEffector,
        Effector selfOnHitEffector,
        Effector targetEffector,
        float windupTime,
        float strikeTime,
        float recoveryTime,
        GameObject projectileObject,
        float range,
        float speed,
        int targetLimit)
    {
        //Required
        this.ID = id;
        this.Title = title;
        this.Image = image;
        this.Category = category;
        this.StackLimit = stackLimit;
        this.Weight = weight;

        //Effectors
        this.CostEffector = costEffector;
        this.SelfEffector = selfEffector;
        this.SelfOnHitEffector = selfOnHitEffector;
        this.TargetEffector = targetEffector;

        //Weapon Specific
        this.WeaponType = weaponType;
        this.WeaponObject = weaponObject;
        //General
        this.WindupSound = windupSound;
        this.StrikeSound = releaseSound;
        this.HitSound = hitSound;
        this.WindupTime = windupTime;
        this.StrikeTime = strikeTime;
        this.RecoveryTime = recoveryTime;
        //Projectile
        this.ProjectileObject = projectileObject;
        this.FlightSound = flightSound;
        this.Range = range;
        this.Speed = speed;
        this.TargetLimit = targetLimit;
    }

    //Weapon - Shield
    public Item(int id,
        string title,
        Sprite image,
        Defs.ItemCategory category,
        int stackLimit,
        float weight,
        Defs.WeaponType weaponType,
        GameObject weaponObject,
        AudioClip[] windupSound,
        AudioClip[] strikeSound,
        Effector costEffector,
        Effector selfEffector,
        Effector selfOnHitEffector,
        Effector targetEffector,
        float windupTime,
        float strikeTime,
        float recoveryTime,
        AudioClip[] blockSound,
        int blockAngle)
    {
        //Required
        this.ID = id;
        this.Title = title;
        this.Image = image;
        this.Category = category;
        this.StackLimit = stackLimit;
        this.Weight = weight;

        //Effectors
        this.CostEffector = costEffector;
        this.SelfEffector = selfEffector;
        this.SelfOnHitEffector = selfOnHitEffector;
        this.TargetEffector = targetEffector;

        //Weapon Specific
        this.WeaponType = weaponType;
        this.WeaponObject = weaponObject;
        //General
        this.WindupSound = windupSound;
        this.StrikeSound = strikeSound;
        this.WindupTime = windupTime;
        this.StrikeTime = strikeTime;
        this.RecoveryTime = recoveryTime;
        //Shield
        this.BlockSound = blockSound;
        this.BlockAngle = blockAngle;

    }

    //Consumeable
    public Item(int id,
        string title,
        Sprite image,
        Defs.ItemCategory category,
        int stackLimit,
        float weight,
        Effector costEffector,
        Effector selfEffector,
        Effector selfOnHitEffector,
        Effector targetEffector,
        float usageDelay,
        bool isProjectileWeapon,
        GameObject projectileObject,
        float range,
        float speed,
        int targetLimit)
    {
        //Required
        this.ID = id;
        this.Title = title;
        this.Image = image;
        this.Category = category;
        this.StackLimit = stackLimit;
        this.Weight = weight;

        //Effectors
        this.CostEffector = costEffector;
        this.SelfEffector = selfEffector;
        this.SelfOnHitEffector = selfOnHitEffector;
        this.TargetEffector = targetEffector;

        //Consumeable Specific
        this.UsageDelay = usageDelay;
        this.IsProjectileWeapon = isProjectileWeapon;

        //Projectile Specific
        this.ProjectileObject = projectileObject;
        this.Range = range;
        this.Speed = speed;
        this.TargetLimit = targetLimit;
    }
}