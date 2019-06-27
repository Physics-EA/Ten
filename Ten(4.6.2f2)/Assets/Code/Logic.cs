using UnityEngine;
using System.Collections;
using GDGeek;
// http://gdgeek.com mit 
using System.Collections.Generic;

public class Logic : MonoBehaviour {
	private FSM fsm_ = new FSM();
	public Ctrl _ctrl = null;

	public void fsmPost(string msg){

		fsm_.post (msg);
	}
	State beginState ()
	{
		StateWithEventMap state = new StateWithEventMap ();
		state.onStart += delegate {
			_ctrl._model.clear();
			_ctrl.getNext();
			_ctrl._speed = 1.0f;
			Time.timeScale = _ctrl._speed;
			_ctrl._view.begin.gameObject.SetActive(true);
		};
		state.onOver += delegate {
			_ctrl._view.begin.gameObject.SetActive(false);
		};

		state.addEvent("begin", "create");
		return state;
	}

	State playState ()
	{

		
		StateWithEventMap state = new StateWithEventMap ();

		state.onStart += delegate {
			_ctrl._view.play.gameObject.SetActive(true);
			_ctrl.refreshModel2View();
		};
		state.onOver += delegate {
			_ctrl._view.play.gameObject.SetActive(false);
		};
		return state;
	}

	State endState ()
	{

		StateWithEventMap state = TaskState.Create(delegate {return this._ctrl.endTask();},fsm_, "");
		state.addEvent("end", "begin");
		state.onStart += delegate {
			_ctrl._view.end.gameObject.SetActive(true);
			_ctrl._sound.end.Play ();
		};
		state.onOver += delegate {
			_ctrl._view.end.gameObject.SetActive(false);
		};
		return state;
	}



	private State fallState ()
	{
		StateWithEventMap state = TaskState.Create(delegate {

			Task fall = _ctrl.doFallTask();
		
			return fall;
		}, fsm_, "remove");

		state.onStart += delegate {
			_ctrl._toX = -1;
				};
		state.onStart += delegate {
			Debug.LogWarning("in fall!");
		};

		return state;
	}



	State removeState ()
	{
		List<Cube> remove = null;
		StateWithEventMap state = TaskState.Create(delegate {

			
			remove = _ctrl.checkRemove();

			Time.timeScale = _ctrl._speed;
			return _ctrl.removeTask(remove);
		}, fsm_, 
		delegate {
			if(remove.Count != 0){
				return "fall";
			}else{
				return "create";
			}});

		return state;
	}


	public State inputState ()
	{	
		
		StateWithEventMap state = new StateWithEventMap ();

		state.addAction("0", delegate(FSMEvent evt) {
			if(_ctrl.checkToX(0)){
				Time.timeScale =3.0f * _ctrl._speed;
				_ctrl._toX = 0;
			}
		});
		
		
		state.addAction("1", delegate(FSMEvent evt) {
			if(_ctrl.checkToX(1)){
				Time.timeScale =3.0f * _ctrl._speed;
				_ctrl._toX = 1;
			}
		});
		
		
		
		state.addAction("2", delegate(FSMEvent evt) {
			if(_ctrl.checkToX(2)){
				Time.timeScale =3.0f * _ctrl._speed;
				_ctrl._toX = 2;
			}
		});
		
		
		
		state.addAction("3", delegate(FSMEvent evt) {
			if(_ctrl.checkToX(3)){
				Time.timeScale =3.0f * _ctrl._speed;
				_ctrl._toX = 3;
			}
		});
		return state;
	}

	public State createState()
	{
		StateWithEventMap state = TaskState.Create(delegate {
			return _ctrl.createTask();
		},fsm_,delegate {
			if(_ctrl.check()){
				return "move.y";
			}else{
				return "over";
			}
		}
		);
		
		return state;
	}


	public State checkState ()
	{
		StateWithEventMap state = TaskState.Create(delegate {
			return new Task();
			
		}, fsm_, delegate {
			if(_ctrl._toX != -1){
				if(_ctrl._toX == _ctrl._currX){
					return "fall";
				}else{
					
					return "move.x";
				}

			}
			if(_ctrl.check()){
				return "move.y";
			}else{
				_ctrl._sound.fall.Play();
				return "remove";
			}
		});
		
		return state;
	}

	
	public State moveYState(){
		StateWithEventMap state = TaskState.Create(delegate {
			return _ctrl.fallOneTask();
		},fsm_, "check");
		
		return state;
	}

	public State moveXState ()
	{
		StateWithEventMap state = TaskState.Create (delegate() {

			return _ctrl.moveXTask();
		}, fsm_, "fall");

		return state;
	}

	private State overState ()
	{
		StateWithEventMap state = TaskState.Create (delegate() {
			
			return _ctrl.overTask();
		}, fsm_, "end");
		
		return state;
	}

	// Use this for initialization
	void Start () {
		fsm_.addState ("begin", beginState());
		fsm_.addState ("play", playState ());
		
		fsm_.addState ("input", inputState(), "play");
		fsm_.addState ("create", createState(), "input");
		fsm_.addState ("check", checkState(), "input");
		fsm_.addState ("move.y", moveYState(), "input");
		fsm_.addState ("move.x", moveXState(), "input");
		fsm_.addState ("fall", fallState(), "play");
		fsm_.addState ("over", overState(), "play");
		fsm_.addState ("remove", removeState(), "play");
		fsm_.addState ("end", endState ());
		fsm_.init ("begin");
	}
	

}
