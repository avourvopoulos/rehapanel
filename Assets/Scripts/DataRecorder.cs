using UnityEngine;
using System.IO;
using System.Collections;
using Tobii.Eyetracking.Sdk;

public class DataRecorder{
	
	ArrayList raw_data;
	string format;
	public long max_count = int.MaxValue;
	
	public DataRecorder(){
		raw_data=new ArrayList();
		for(int i=0;i<16;i++){
			if(i>0) format += ",";
			format += "{"+i.ToString()+"}";
		}
	}
	
	public void feed(DataItem item){
		raw_data.Add(item);
	}
	
	public void WriteData(string path){
		if(raw_data.Count>0){
			using(StreamWriter sw = new StreamWriter(path)){
				foreach(DataItem i in raw_data){
					sw.WriteLine(string.Format(format,
						i.gazeItem.LeftGazePoint2D.X, i.gazeItem.LeftGazePoint2D.Y,
						i.gazeItem.RightGazePoint2D.X, i.gazeItem.RightGazePoint2D.Y,
						i.gazeItem.LeftEyePosition3D.X, i.gazeItem.LeftEyePosition3D.Y, i.gazeItem.LeftEyePosition3D.Z,
						i.gazeItem.RightEyePosition3D.X, i.gazeItem.RightEyePosition3D.Y, i.gazeItem.RightEyePosition3D.Z,
						i.gazeItem.LeftValidity, i.gazeItem.RightValidity, i.gazeItem.TimeStamp,
						i.objectX, i.objectY, i.isVisible
					));
				}
			}
			raw_data.Clear();
			Debug.Log("data recorded");
		}
	}

	public void ClearData(){
		raw_data.Clear();
	}
	
	public void WriteData(){
		WriteData(Application.dataPath+"\\Logs\\"+System.DateTime.Now.ToFileTime()+".txt");
	}
}
