using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Sheet", menuName = "Data/Player/Ability Sheet", order = 1)]
public class Abilities : ScriptableObject
{
    public List<Ability> contained = new List<Ability>();
}

[System.Serializable]
public class Ability
{
    [Header("General")]
    public string title;
    public Sprite image;
    public Defs.SlotType slotType;
    public Effector cost;
    [Multiline(4)]
    public string description;

    [Header("Instructions")]
    public List<Action> actions = new List<Action>();
    [System.Serializable]
    public class Action
    {
        public enum Target
        {
            Self
        }
        public Target target;

        public enum Type
        {
            ApplyState, RemoveState, ApplyEffector , Animation
        }
        public Type type;

        [Header("Apply State & Remove State")]
        public List<State> targetStates;

        [Header("Effector")]
        public Effector targetEffector;

        [Header("Animation")]
        public string animationTrigger;

        public enum MovementType
        {
            None, MoveDirection, MouseDirection, CustomDirection
        }

        [Header("Movement")]
        public MovementType movementType;
        public Quaternion direction;
        public float distance;
        public float time;
    }

    [Header("Mechanics")]
    public float length = 0;
}