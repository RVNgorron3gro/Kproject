using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class DetermineSeason : MonoBehaviour {
	
	public int leTurn;
	
	public int seasonSerial;
	public string seasonName;
	
	public int gameDay;
	
	public int theDay;
	public int theMonth;
	
	public PostProcessingProfile post;
	
	// Use this for initialization
	void Start () {
		
		//leTurn should be the current turn
		leTurn = 1;
		gameDay = 1;
		//in the future these will be retrieved from a server
		theDay = System.DateTime.Now.Day;
		theMonth = System.DateTime.Now.Month;
		
		/* space for test dates
			theDay = 7;
			theMonth = 6;
		*/
		
		//Winter
		if((theDay >= 21 && theMonth == 12) || (theMonth == 1) || (theMonth == 2) || (theDay < 21 && theMonth == 3)){
			seasonSerial = 0;
		} else
		//Spring
		if((theDay >= 21 && theMonth == 3) || (theMonth == 4) || (theMonth == 5) || (theDay < 21 && theMonth == 6)){
			seasonSerial = 1;
		} else
		//Summer
		if((theDay >= 21 && theMonth == 6) || (theMonth == 7) || (theMonth == 8) || (theDay < 21 && theMonth == 9)){
			seasonSerial = 2;
		} else
		//Autumn
		if((theDay >= 21 && theMonth == 9) || (theMonth == 10) || (theMonth == 11) || (theDay < 21 && theMonth == 12)){
			seasonSerial = 3;
		}
		translateSeason(seasonSerial);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.C)){
			if (leTurn % 2 != 0){
				leTurn = 0;
			}
			leTurn += 12;
			updateSeason(leTurn);
		}
		
		if (Input.GetKeyUp(KeyCode.E)){
			var colors = post.colorGrading.settings;
			colors.basic.temperature = 30;
			post.colorGrading.settings = colors;
		}
	}
	
	void updateSeason (int turn){
		if (isSeasonChanging(turn)){
			Debug.Log("Season will change");
			seasonSerial += 1;
			if (seasonSerial > 3){
				seasonSerial = 0;
			}
			translateSeason(seasonSerial);
		}
		if (isDayChanging(turn)){
			if (isSeasonChanging(turn)){
				gameDay = 0;
			}
			gameDay += 1;
			Debug.Log("A new day starts");
		}
	}
	
	void translateSeason (int seasonSerially){
		if (seasonSerially == 0){
			seasonName = "Winter";
		}
		if (seasonSerially == 1){
			seasonName = "Spring";
		}
		if (seasonSerially == 2){
			seasonName = "Summer";
		}
		if (seasonSerially == 3){
			seasonName = "Autumn";
		}
		seasonColoring(seasonName);
	}
	
	void seasonColoring(string season){
		var colors = post.colorGrading.settings;
		switch (season){
			case "Winter":
				colors.basic.postExposure = 2;
				colors.basic.temperature = -30;
				colors.basic.tint = 40;
				colors.basic.saturation = 1.1f;
				colors.basic.contrast = 1.2f;
				colors.channelMixer.red = new Vector3(1, 0, 0);
				colors.channelMixer.green = new Vector3(0, 1, 0);
				colors.channelMixer.blue = new Vector3(0, 0, 1);
			break;
			case "Spring":
				colors.basic.postExposure = 2;
				colors.basic.temperature = 20;
				colors.basic.tint = -10;
				colors.basic.saturation = 1.5f;
				colors.basic.contrast = 1.2f;
				colors.channelMixer.red = new Vector3(1, 0, 0);
				colors.channelMixer.green = new Vector3(0.1f, 1, 0.1f);
				colors.channelMixer.blue = new Vector3(0, 0, 1);
			break;
			case "Summer":
				colors.basic.postExposure = 2.12f;
				colors.basic.temperature = 0;
				colors.basic.tint = 15;
				colors.basic.saturation = 1.5f;
				colors.basic.contrast = 1.2f;
				colors.channelMixer.red = new Vector3(1, 0, 0);
				colors.channelMixer.green = new Vector3(0, 1, 0);
				colors.channelMixer.blue = new Vector3(0, 0, 1);
			break;
			case "Autumn":
				colors.basic.postExposure = 2;
				colors.basic.temperature = 0;
				colors.basic.tint = 20;
				colors.basic.saturation = 1.2f;
				colors.basic.contrast = 1.2f;
				colors.channelMixer.red = new Vector3(1, 0, 0);
				colors.channelMixer.green = new Vector3(0, 0.9f, 0);
				colors.channelMixer.blue = new Vector3(0.15f, 0, 1);
			break;
		}
		post.colorGrading.settings = colors;
	}
	
	bool isDayChanging(int turn){
		if (turn % 8 == 0){
			return true;
		} else {
			return false;
		}
	}
	
	bool isSeasonChanging(int turn){
		if (turn % 24 == 0){
			return true;
		} else {
			return false;
		}
	}
}