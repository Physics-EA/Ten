using UnityEngine;
using System.Collections;

public class Model : MonoBehaviour {

	public int width = 4;
	public int height = 7;
	public int score = 0;
	public Cube[] list = null;
	public void Awake(){
		for(int x = 0; x<width; ++x){
			for(int y=0; y<height; ++y){
				Cube c = this.getCube(x, y);
				c.position = new Vector2(x, y);
			}
		}
	}

	public void clear ()
	{
		foreach (Cube c in this.list) {
			c.isEnabled = false;		
		}
		this.score = 0;
	}

	public Cube getCube (int x, int y)
	{
		if (x < 0 || x >= width) {
			return null;		
		}
		if (y < 0 || y >= height) {
			return null;		
		}
		int n = x + y * 4;
		return list[n];
	}
}
