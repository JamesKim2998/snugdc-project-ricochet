using UnityEngine;
using System.Collections;

public class CrawlAgent : Agent {

	public TerrainDetector forwardDetector;

	public override void Start() {

		base.Start();

		direction = new Vector2(1, 0);

		forwardDetector.postDetect = (Collider2D) => {
			direction = -direction;
		};
	}

	public override void Update () {
		base.Update();
		rigidbody2D.AddForce(speed * direction);
	}
}
