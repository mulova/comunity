using UnityEngine;
using System.Collections;
using System;
using Object = UnityEngine.Object;

public class ShaderSearchItem : IComparable<ShaderSearchItem>
{
	public Renderer rend;
	public Material material;

	public string name
	{
		get
		{
			return material.shader.name;
		}
	}

	public ShaderSearchItem(Renderer r, Material mat)
	{
		this.rend = r;
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
        return i.material;
	}

	public static explicit operator GameObject(ShaderSearchItem i)
	{
        if (i.rend != null)
        {
            return i.rend.gameObject;
        }
        return null;
	}
}

