using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorRoom
{
    public List<int> Points { get; }

	public EditorRoom()
	{
		Points = new List<int>();
	}

	public void AddPoint(int i)
	{
		Points.Add(i);
	}

	public void AddRange(IEnumerable<int> collection)
	{
		Points.AddRange(collection);
	}

	public void RemovePoint(int i)
	{
		Points.Remove(i);
	}
}
