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

		var _shooter = Game.Player().shooter;

		if (_shooter) {
			followAgent.target = _shooter.gameObject;
		}

		Game.Player().postPlayerChanged += ListenPlayerChanged;
	}

	public override void OnDestroy() {
		Game.Player().postPlayerChanged -= ListenPlayerChanged;
	}

	void ListenPlayerChanged(Shooter _shooter) {

		if (_shooter == null) {
			followAgent.enabled = false;
			followAgent.target = null;
		} else {
			followAgent.enabled = true;
			followAgent.target = _shooter.gameObject;
		}

	}
}

