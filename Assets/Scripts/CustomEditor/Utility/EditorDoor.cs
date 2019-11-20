using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EditorDoor
{
	public int Room1 { get; }

	public int Room2 { get; }

	public int Node1 { get; }

	public int Node2 { get; }

	public Vector3 Pos { get; }

	public EditorDoor(int room1, int room2, int node1, int node2, Vector3 pos)
	{
		this.Room1 = room1;
		this.Room2 = room2;
		this.Node1 = node1;
		this.Node2 = node2;
		this.Pos = pos;
	}
}
