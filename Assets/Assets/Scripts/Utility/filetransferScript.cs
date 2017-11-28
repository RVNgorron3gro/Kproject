using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System;
using System.IO;
using System.Net;
using System.Text;
using Steamworks;

public class filetransferScript : MonoBehaviour {
	
	public string myacc = "devacc";
	public string mypass = "devpass";
	public string url;
	
	public string IP = "62.151.145.167";
	
	public string PlayerFoldersDir = "ftp://90.74.87.4/Playerfolders/";
	
	public string SteamIDString;
	public string SteamURL;
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyUp(KeyCode.C)){
			//to create the player folder which will have SteamURL as the folder's name
			WebRequest request = WebRequest.Create(SteamURL);
			request.Method = WebRequestMethods.Ftp.MakeDirectory;
			request.Credentials = new NetworkCredential(myacc, mypass);
			using (var resp = (FtpWebResponse) request.GetResponse())
			{
				Debug.Log(resp.StatusCode);
			}
		}
		
		if (Input.GetKeyUp(KeyCode.X)){
			Debug.Log(FileExists("ftp://" + myacc + ":" + mypass + "@90.74.87.4/testtext.txt"));
		}
	}
	
    IEnumerator Start()
    {
		if (SteamManager.Initialized){
			Steamworks.CSteamID playerFTPName = SteamUser.GetSteamID();
			SteamIDString = playerFTPName.ToString();
			SteamURL = PlayerFoldersDir + SteamIDString;
		}
		url = "ftp://" + myacc + ":" + mypass + "@90.74.87.4/testtext.txt";
		//to receive text from .txt file hosted in url variable
        WWW www = new WWW(url);
        yield return www;
		Debug.Log("the FTP is running");
		Debug.Log(www.url);
		Debug.Log(www.text);
		
		//check if the folder exists using DirExists method
		if (!DirExists(SteamURL + "/")){
			//folder creation if it doesn't exist
			WebRequest request = WebRequest.Create(SteamURL);
			request.Method = WebRequestMethods.Ftp.MakeDirectory;
			request.Credentials = new NetworkCredential(myacc, mypass);
			using (var resp = (FtpWebResponse) request.GetResponse())
			{
				Debug.Log(resp.StatusCode);
			}
			
			//create files inside the folder
			
		} else {
			Debug.Log("Folder for player " + SteamIDString + " already exists");
			//if the folder exists, check for it's files
			
			//Account level
			if (FileExists(SteamURL + "/AccLevel.txt")){
				//if it exists, retrieve it's value for the player
				url = "ftp://" + myacc + ":" + mypass + "@90.74.87.4/Playerfolders/" + SteamIDString + "/AccLevel.txt";
				//to receive text from .txt file hosted in url variable
				WWW www1 = new WWW(url);
				yield return www1;
				Debug.Log("AccLevel: " + www1.text);
			} else {
				//if it doesn't exist, create it on the server, then download it
				FtpWebRequest request = (FtpWebRequest)WebRequest.Create(SteamURL + "/AccLevel.txt");  
				request.Method = WebRequestMethods.Ftp.UploadFile;
				request.Credentials = new NetworkCredential (myacc, mypass);
				
				System.Text.ASCIIEncoding Encoding = new System.Text.ASCIIEncoding();
				Byte[] bytes = Encoding.GetBytes("1");
				request.ContentLength = bytes.Length;

				Stream requestStream = request.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
				FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			}
			
			//Account killcount
			if (FileExists(SteamURL + "/AccKills.txt")){
				//if it exists, retrieve it's value for the player
				url = "ftp://" + myacc + ":" + mypass + "@90.74.87.4/Playerfolders/" + SteamIDString + "/AccKills.txt";
				//to receive text from .txt file hosted in url variable
				WWW www2 = new WWW(url);
				yield return www2;
				Debug.Log("AccKills: " + www2.text);
			} else {
				//if it doesn't exist, create it on the server, then download it
				FtpWebRequest request = (FtpWebRequest)WebRequest.Create(SteamURL + "/AccKills.txt");  
				request.Method = WebRequestMethods.Ftp.UploadFile;
				request.Credentials = new NetworkCredential (myacc, mypass);
				
				System.Text.ASCIIEncoding Encoding = new System.Text.ASCIIEncoding();
				Byte[] bytes = Encoding.GetBytes("0");
				request.ContentLength = bytes.Length;

				Stream requestStream = request.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
				FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			}
			
			//Account wins
			if (FileExists(SteamURL + "/AccWins.txt")){
				//if it exists, retrieve it's value for the player
			} else {
				//if it doesn't exist, create it on the server, then download it
				FtpWebRequest request = (FtpWebRequest)WebRequest.Create(SteamURL + "/AccWins.txt");  
				request.Method = WebRequestMethods.Ftp.UploadFile;
				request.Credentials = new NetworkCredential (myacc, mypass);
				
				System.Text.ASCIIEncoding Encoding = new System.Text.ASCIIEncoding();
				Byte[] bytes = Encoding.GetBytes("0");
				request.ContentLength = bytes.Length;

				Stream requestStream = request.GetRequestStream();
				requestStream.Write(bytes, 0, bytes.Length);
				requestStream.Close();
				FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			}
			
		}
    }
	
	//returns if the FTP directory exists or not
	public bool DirExists(string TheDir){
		try{
			WebRequest request = WebRequest.Create(TheDir);
			request.Method = WebRequestMethods.Ftp.ListDirectory;
			request.Credentials = new NetworkCredential(myacc, mypass);

			FtpWebResponse response = (FtpWebResponse)request.GetResponse();  

			Stream responseStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(responseStream);
			return true;
		} catch {
			return false;
		}
	}
	
	//returns if the FTP file exists or not
	public bool FileExists(string TheFile){
		try{
			WebRequest filerq = WebRequest.Create(TheFile);
			filerq.Method = WebRequestMethods.Ftp.GetFileSize;
			filerq.Credentials = new NetworkCredential(myacc, mypass);
			
			FtpWebResponse responsefile = (FtpWebResponse)filerq.GetResponse();

			Stream responseStream = responsefile.GetResponseStream();
			StreamReader readerfile = new StreamReader(responseStream);
			return true;
		} catch {
			return false;
		}
	}
}