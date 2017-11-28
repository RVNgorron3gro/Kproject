using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defs
{
    public enum PostProcessingProfile
    {
        Normal, Death
    }

    public enum MessageType
    {
        Notify, Warn
    }

    public enum ResourceTypes
    {
        Health, Stamina, Mana, Bloodlust, Sunlight, Moonlight, Curse, Corruption, Darkness
    }

    public enum AbilityMode
    {
        Use, Cooldown, Ready, Level, Chosen, NotChosen
    }

    public enum LayoutSize
    {
        Large, Medium, Small
    }

    public enum PlayerClass
    {
        Warrior, Commander, Scout, Colonel, Monk, Herald, Knight, Assassin, Sentinel, Strategist, Patrol, Hunter
    }

    public enum SlotType
    {
        Red, Blue, Green
    }

    public enum XpType
    {
        Neutral, Enemy
    }

    public enum UnitType
    {
        Critter, Troop, Player
    }

    public enum ItemCategory
    {
        Weapon, Ammunition, Consumeable
    }

    public enum WeaponType
    {
        Melee, Projectile, Shield
    }

    public enum ItemSound
    {
        WindupSound, StrikeSound, HitSound, FlightSound, BlockSound
    }

    public enum Regions
    {
        Beach, FishingVillage, Swamp, Badlands, Ruins, Player1Start, Player2Start, Farmlands, Forest, Lake, FrozenVillage, Mountain
    }
}