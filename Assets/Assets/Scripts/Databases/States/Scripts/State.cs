using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New State", menuName = "Data/State", order = 2)]
[System.Serializable]
public class State : ScriptableObject
{
    public Sprite image;
    public bool debuff;
    public int duration;

    public bool increaseDuration;
    public bool independent;

    public int maxStacks;

    [System.Serializable]
    public class Effect
    {
        [Header("Activation")]
        public bool awakeEffect;
        public bool destroyEffect;
        public int tickLength;

        [Header("Effects")]
        public Effector effector;
    }
    public List<Effect> effects;
}