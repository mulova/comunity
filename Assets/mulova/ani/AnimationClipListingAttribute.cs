using UnityEngine;
 
public class AnimationClipListingAttribute : PropertyAttribute {
 
	public AnimationClipListingAttribute() {}

	public string varName;
	public AnimationClipListingAttribute(string animVarName) {
		this.varName = animVarName;
	}
	
}