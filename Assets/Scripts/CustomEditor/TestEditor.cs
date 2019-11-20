using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestScript))]
public class TestEditor : Editor
{
	List<Vector3> points = new List<Vector3>()
	{
		new Vector3(0, 0 ,0),
		new Vector3(4, 0, 0),
		new Vector3(4, 0, 4),
		new Vector3(0, 0, 4),
	};

	RoomState state = RoomState.Move;
	bool canRemove = true;

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Add point"))
			state = RoomState.Add;

		if (GUILayout.Button("Remove point"))
			state = RoomState.Remove;
	}

	private void OnSceneGUI()
	{
		DrawNodeSpheres(state == RoomState.Move);

		var closest = GetClosestLinePoint();
		if (state == RoomState.Add)
			Handles.CubeHandleCap(0, closest.p, Quaternion.identity, 0.1f, EventType.Repaint);

		int controlID = GUIUtility.GetControlID(FocusType.Passive | FocusType.Keyboard);
		EventType eventType = Event.current.GetTypeForControl(controlID);
		//Debug.Log(eventType);

		if (eventType == EventType.MouseDown)
		{
			switch (state)
			{
				case RoomState.Add:
					points.Insert(closest.point, closest.p);
					state = RoomState.Move;
					break;
				case RoomState.Remove:
					if (canRemove)
						points.RemoveAt(GetClosestPoint());

					if (!Event.current.shift)
						state = RoomState.Move;

					canRemove = false;
					break;
				default:
					break;
			}
		}
		else if (eventType == EventType.MouseUp || eventType == EventType.Used)
			canRemove = true;
		else if (eventType == EventType.KeyDown)
		{
			if (Event.current.keyCode == KeyCode.Escape)
				state = RoomState.Move;
		}
	}

	void DrawNodeSpheres(bool drawHandle)
	{
		int closestIndex = GetClosestPoint();
		for (int i = 0; i < points.Count; i++)
		{
			points[i] = new Vector3(points[i].x, 0, points[i].z);
			if (i == closestIndex && drawHandle)
				points[i] = Handles.PositionHandle(points[i], Quaternion.identity);
			else
			{
				if (i == closestIndex && state == RoomState.Remove)
					Handles.color = Color.red;

				Handles.SphereHandleCap(100 + i, points[i], Quaternion.identity, 0.5f, EventType.Repaint);
				Handles.color = Color.white;
			}

			if (i - 1 >= 0)
				Handles.DrawLine(points[i - 1], points[i]);
		}
		Handles.DrawLine(points[0], points[points.Count - 1]);
	}

	int GetClosestPoint()
	{
		float dist = float.MaxValue;
		int point = -1;

		for (int i = 0; i < points.Count; i++)
		{
			float newDist = Vector2.Distance(Event.current.mousePosition, HandleUtility.WorldToGUIPoint(points[i]));

			if (newDist < dist)
			{
				dist = newDist;
				point = i;
			}
		}

		return point;
	}

	(Vector3 p, int point) GetClosestLinePoint()
	{
		Vector3 point = default;
		float pointDist = 0;
		int index = -1;
		for (int i = 1; i < points.Count; i++)
		{
			float newDist = HandleUtility.DistanceToPolyLine(points[i - 1], points[i]);
			if (point == default || newDist < pointDist)
			{
				point = HandleUtility.ClosestPointToPolyLine(points[i - 1], points[i]);
				pointDist = newDist;
				index = i;
			}
		}

		float lastNewDist = HandleUtility.DistanceToPolyLine(points[0], points[points.Count - 1]);
		if (point == default || lastNewDist < pointDist)
		{
			point = HandleUtility.ClosestPointToPolyLine(points[0], points[points.Count - 1]);
			pointDist = lastNewDist;
			index = points.Count - 1;
		}

		return (point, index);
	}
}

enum RoomState
{
	Move,
	Add,
	Remove
}
