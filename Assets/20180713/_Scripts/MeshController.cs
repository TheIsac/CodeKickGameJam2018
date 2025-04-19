using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshController : MonoBehaviour
{
	public void SetColor(Color color)
	{
		GetComponent<Renderer>().material.color = color;
	}
}
