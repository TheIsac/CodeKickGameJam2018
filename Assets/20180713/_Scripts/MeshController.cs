using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshController : MonoBehaviour {
	public void SetColor(Color color)
	{
		GetComponent<Renderer>().material.color = color;
	}

	public void SetColorByPlayerOrder(int order)
	{
		if (order == 1)
		{
			SetColor(Color.yellow);
		}
		else if (order == 2)
		{
			SetColor(Color.magenta);
		}
		else if (order == 3)
		{
			SetColor(Color.green);
		}
		else if (order == 4)
		{
			SetColor(Color.cyan);
		}
	}
}
