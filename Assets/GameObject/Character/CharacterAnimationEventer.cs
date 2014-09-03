using UnityEngine;
using System;
using System.Collections;

public class CharacterAnimationEventer : MonoBehaviour
{
	public Action postThrowAway;

	public void PostThrowAway() { if (postThrowAway != null) postThrowAway(); }
}

