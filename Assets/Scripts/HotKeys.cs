using UnityEngine;
using System.Collections;

public class HotKeys : MonoBehaviour {
	
	public KeyCode nextScene=KeyCode.N;
	public KeyCode preScene=KeyCode.P;
	public KeyCode quit=KeyCode.Q;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(nextScene)){
			if(Application.loadedLevel+1<Application.levelCount){
				StartCoroutine(loadScene(Application.loadedLevel+1));
			}
		}else if(Input.GetKeyDown(preScene)){
			if(Application.loadedLevel-1>=0){
				StartCoroutine(loadScene(Application.loadedLevel-1));
			}
		}else if(Input.GetKeyDown(quit)){
			Application.Quit();
		}
	}
	
	IEnumerator loadScene(int scene){
		AsyncOperation async = Application.LoadLevelAsync(scene);
		yield return async;
	}
}
