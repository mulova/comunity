using UnityEngine;
using System.Collections;
using System;
using Object = UnityEngine.Object;

public class ShaderSearchItem : IComparable<ShaderSearchItem>
{
	public Object obj;
	public Material material;

	public string name
	{
		get
		{
			return material.shader.name;
		}
	}

	public ShaderSearchItem(Object obj, Material mat)
	{
		this.obj = obj;
		this.material = mat;
	}

	public int CompareTo(ShaderSearchItem other)
	{
		if (other == null) {
			return -1;
		}
		return this.name.CompareTo(other.name);
	}

	public static explicit operator Object(ShaderSearchItem i)
	{
		return i.obj;
	}

	public static explicit operator GameObject(ShaderSearchItem i)
	{
		return i.obj as GameObject;
	}
}

