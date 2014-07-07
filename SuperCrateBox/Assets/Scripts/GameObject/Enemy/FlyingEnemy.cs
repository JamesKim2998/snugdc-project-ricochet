using UnityEngine;
using System.Collections;

public class FlyingEnemy : Enemy
{
	private FollowAgent followAgent { 
		get { return agent as FollowAgent; }
	}

	public override void Start ()
	{
		base.Start();

		var _character = Game.Character().character;

		if (_character) {
			followAgent.target = _character.gameObject;
		}

		Game.Character().postCharacterChanged += ListenCharacterChanged;
	}

	public override void OnDestroy() {
		Game.Character().postCharacterChanged -= ListenCharacterChanged;
	}

	void ListenCharacterChanged(Character _character) {

		if (_character == null) {
			followAgent.enabled = false;
			followAgent.target = null;
		} else {
			followAgent.enabled = true;
			followAgent.target = _character.gameObject;
		}

	}
}

