using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Effector
{
    //Health
    public float health;
    public float healthPCT;

    //Stamina
    public float stamina;
    public float staminaPCT;

    //Mana
    public float mana;
    public float manaPCT;

    //Rest
    public float bloodlust;
    public float sunlight;
    public float moonlight;
    public float curse;
    public float corruption;
    public float darkness;

    [Header("States")]
    public List<State> states = new List<State>();
}

public static class EffectorMethods
{
    public static bool CheckIfEnoughResources(UnitCore unit, Effector effector)
    {
        //Health
        if (effector.health != 0)
            if (Mathf.Abs(effector.health) > unit.Health.Val)
                return false;

        if (effector.healthPCT != 0)
            if (Mathf.Abs(effector.healthPCT) > unit.Health.PCT())
                return false;

        //Stamina
        if (effector.stamina != 0)
            if (Mathf.Abs(effector.stamina) > unit.Stamina.Val)
                return false;

        if (effector.staminaPCT != 0)
            if (Mathf.Abs(effector.staminaPCT) > unit.Stamina.PCT())
                return false;

        //Mana
        if (effector.mana != 0)
            if (Mathf.Abs(effector.mana) > unit.Mana.Val)
                return false;

        if (effector.manaPCT != 0)
            if (Mathf.Abs(effector.manaPCT) > unit.Mana.PCT())
                return false;

        //Rest
        if (effector.bloodlust != 0)
            if (Mathf.Abs(effector.bloodlust) > unit.Bloodlust.Val)
                return false;
        if (effector.sunlight != 0)
            if (Mathf.Abs(effector.sunlight) > unit.Sunlight.Val)
                return false;
        if (effector.moonlight != 0)
            if (Mathf.Abs(effector.moonlight) > unit.Moonlight.Val)
                return false;
        if (effector.curse != 0)
            if (Mathf.Abs(effector.curse) > unit.Curse.Val)
                return false;
        if (effector.corruption != 0)
            if (Mathf.Abs(effector.corruption) > unit.Corruption.Val)
                return false;
        if (effector.darkness != 0)
            if (Mathf.Abs(effector.darkness) > unit.Darkness.Val)
                return false;
        return true;
    }
}