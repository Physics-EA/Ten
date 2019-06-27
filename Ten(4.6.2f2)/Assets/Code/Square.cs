using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GDGeek;

public class Square : MonoBehaviour {


	public Texture[] _texutres = null;
	public MeshRenderer _mesh = null;
	public Text _number = null;
	public Rigidbody _body = null;
	private Vector3 localPosition;
	private Vector3 localScale;
	public void hide ()
	{

		this.gameObject.SetActive (false);
		//_body.isKinematic = true;
		//_body.useGravity = false;
		//this.transform.localScale = Vector3.one;
	}

	public void Awake(){
		//this.transform
	}
	public void show ()
	{

		this.gameObject.SetActive (true);

	}
	public Task removeTask(){
		TweenTask task = new TweenTask (delegate {
			TweenScale tween = TweenScale.Begin(this.gameObject, 0.2f, Vector3.zero);
			return tween;
				});
		TaskManager.PushBack (task, delegate {
			this.hide ();
			this.gameObject.transform.localScale = Vector3.one;
				});
		return task;
	}

	public Task overTask ()
	{
		TaskWait tw = new TaskWait ();
	/*	tw.setAllTime (0.1f);
		//_body.AddForce
		TaskManager.PushFront (tw, delegate {
			_body.isKinematic = false;
			_body.useGravity = true;
				});
		TaskManager.AddUpdate (tw, delegate(float d) {
			_body.AddForce(new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f),Random.Range(-10, -50f)));
				});
		TaskManager.PushBack (tw, delegate {
			//_body.isKinematic = false;
			//_body.useGravity = true;

				});*/
		return tw;
	}
	public int number {
		set{
			_mesh.material.mainTexture = _texutres[value -1];
			_number.text = value.ToString();
		}
	}


}
