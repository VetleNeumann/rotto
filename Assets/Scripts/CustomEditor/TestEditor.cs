using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;
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
	bool canClick = true;
	bool meshGenerated = false;

	int spawnRoom = 0;
	int selectedRoom = -1;

	int minimapWidth = 100;
	int minimapHeight = 100;
	int minimapPadding = 10;
	int minimapWallThickness = 1;

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
			GUILayout.Label($"Room {i}: {room.Points.Count} verticies");

			if (GUILayout.Button("P"))
			{
				if (selectedRoom == i)
					selectedRoom = -1;
				else
					selectedRoom = i;
			}

			if (GUILayout.Button("Remove"))
			{
				rooms.RemoveAt(i);
				doors.RemoveAt(doors.FindIndex(x => x.Room1 == i || x.Room2 == i));
			}

			EditorGUILayout.EndHorizontal();
		}

		if (rooms.Count > 0)
		{
			if (GUILayout.Button("Set spawn"))
				state = RoomState.SetSpawn;
		}

		if (meshGenerated && GUILayout.Button("Finish Mesh"))
			FinishMesh();

		GUILayout.Space(10);
		GUILayout.Label($"Minimap");
		EditorGUILayout.BeginHorizontal();

		GUILayout.Label($"Size: ");
		minimapWidth = EditorGUILayout.IntField(minimapWidth);
		minimapHeight = EditorGUILayout.IntField(minimapHeight);

		EditorGUILayout.EndHorizontal();

		minimapPadding = EditorGUILayout.IntField("Padding", minimapPadding);

		minimapWallThickness = EditorGUILayout.IntField("Wall Thickness", minimapWallThickness);

		if (GUILayout.Button("Generate minimap"))
		{
			Texture2D minimap = GenerateMinimap();
			AssetDatabase.DeleteAsset($"Assets/Minimap.png");
			File.WriteAllBytes($"{Application.dataPath}/Minimap.png", minimap.EncodeToPNG());
			AssetDatabase.Refresh();
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

			//Debug.Log($"Room nodes {closest.room.Points.Count} {p1}, {p2}");
			if (IsDoubleSided(closest.room.Points[p1], closest.room.Points[p2]))
			{
				Handles.color = new Color(0.45f, 0.2f, 0.04f); // Brown
				Handles.CubeHandleCap(2, closest.p, Quaternion.identity, 0.5f, EventType.Repaint);
				Handles.color = Color.white;
			}
		}

		if (selectedNodes.Count > 0 && state == RoomState.CreateRoom)
		{
			int closestNode = GetClosestPoint();

			Handles.color = new Color(0.18f, 0.68f, 0.7f, 0.25f); // Light blue
			Handles.DrawAAConvexPolygon(selectedNodes.Concat(new[] { closestNode }).Select(x => points[x]).ToArray());
			Handles.color = Color.white;
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
			DrawRoom(rooms[i], i);
	}

	void DrawRoom(EditorRoom room, int index)
	{
		for (int i = 1; i < room.Points.Count; i++)
		{
			int p1 = room.Points[i - 1];
			int p2 = room.Points[i];

			Handles.color = Color.white;
			if (IsDoubleSided(p1, p2))
				Handles.color = Color.black;

			Handles.DrawLine(points[p1], points[p2]);
		}

		int lastP1 = room.Points[room.Points.Count - 1];
		int lastP2 = room.Points[0];
		Handles.color = Color.white;
		if (IsDoubleSided(lastP1, lastP2))
			Handles.color = Color.black;

		Handles.DrawLine(points[lastP1], points[lastP2]);

		Handles.color = new Color(0.18f, 0.68f, 0.7f, 0.25f); // Light blue
		if (state == RoomState.SetSpawn && index == GetClosestRoom())
			Handles.color = new Color(0.89f, 0.92f, 0.18f, 0.25f); // Light yellow

		if (state != RoomState.SetSpawn && index == spawnRoom)
			Handles.color = new Color(0.89f, 0.92f, 0.18f, 0.25f); // Light yellow

		if (selectedRoom == index)
			Handles.color = new Color(0.89f, 0.02f, 0.18f, 0.25f); // Light yellow

		Handles.DrawAAConvexPolygon(room.Points.Select(x => points[x]).ToArray());
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
			roomObject.AddComponent<RoomController>();

			Material defaultMat = Resources.Load<Material>("Default");
			meshFilter.mesh = GenerateFloor(room);
			meshRenderer.material = defaultMat;

			for (int i = 1; i < room.Points.Count; i++)
			{
				int p1 = room.Points[i];
				int p2 = room.Points[i - 1];

				if (p1 < p2)
					walls.Add((p1, p2));
				else
					walls.Add((p2, p1));
			}

			int lastP1 = room.Points[0];
			int lastP2 = room.Points[room.Points.Count - 1];

			if (lastP1 < lastP2)
				walls.Add((lastP1, lastP2));
			else
				walls.Add((lastP2, lastP1));
		}

		GameObject wallObj = new GameObject();
		wallObj.transform.parent = obj.transform;
		wallObj.name = "Walls";
		foreach (var wall in walls)
		{
			Vector3 p1 = points[wall.Item1] + Vector3.up / 2f;
			Vector3 p2 = points[wall.Item2] + Vector3.up / 2f;

			int doorIndex = GetDoor(wall.Item1, wall.Item2);
			EditorDoor door = default;
			if (doorIndex != -1)
				door = doors[doorIndex];

			Vector3 half = (p2 - p1) / 2;
			float angle = Vector3.SignedAngle(Vector3.right, half.normalized, Vector3.up);

			if (doorIndex == -1)
			{
				GameObject wallObject = Instantiate(Resources.Load<GameObject>("Wall"));
				wallObject.transform.parent = wallObj.transform;
				wallObject.transform.position = p1 + half;
				wallObject.transform.localScale = new Vector3(half.magnitude * 2, 2, 0.2f);
				wallObject.transform.rotation = Quaternion.Euler(0, angle, 0);
			}
			else
			{
				float doorSize = 1.25f;
				float doorDistP1 = Vector3.Distance(door.Pos, p1) - doorSize / 2;
				float doorDistP2 = Vector3.Distance(door.Pos, p2) - doorSize / 2;

				GameObject wallObject = Instantiate(Resources.Load<GameObject>("Wall"));
				wallObject.transform.parent = wallObj.transform;
				wallObject.transform.position = p1 + (p2 - p1).normalized * doorDistP1 / 2;
				wallObject.transform.localScale = new Vector3(doorDistP1, 2, 0.2f);
				wallObject.transform.rotation = Quaternion.Euler(0, angle, 0);

				wallObject = Instantiate(Resources.Load<GameObject>("Wall"));
				wallObject.transform.parent = wallObj.transform;
				wallObject.transform.position = p2 + (p1 - p2).normalized * doorDistP2 / 2;
				wallObject.transform.localScale = new Vector3(doorDistP2, 2, 0.2f);
				wallObject.transform.rotation = Quaternion.Euler(0, angle, 0);
			}
		}

		GameObject doorObj = new GameObject();
		doorObj.transform.parent = obj.transform;
		doorObj.name = "Doors";
		for (int i = 0; i < doors.Count; i++)
		{
			EditorDoor door = doors[i];

			Vector3 p1 = points[door.Node1];
			Vector3 p2 = points[door.Node2];

			float angle = Vector3.SignedAngle(Vector3.right, (p2 - p1).normalized, Vector3.up);

			GameObject doorObject = Instantiate(Resources.Load<GameObject>("Door"));
			doorObject.transform.parent = doorObj.transform;
			doorObject.transform.position = door.Pos;
			doorObject.transform.rotation = Quaternion.Euler(0, angle, 0);

			DoorController doorController = doorObject.GetComponent<DoorController>();
			roomObj.transform.GetChild(door.Room1).GetComponent<RoomController>().AddDoor(doorController);
			roomObj.transform.GetChild(door.Room2).GetComponent<RoomController>().AddDoor(doorController);
		}

		meshGenerated = true;
	}

	void ClearMesh()
	{
		GameObject obj = ((TestScript)target).gameObject;

		foreach (Transform child in obj.transform)
			GameObject.DestroyImmediate(child.gameObject);

		foreach (Transform child in obj.transform)
			GameObject.DestroyImmediate(child.gameObject);

		meshGenerated = false;
	}

	void FinishMesh()
	{
		GameObject obj = ((TestScript)target).gameObject;

		GameObject mapObj = new GameObject();
		mapObj.name = "Map";

		for (int i = 0; i < obj.transform.childCount; i++)
			obj.transform.GetChild(i).parent = mapObj.transform;

		for (int i = 0; i < obj.transform.childCount; i++)
			obj.transform.GetChild(i).parent = mapObj.transform;

		meshGenerated = false;
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

	int GetDoor(int p1, int p2)
	{
		return doors.FindIndex(x => (x.Node1 == p1 || x.Node1 == p2) && (x.Node2 == p1 || x.Node2 == p2));
	}

	Texture2D GenerateMinimap()
	{
		Texture2D tex = new Texture2D(minimapWidth + minimapPadding, minimapHeight + minimapPadding);

		// Fill background black
		for (int y = 0; y < minimapHeight + minimapPadding; y++)
		{
			for (int x = 0; x < minimapWidth + minimapPadding; x++)
			{
				tex.SetPixel(x, y, Color.black);
			}
		}

		Vector2 xBounds = new Vector2(points.Min(x => x.x), points.Max(x => x.x));
		Vector2 yBounds = new Vector2(points.Min(x => x.z), points.Max(x => x.z));

		//Debug.Log(xBounds);
		//Debug.Log(zBounds);
		Vector2Int[] minimapPoints = new Vector2Int[points.Count];
		for (int i = 0; i < points.Count; i++)
		{
			Vector3 point = points[i];

			float px = Mathf.InverseLerp(xBounds.x, xBounds.y, point.x);
			float py = Mathf.InverseLerp(yBounds.x, yBounds.y, point.z);

			//Debug.Log($"Px: {px}, Py: {py}, PosX: {Mathf.RoundToInt(px * minimapWidth + minimapPadding)}, PosY: {Mathf.RoundToInt(py * minimapHeight + minimapPadding)}");

			minimapPoints[i] = new Vector2Int(Mathf.RoundToInt(px * minimapWidth + minimapPadding / 2f), Mathf.RoundToInt(py * minimapHeight + minimapPadding / 2f));
		}

		foreach (EditorRoom room in rooms)
		{
			for (int i = 1; i < room.Points.Count; i++)
			{
				Vector2Int p1 = minimapPoints[room.Points[i]];
				Vector2Int p2 = minimapPoints[room.Points[i - 1]];

				DrawLine(ref tex, p1, p2, minimapWallThickness, Color.white);
			}

			DrawLine(ref tex, minimapPoints[room.Points[0]], minimapPoints[room.Points[room.Points.Count - 1]], minimapWallThickness, Color.white);
		}

		foreach (EditorDoor door in doors)
		{
			Vector3 point = door.Pos;

			float px = Mathf.InverseLerp(xBounds.x, xBounds.y, point.x);
			float py = Mathf.InverseLerp(yBounds.x, yBounds.y, point.z);

			int posX = Mathf.RoundToInt(px * minimapWidth + minimapPadding / 2f);
			int posY = Mathf.RoundToInt(py * minimapHeight + minimapPadding / 2f);

			DrawCircle(ref tex, new Vector2Int(posX, posY), 6, Color.black);
			//DrawBox(ref tex, new Vector2Int(posX, posY), new Vector2Int(4, minimapWallThickness), Color.black);
		}

		tex.Apply();

		return tex;
	}

	void DrawLine(ref Texture2D tex, Vector2Int p1, Vector2Int p2, int thickness, Color col)
	{
		Vector2 t = p1;
		float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
		float ctr = 0;

		while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
		{
			t = Vector2.Lerp(p1, p2, ctr);
			ctr += frac;
			DrawCircle(ref tex, new Vector2Int((int)t.x, (int)t.y), thickness, col);
			//tex.SetPixel((int)t.x, (int)t.y, col);
		}
	}

	void DrawCircle(ref Texture2D tex, Vector2Int point, int radius, Color col)
	{
		float rSquared = radius * radius;

		for (int u = point.x - radius; u < point.x + radius + 1; u++)
			for (int v = point.y - radius; v < point.y + radius + 1; v++)
				if ((point.x - u) * (point.x - u) + (point.y - v) * (point.y - v) < rSquared)
					tex.SetPixel(u, v, col);
	}

	void DrawBox(ref Texture2D tex, Vector2Int pos, Vector2Int size, Color col)
	{
		for (int x = -size.x / 2; x < size.x / 2; x++)
		{
			for (int y = -size.y / 2; y < size.y / 2; y++)
			{
				tex.SetPixel(pos.x + x, pos.y + y, col);
			}
		}
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
				if (canClick)
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

					points.Add(point);
				}

				canClick = false;
				if (!Event.current.shift)
					state = RoomState.Move;

				break;
			case RoomState.RemovePoint:
				if (canClick)
					points.RemoveAt(GetClosestPoint());

				canClick = false;
				if (!Event.current.shift)
					state = RoomState.Move;

				break;
			case RoomState.CreateRoom:
				if (canClick)
				{
					int index = GetClosestPoint();

					if (!selectedNodes.Contains(index))
						selectedNodes.Add(index);

					if (index == selectedNodes[0] && selectedNodes.Count > 1)
					{
						Vector3 roomPos = selectedNodes.Select(x => points[x]).Aggregate((x, y) => x + y) / selectedNodes.Count;

						EditorRoom room = new EditorRoom(roomPos);
						room.AddRange(selectedNodes);
						rooms.Add(room);

						selectedNodes.Clear();
						state = RoomState.Move;
					}
				}

				canClick = false;
				break;
			case RoomState.AddDoor:
				if (canClick)
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
				canClick = false;
				break;
			case RoomState.RemoveDoor:
				if (canClick)
					doors.RemoveAt(GetClosestDoor());

				canClick = false;
				if (!Event.current.shift)
					state = RoomState.Move;

				break;
			case RoomState.SetSpawn:
				spawnRoom = GetClosestRoom();
				state = RoomState.Move;
				break;
			default:
				state = RoomState.Move;
				break;
		}
	}

	void MouseUpEvent()
	{
		canClick = true;
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

	int GetClosestRoom()
	{
		float dist = float.MaxValue;
		int point = -1;

		for (int i = 0; i < rooms.Count; i++)
		{
			float newDist = Vector2.Distance(Event.current.mousePosition, HandleUtility.WorldToGUIPoint(rooms[i].Pos));

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
				index = 0;
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

		if (foundRooms.Count != 2)
			return (-1, -1);

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
	SetSpawn,
}