using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lvllimit : MonoBehaviour {
	
	public int maxXP;
	public int maxXPDelta;
	public int levels;
	public float levelRaw;
	public float difference;
	public float singleDiff;
	public float firstLevel;
	public float totalLevel;
	List<float> levelLimit = new List<float>();
	List<float> totalLevelLimit = new List<float>();
	
	public int ranges;
	public int monstersPerRange;
	List<float> limitExpRange = new List<float>();
	
	
	// Use this for initialization
	void Start () {
		//default values for this game
		maxXP = 10000;
		levels = 50;
		ranges = 5;
		monstersPerRange = 5;
		LevelCalculation(maxXP, levels);
	}
	
	// Update is called once per frame
	void Update () {
		if (CheckChangeInt(maxXP, maxXPDelta))
		{
			LevelCalculation(maxXP, levels);
			maxXPDelta = GiveDeltaInt(maxXP, maxXPDelta);
		}
	}
	
	public void LevelCalculation (int maxXP, int levels)
	{
		levelRaw = 0;
		difference = 0;
		singleDiff = 0;
		firstLevel = 0;
		totalLevel = 0;
		levelLimit.Clear();
		totalLevelLimit.Clear();
		//for levels 0 and 1
		levelLimit.Add(0);
		totalLevelLimit.Add(0);
		levelLimit.Add(0);
		totalLevelLimit.Add(0);
		levelRaw = maxXP / levels;
		singleDiff = levelRaw / levels;
		firstLevel = levelRaw - singleDiff * levels;
		singleDiff = singleDiff * 2;
		for (int i = 2; i <= levels; i++)
		{
			if (i == levels)
			{
				firstLevel += firstLevel / 2 + singleDiff;
			}
			firstLevel += singleDiff;
			totalLevel += firstLevel;
			levelLimit.Add(firstLevel);
			totalLevelLimit.Add(totalLevel);
			Debug.Log("Level: " + i + " XP: " + firstLevel + " TotalXP: " + totalLevel);
		}
		
		//"level 0" in the list
		limitExpRange.Add(0);
		for(int i2 = 1; i2 <= ranges; i2++)
		{
			limitExpRange.Add(totalLevelLimit[(levels / ranges) * (i2)] - totalLevelLimit[(levels / ranges) * (i2 - 1)]);
		}
		
		for(int i3 = 1; i3 <= ranges; i3++)
		{
			Debug.Log("Exp for range " + i3 + ": " + limitExpRange[i3]);
		}
		
		for (int i4 = 1; i4 <= ranges; i4++)
		{
			Debug.Log("Exp difference between " + (i4 - 1) * (levels / ranges) + " and " + i4 * (levels / ranges) + ": " + (limitExpRange[i4] - limitExpRange[i4 - 1]));
		}
		
		for (int i5 = 1; i5 <= ranges; i5++)
		{
			Debug.Log("To level up from " + (i5 - 1) * (levels / ranges) + " to " + i5 * (levels / ranges) + " in " + monstersPerRange + " monsters each tier " + i5 + " monster should give " + limitExpRange[i5] / monstersPerRange + " XP");
		}
	}
	
	public bool CheckChangeInt (int varNormal, int varDelta)
	{
		if (varNormal != varDelta)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public int GiveDeltaInt (int varNormal, int varDelta)
	{
		if (varNormal != varDelta)
		{
			return varNormal;
		}
		else
		{
			return varDelta;
		}
	}
	
	//this script 
	
}