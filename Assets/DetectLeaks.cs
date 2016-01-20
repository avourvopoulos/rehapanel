using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class DetectLeaks : MonoBehaviour
{
	void OnGUI()
	{
		Object[] objects = FindObjectsOfType(typeof (UnityEngine.Object));
 
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
 
		foreach(Object obj in objects)
		{
			string key = obj.GetType().ToString();
			if(dictionary.ContainsKey(key))
			{
				dictionary[key]++;
			} 
			else
			{
				dictionary[key] = 1;
			}
		}
 
		List<KeyValuePair<string, int>> myList = new List<KeyValuePair<string, int>>(dictionary);
		myList.Sort(
			delegate(KeyValuePair<string, int> firstPair,
			KeyValuePair<string, int> nextPair)
				{
					return nextPair.Value.CompareTo((firstPair.Value));
				}
		);
 
		foreach (KeyValuePair<string, int> entry in myList)
		{
			GUILayout.Label(entry.Key + ": " + entry.Value);
		}
 
	}
}