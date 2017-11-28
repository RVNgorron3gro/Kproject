using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UnitHUDStyles", menuName = "Data/Utility/UnitHUDStyles", order = 1)]
public class UnitHUDStyles : ScriptableObject
{
    public enum Container
    {
        Large, Medium, Small
    }

    public enum Bar
    {
        Large, Medium, Small
    }

    [System.Serializable]
    public struct Gauge
    {
        public Bar bar;
        public Defs.ResourceTypes resource;
        public bool reductionAnimation;
        public bool guideLines;
    }

    [System.Serializable]
    public struct Style
    {
        [Header("Setup")]
        public Defs.UnitType target;
        public Container container;
        public List<Gauge> gauge;
    }
    public List<Style> type = new List<Style>();
}