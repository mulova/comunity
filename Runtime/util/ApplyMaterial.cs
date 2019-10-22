//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using UnityEngine;
using mulova.commons;

namespace mulova.comunity {

    public class ApplyMaterialColor {
		private string name;
		private Color color;
		
		public ApplyMaterialColor(string name, Color c) {
			this.name = name;
			this.color = c;
		}
		
		public void Apply(Material target)
		{
			target.SetColor(name, color);
		}
		
	}
	
	
	public class ApplyMaterialFloat {
		private string name;
		private float f;
		
		public ApplyMaterialFloat(string name, float f) {
			this.name = name;
			this.f = f;
		}
		
		public void Apply(Material target)
		{
			target.SetFloat(name, f);
		}
	}
	
	
	public class ApplyMaterialTexture {
		private string name;
		private Texture tex;
		
		public ApplyMaterialTexture(string name, Texture tex) {
			this.name = name;
			this.tex = tex;
		}
		
		public void Apply(Material target)
		{
			target.SetTexture(name, tex);
		}
	}
	
	
	public class ApplyMaterialVec4 {
		private string name;
		private Vector4 vec;
		
		public ApplyMaterialVec4(string name, Vector4 vec) {
			this.name = name;
			this.vec = vec;
		}
		
		public void Apply(Material target)
		{
			target.SetVector(name, vec);
		}
	}
	
}