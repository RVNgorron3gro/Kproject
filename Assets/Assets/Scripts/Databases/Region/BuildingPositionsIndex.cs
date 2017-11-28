using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building Positions Index", menuName = "Data/Regions/Building Positions Index", order = 3)]
[System.Serializable]
public class BuildingPositionsIndex : ScriptableObject
{
    public int toRestore;
    [System.Serializable]
    public struct Layout
    {
        public Vector3 castle;
        public Quaternion castleRot;
        public Vector3 houseA;
        public Quaternion houseARot;
        public Vector3 houseB;
        public Quaternion houseBRot;
        public Vector3 barracks;
        public Quaternion barracksRot;
        public Vector3 granary;
        public Quaternion granaryRot;
        public Vector3 workshop;
        public Quaternion workshopRot;
    }
    public List<Layout> layout = new List<Layout>();

    public void CreateNewEntry()
    {
        Transform castleT = GameObject.Find("ICastle").transform;
        Transform houseAT = GameObject.Find("IHouseA").transform;
        Transform houseBT = GameObject.Find("IHouseB").transform;
        Transform barracksT = GameObject.Find("IBarracks").transform;
        Transform granaryT = GameObject.Find("IGranary").transform;
        Transform workshopT = GameObject.Find("IWorkshop").transform;

        Layout newLayout = new Layout()
        {
            castle = castleT.transform.localPosition,
            castleRot = castleT.transform.localRotation,
            houseA = houseAT.localPosition,
            houseARot = houseAT.transform.localRotation,
            houseB = houseBT.localPosition,
            houseBRot = houseBT.transform.localRotation,
            barracks = barracksT.localPosition,
            barracksRot = barracksT.transform.localRotation,
            granary = granaryT.localPosition,
            granaryRot = granaryT.transform.localRotation,
            workshop = workshopT.localPosition,
            workshopRot = workshopT.transform.localRotation,
        };
        layout.Add(newLayout);
    }

    public void Restore()
    {
        GameObject castle = GameObject.Find("ICastle");
        castle.transform.localPosition = layout[toRestore].castle;
        castle.transform.localRotation = layout[toRestore].castleRot;
        GameObject houseA = GameObject.Find("IHouseA");
        houseA.transform.localPosition = layout[toRestore].houseA;
        houseA.transform.localRotation = layout[toRestore].houseARot;
        GameObject houseB = GameObject.Find("IHouseB");
        houseB.transform.localPosition = layout[toRestore].houseB;
        houseB.transform.localRotation = layout[toRestore].houseBRot;
        GameObject barracks = GameObject.Find("IBarracks");
        barracks.transform.localPosition = layout[toRestore].barracks;
        barracks.transform.localRotation = layout[toRestore].barracksRot;
        GameObject granary = GameObject.Find("IGranary");
        granary.transform.localPosition = layout[toRestore].granary;
        granary.transform.localRotation = layout[toRestore].granaryRot;
        GameObject workshop = GameObject.Find("IWorkshop");
        workshop.transform.localPosition = layout[toRestore].workshop;
        workshop.transform.localRotation = layout[toRestore].workshopRot;
    }
}