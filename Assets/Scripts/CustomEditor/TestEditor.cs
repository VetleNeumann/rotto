using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

[CustomEditor(typeof(TestScript))]
public class TestEditor : Editor
{
	List<Vector3> points = new List<Vector3>()
	{
		new Vector3(0, 0 ,0),
		new Vector3(4, 0, 0),
		//new Vector3(8, 0, 0),
		//new Vector3(8, 0, 4),
		new Vector3(4, 0, 4),
		new Vector3(0, 0, 4),
	};

	List<EditorRoom> rooms = new List<EditorRoom>();
	List<EditorDoor> doors = new List<EditorDoor>();
	List<int> selectedNodes = new List<int>();

	RoomState state = RoomState.Move;
	bool canAction = true;

	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Add point"))
			state = RoomState.AddPoint;

		if (GUILayout.Button("Remove point"))
			state = RoomState.RemovePoint;

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Add door"))
			state = RoomState.AddDoor;

		if (GUILayout.Button("Remove door"))
			state = RoomState.RemoveDoor;

		EditorGUILayout.EndHorizontal();

		if (GUILayout.Button("Create Room"))
			state = RoomState.CreateRoom;

		EditorGUILayout.BeginHorizontal();

		if (GUILayout.Button("Generate mesh"))
			GenerateMesh();

		if (GUILayout.Button("Clear mesh"))
			ClearMesh();

		EditorGUILayout.EndHorizontal();

		for (int i = 0; i < rooms.Count; i++)
		{
			EditorRoom room = rooms[i];

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label($"Room {i}: Containing {room.Points.Count} verticies");

			if (GUILayout.Button("Remove"))
				rooms.RemoveAt(i);

			EditorGUILayout.EndHorizontal();
		}
	}

	private void OnSceneGUI()
	{
		DrawNodeSpheres(state == RoomState.Move);
		DrawRooms();
		DrawDoors();
		DoEvents();

		if (state == RoomState.CreateRoom)
			DrawSelectedNodes();

		// Draw where sphere will be placed
		var closest = GetClosestLinePoint();
		if (state == RoomState.AddPoint)
		{
			if (Event.current.shift && closest.room != null)
				Handles.SphereHandleCap(0, closest.p, Quaternion.identity, 0.5f, EventType.Repaint);
			else
			{
				Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				Plane plane = new Plane(Handles.matrix * Vector3.up, Handles.matrix.MultiplyPoint(Vector3.zero));
				plane.Raycast(ray, out float dist);

				Vector3 point = default;
				if (dist > 0)
					point = ray.GetPoint(dist);

				if (Event.current.control)
					point = new Vector3(Mathf.Round(point.x), 0, Mathf.Round(point.z));

				Handles.SphereHandleCap(1, point, Quaternion.identity, 0.5f, EventType.Repaint);
			}
		}

		if (closest.room != null &&  state == RoomState.AddDoor)
		{
			int p1 = closest.index;
			int p2 = closest.index - 1;

			if (p2 < 0)
				p2 = closest.room.Points.Count - 1;

			if (IsDoubleSided(closest.room.Points[p1], closest.room.Points[p2]))
			{
				Handles.color = new Color(0.45f, 0.2f, 0.04f); // Brown
				Handles.CubeHandleCap(2, closest.p, Quaternion.identity, 0.5f, EventType.Repaint);
				Handles.color = Color.white;
			}
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
				if (i == closestIndex && state == RoomState.RemovePoint)
					Handles.color = Color.red;
				else if (i == closestIndex && state == RoomState.CreateRoom)
					Handles.color = Color.gray;
				else if (selectedNodes.Contains(i) && state == RoomState.CreateRoom)
					Handles.color = Color.gray;

				if (selectedNodes.Count > 0 && i == selectedNodes[0])
					Handles.color = Color.green;

				Handles.SphereHandleCap(100 + i, points[i], Quaternion.identity, 0.5f, EventType.Repaint);
				Handles.color = Color.white;
			}
		}
	}

	void DrawSelectedNodes()
	{
		Handles.color = Color.gray;
		for (int i = 1; i < selectedNodes.Count; i++)
		{
			int p1 = selectedNodes[i - 1];
			int p2 = selectedNodes[i];

			Handles.DrawLine(points[p1], points[p2]);
		}

		Handles.color = Color.white;
	}

	void DrawRooms()
	{
		for (int i = 0; i < rooms.Count; i++)
			DrawRoom(rooms[i]);
	}

	void DrawRoom(EditorRoom room)
	{
		for (int i = 1; i < room.Points.Count; i++)
		{
			int p1 = room.Points[i - 1];
			int p2 = room.Points[i];

			Handles.color = Color.white;
			if (IsDoubleSided(p1, p2))
				Handles.color = Color.gray;

			Handles.DrawLine(points[p1], points[p2]);
		}

		int lastP1 = room.Points[room.Points.Count - 1];
		int lastP2 = room.Points[0];
		Handles.color = Color.white;
		if (IsDoubleSided(lastP1, lastP2))
			Handles.color = Color.gray;

		Handles.DrawLine(points[lastP1], points[lastP2]);
		Handles.color = Color.white;
	}

	void DrawDoors()
	{
		Handles.color = new Color(0.45f, 0.2f, 0.04f); // Brown
		for (int i = 0; i < doors.Count; i++)
		{
			EditorDoor door = doors[i];
			if (i == GetClosestDoor() && state == RoomState.RemoveDoor)
				Handles.color = Color.red;

			Handles.CubeHandleCap(2, door.Pos, Quaternion.identity, 0.5f, EventType.Repaint);
		}

		Handles.color = Color.white;
	}

	void GenerateMesh()
	{
		GameObject obj = ((TestScript)target).gameObject;
		HashSet<(int, int)> walls = new HashSet<(int, int)>();

		GameObject roomObj = new GameObject();
		roomObj.transform.parent = obj.transform;
		roomObj.name = "Rooms";
		foreach (EditorRoom room in rooms)
		{
			GameObject roomObject = new GameObject();
			roomObject.transform.parent = roomObj.transform;
			roomObject.name = "Rooms";

			MeshRenderer meshRenderer = roomObject.AddComponent<MeshRenderer>();
			MeshFilter meshFilter = roomObject.AddComponent<MeshFilter>();

			Material defaultMat = Resources.Load<Material>("Default");
			meshFilter.mesh = GenerateFloor(room);
			meshRenderer.material = defaultMat;

			for (int i = 1; i < room.Points.Count; i++)
			{
				int p1 = room.Points[i];
				int p2 = room.Points[i - 1];

				walls.Add((p1, p2));
			}

			int lastP1 = room.Points[0];
			int lastP2 = room.Points[room.Points.Count - 1];

			walls.Add((lastP1, lastP2));
		}

		GameObject wallObj = new GameObject();
		wallObj.transform.parent = obj.transform;
		wallObj.name = "Walls";
		foreach (var wall in walls)
		{
			Vector3 p1 = points[wall.Item1];
			Vector3 p2 = points[wall.Item2];

			Vector3 half = (p2 - p1) / 2;
			float angle = Vector3.SignedAngle(Vector3.right, half.normalized, Vector3.up);

			GameObject wallObject = Instantiate(Resources.Load<GameObject>("Wall"));
			wallObject.transform.parent = wallObj.transform;
			wallObject.transform.position = p1 + half;
			wallObject.transform.localScale = new Vector3(half.magnitude * 2, 2, 0.2f);
			wallObject.transform.rotation = Quaternion.Euler(0, angle, 0);
		}

		GameObject doorObj = new GameObject();
		doorObj.transform.parent = obj.transform;
		doorObj.name = "Doors";
		foreach (EditorDoor door in doors)
		{
			Vector3 p1 = points[door.Node1];
			Vector3 p2 = points[door.Node2];

			float angle = Vector3.SignedAngle(Vector3.right, (p2 - p1).normalized, Vector3.up);

			GameObject wallObject = Instantiate(Resources.Load<GameObject>("Door"));
			wallObject.transform.parent = doorObj.transform;
			wallObject.transform.position = door.Pos;
			wallObject.transform.rotation = Quaternion.Euler(0, angle, 0);
		}
	}

	void ClearMesh()
	{
		GameObject obj = ((TestScript)target).gameObject;

		foreach (Transform child in obj.transform)
			GameObject.DestroyImmediate(child.gameObject);
	}

	Mesh GenerateFloor(EditorRoom room)
	{
		Vector3[] verticies = room.Points.Select(x => points[x]).ToArray();
		Triangulator tr = new Triangulator(verticies.Select(x => new Vector2(x.x, x.z)).ToArray());
		int[] indicies = tr.Triangulate();

		Mesh mesh = new Mesh();
		mesh.vertices = verticies;
		mesh.triangles = indicies;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		return mesh;
	}

	void DoEvents()
	{
		int controlID = GUIUtility.GetControlID(FocusType.Passive | FocusType.Keyboard);
		EventType eventType = Event.current.GetTypeForControl(controlID);

		switch (eventType)
		{
			case EventType.MouseDown:
				MouseDownEvent();
				break;
			case EventType.MouseUp:
			case EventType.Used:
				MouseUpEvent();
				break;
			case EventType.KeyDown:
				KeyDownEvent();
				break;
			default:
				break;
		}
	}

	void MouseDownEvent()
	{
		if (Event.current.button == 1)
		{
			state = RoomState.Move;
			selectedNodes.Clear();
		}

		switch (state)
		{
			case RoomState.AddPoint:
				if (canAction)
				{
					int closest = GetClosestPoint();
					Vector3 point = default;

					Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
					Plane plane = new Plane(Handles.matrix * Vector3.up, Handles.matrix.MultiplyPoint(Vector3.zero));
					plane.Raycast(ray, out float dist);

					if (dist > 0)
						point = ray.GetPoint(dist);

					if (Event.current.control)
						point = new Vector3(Mathf.Round(point.x), 0, Mathf.Round(point.z));

					points.Insert(closest, point);
				}

				canAction = false;
				if (!Event.current.shift)
					state = RoomState.Move;

				break;
			case RoomState.RemovePoint:
				if (canAction)
					points.RemoveAt(GetClosestPoint());

				canAction = false;
				if (!Event.current.shift)
					state = RoomState.Move;

				break;
			case RoomState.CreateRoom:
				if (canAction)
				{
					int index = GetClosestPoint();

					if (!selectedNodes.Contains(index))
						selectedNodes.Add(index);

					if (index == selectedNodes[0] && selectedNodes.Count > 1)
					{
						EditorRoom room = new EditorRoom();
						room.AddRange(selectedNodes);
						rooms.Add(room);

						selectedNodes.Clear();
						state = RoomState.Move;
					}
				}

				canAction = false;
				break;
			case RoomState.AddDoor:
				if (canAction)
				{
					var closesPoint = GetClosestLinePoint();

					int p1 = closesPoint.index;
					int p2 = closesPoint.index - 1;

					if (p2 < 0)
						p2 = closesPoint.room.Points.Count - 1;

					var rooms = GetWallRooms(closesPoint.room.Points[p1], closesPoint.room.Points[p2]);
					doors.Add(new EditorDoor(rooms.r1, rooms.r2, closesPoint.room.Points[p1], closesPoint.room.Points[p2], closesPoint.p));
				}

				state = RoomState.Move;
				canAction = false;
				break;
			case RoomState.RemoveDoor:
				if (canAction)
					doors.RemoveAt(GetClosestDoor());

				canAction = false;
				if (!Event.current.shift)
					state = RoomState.Move;

				break;
			default:
				break;
		}
	}

	void MouseUpEvent()
	{
		canAction = true;
	}

	void KeyDownEvent()
	{
		if (Event.current.keyCode == KeyCode.Escape)
			state = RoomState.Move;
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

	int GetClosestDoor()
	{
		float dist = float.MaxValue;
		int point = -1;

		for (int i = 0; i < doors.Count; i++)
		{
			float newDist = Vector2.Distance(Event.current.mousePosition, HandleUtility.WorldToGUIPoint(doors[i].Pos));

			if (newDist < dist)
			{
				dist = newDist;
				point = i;
			}
		}

		return point;
	}

	(Vector3 p, EditorRoom room, int index) GetClosestLinePoint()
	{
		Vector3 point = default;
		float pointDist = 0;
		int index = -1;
		EditorRoom returnRoom = default;
		foreach (EditorRoom room in rooms)
		{
			if (room.Points.Count == 0)
				continue;

			for (int i = 1; i < room.Points.Count; i++)
			{
				int p1 = room.Points[i];
				int p2 = room.Points[i - 1];

				float newDist = HandleUtility.DistanceToPolyLine(points[p2], points[p1]);
				if (point == default || newDist < pointDist)
				{
					point = HandleUtility.ClosestPointToPolyLine(points[p2], points[p1]);
					pointDist = newDist;
					index = i;
					returnRoom = room;
				}
			}

			int lastP1 = room.Points[0];
			int lastP2 = room.Points[room.Points.Count - 1];

			float lastNewDist = HandleUtility.DistanceToPolyLine(points[lastP1], points[lastP2]);
			if (point == default || lastNewDist < pointDist)
			{
				point = HandleUtility.ClosestPointToPolyLine(points[lastP1], points[lastP2]);
				index = lastP1;
				returnRoom = room;
			}
		}

		return (point, returnRoom, index);
	}

	bool IsDoubleSided(int start, int end)
	{
		int sides = 0;
		for (int i = 0; i < rooms.Count; i++)
		{
			EditorRoom room = rooms[i];
			for (int a = 1; a < room.Points.Count; a++)
			{
				if (room.Points[a - 1] == start && room.Points[a] == end)
					sides++;

				if (room.Points[a - 1] == end && room.Points[a] == start)
					sides++;
			}

			if (room.Points[0] == start && room.Points[room.Points.Count - 1] == end)
				sides++;

			if (room.Points[0] == end && room.Points[room.Points.Count - 1] == start)
				sides++;
		}

		return sides > 1;
	}

	(int r1, int r2) GetWallRooms(int start, int end)
	{
		List<int> foundRooms = new List<int>();
		for (int i = 0; i < rooms.Count; i++)
		{
			EditorRoom room = rooms[i];
			for (int a = 1; a < room.Points.Count; a++)
			{
				if (room.Points[a - 1] == start && room.Points[a] == end)
					foundRooms.Add(i);

				if (room.Points[a - 1] == end && room.Points[a] == start)
					foundRooms.Add(i);
			}

			if (room.Points[0] == start && room.Points[room.Points.Count - 1] == end)
				foundRooms.Add(i);

			if (room.Points[0] == end && room.Points[room.Points.Count - 1] == start)
				foundRooms.Add(i);
		}

		return (foundRooms[0], foundRooms[1]);
	}
}

enum RoomState
{
	Move,
	AddPoint,
	RemovePoint,
	CreateRoom,
	AddDoor,
	RemoveDoor,
}