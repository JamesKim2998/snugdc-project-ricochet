using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CallOnKeyboardEvent), true)]
public class CallOnKeyboardEventEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
		var _target = target as CallOnKeyboardEvent;
		NGUIEditorTools.DrawEvents("On Event", _target, _target.onEvent);
	}
}
