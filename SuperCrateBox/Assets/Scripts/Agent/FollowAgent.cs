using UnityEngine;
using System.Collections;

public class FollowAgent : Agent
{
	public GameObject target;

	public override void Start () {
		base.Start();
		direction = new Vector2(1, 0);
	}

	public override void Update ()
	{
		base.Update();
		direction = (target.transform.position - transform.position).normalized;
		rigidbody2D.AddForce(speed * direction);
	}
}
