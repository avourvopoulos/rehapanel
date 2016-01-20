using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

public class OnlineGames : MonoBehaviour {

	public GUIStyle maingui;

	public Texture2D backIcon;
	
	List<Texture2D> textures = new List<Texture2D>();
	bool texturesLoaded = false;
	
	XmlDocument xmlDoc = new XmlDocument();
	
	WWW www;
	
	public string url = "http://neurorehabilitation.m-iti.org/tools/downloads/rehabnetcp/onlineapps/games.xml";
	
     string id = string.Empty;
     string gameTitle = string.Empty;
	 string gameURL = string.Empty;		
     string gameImage = string.Empty;
	 string gameDescription = string.Empty;
	
	public Vector2 scrollPosition = Vector2.zero;

  IEnumerator Start()
  {
    //Load XML data from a URL
//    public string url = "https://dl.dropboxusercontent.com/u/50963138/games.xml";
    WWW www = new WWW(url);
 
    //Load the data and yield (wait) till it's ready before we continue executing the rest of this method.
    yield return www;
    if (www.error == null)
    {
      //Sucessfully loaded the XML
      Debug.Log("Loaded following XML " + www.data);
 
      //Create a new XML document out of the loaded data
      xmlDoc.LoadXml(www.data);
 
      //Point to the game nodes and process them
//      ProcessGames(xmlDoc.SelectNodes("games/game"));
			
		//populate list with textures	
	  foreach (XmlNode node in xmlDoc.SelectNodes("games/game"))
	    {
	      	gameImage = node.SelectSingleNode("img").InnerText;	
			www = new WWW(gameImage);
	        yield return www;
			textures.Add(www.texture);
	    }		
		texturesLoaded = true;	
    }
    else
    {
      Debug.Log("ERROR: " + www.error);
    }
 
  }
 
 //Converts an XmlNodeList into Book objects and shows a book out of it on the screen
//  private void ProcessGames(XmlNodeList nodes)
//  {
// 
//    foreach (XmlNode node in nodes)
//    {
//      id = node.Attributes.GetNamedItem("id").Value;
//      gameTitle = node.SelectSingleNode("title").InnerText;
//	  gameURL = node.SelectSingleNode("url").InnerText;		
//      gameImage = node.SelectSingleNode("img").InnerText;
// 			
//		Debug.Log(id+" "+gameTitle+" "+gameURL+" "+gameImage);		
//    }
//  }
	
		
	void OnGUI()
	{
		if(MainGuiControls.GamesMenu && texturesLoaded)
		{
		//	GUI.Box(new Rect(Screen.width/2 - 260, 90, 480, 470), " ");
//			GUI.Box(new Rect(20, 90, 984, 610), " ", maingui);
			GUI.color = Color.yellow;
			GUI.Label(new Rect (Screen.width/2-80 , 100, 180, 20), "Available Online Apps");
			GUI.color = Color.white;
			//list all games dynamically
			float yOffset = 0.0f;
		scrollPosition = GUI.BeginScrollView(new Rect(Screen.width/2-250, 120, 740, 550), scrollPosition, new Rect(0, 0, 720, 600+(UDPReceive.emutracklst.Count*20)));	
			foreach (XmlNode node in xmlDoc.SelectNodes("games/game"))
	    	{
				id = node.Attributes.GetNamedItem("id").Value;
			    gameTitle = node.SelectSingleNode("title").InnerText;
				gameURL = node.SelectSingleNode("url").InnerText;		
				gameDescription = node.SelectSingleNode("description").InnerText;
					
				if(texturesLoaded)
				{
					GUI.Label(new Rect (20 , 20+yOffset, 100, 30+(gameTitle.Length*10)), id+": "+gameTitle);//game title
					
					if (GUI.Button (new Rect (20 , 50+yOffset , 60, 60), textures[int.Parse(id)-1]))//game image
					{
						 Application.OpenURL(gameURL);
					}
					
					GUI.Label(new Rect (150 , 60+yOffset, 200, 30+(gameDescription.Length*10)), gameDescription);//game description
				}
//				else
//				{
//					GUI.Label(new Rect (210 , 160, 100, 30+(gameDescription.Length*10)), "Loading...");
//				}	
			  yOffset += 120;	
			}
   		GUI.EndScrollView();
			
		}//if games menu
		
		else if(MainGuiControls.GamesMenu && !texturesLoaded)
		{
		//	GUI.Box(new Rect(20, 90, 984, 610), " ");
			GUI.Label(new Rect (Screen.width/2-40 , 300, 100, 30+(gameDescription.Length*10)), "Loading...");
		}

	}//ongui
	
	
}
