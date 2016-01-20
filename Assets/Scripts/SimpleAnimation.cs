using UnityEngine;
using System;
using System.Collections;

public class SimpleAnimationFrame{
	public static Vector3 EmptyVector = new Vector3(float.NaN,float.NaN,float.NaN);
	public static Quaternion EmptyQuaternion = new Quaternion(float.NaN,float.NaN,float.NaN,float.NaN);

	public Vector3 position = new Vector3(0.0f,0.0f,0.0f);
	public Vector3 scale = new Vector3(1.0f,1.0f,1.0f);
	public Quaternion rotate = new Quaternion(0.0f,0.0f,1.0f,-0.1f);
	public float duration = 5.0f;
	
	public SimpleAnimationFrame(){}
	
	public SimpleAnimationFrame(Vector3 _position,Vector3 _scale,Quaternion _rotate,float _duration){
		this.position = _position;
		this.scale = _scale;
		this.rotate = _rotate;
		this.duration = _duration;
	}
}

public class SimpleAnimation : MonoBehaviour {

	public SimpleAnimationFrame currentFrame;
	public event EventHandler finished;
	private bool isPlaying = false;
	
	public bool IsPlaying{
		get{return isPlaying;}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	bool usable(Quaternion rotate){
		return !(float.IsNaN(rotate.x) || float.IsNaN(rotate.y) || float.IsNaN(rotate.z) || float.IsNaN(rotate.w));
	}
	
	bool usable(Vector3 point){
		return !(float.IsNaN(point.x) || float.IsNaN(point.y) || float.IsNaN(point.z));
	}
	
	private IEnumerator play(object obj){
		if(isPlaying)yield break;
		isPlaying = true;
		SimpleAnimationFrame frame;
		if(obj is SimpleAnimationFrame){
			frame = obj as SimpleAnimationFrame;
		}else{
			frame = (obj as Queue).Dequeue() as SimpleAnimationFrame;
		}
		Vector3 start_position = transform.localPosition;
		Vector3 start_scale = transform.localScale;
		Vector3 end_postiion = usable(frame.position)?frame.position:start_position;
		Vector3 end_scale = usable(frame.scale)?frame.scale:start_scale;
		float start_time = Time.time;
		float end_time = start_time + frame.duration;
		while(Time.time<end_time){
			float delta = (Time.time-start_time)/frame.duration;
			transform.localPosition = Vector3.Lerp(start_position,end_postiion,delta);
			transform.localScale = Vector3.Lerp(start_scale,end_scale,delta);
			if(usable (frame.rotate)){
				transform.RotateAroundLocal(frame.rotate.eulerAngles,frame.rotate.w);
			}
			yield return new WaitForSeconds(1.0f/60.0f);
		}
		isPlaying =false;
		if(finished!=null){
			finished(this,EventArgs.Empty);
		}
		if(obj is Queue){
			if((obj as Queue).Count>0){
				StartCoroutine(play(obj));
			}
		}
	}
	
	public void Play(){
		if(isPlaying)return;
		StartCoroutine(play(currentFrame));
	}
	
	public void PlayWithFrame(SimpleAnimationFrame frame){
		if(isPlaying) return;
		if(frame==null)return;
		currentFrame = frame;
		StartCoroutine(play(currentFrame));
	}
	
	public void PlayWithQueue(Queue frames){
		StartCoroutine(play(frames));
	}
}
