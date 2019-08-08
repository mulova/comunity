#if FULL
using UnityEngine;
using System.Reflection;

public class Descriptor
{
	private Component component;
	private FieldInfo fieldInfo;
	private PropertyInfo propertyInfo;
	private InspectableAttribute attr;
	private InspectorPlugin plugin;
	
	public Descriptor (InspectorPlugin plugin) {
		this.plugin = plugin;
	}
	
	public Descriptor (Component component, FieldInfo info, InspectableAttribute attr)
	{
		this.component = component;
		this.fieldInfo = info;
		this.attr = attr;
	}
	
	public Descriptor (Component component, PropertyInfo info, InspectableAttribute attr)
	{
		this.component = component;
		this.propertyInfo = info;
		this.attr = attr;
	}
	
	public string Title {
		get {
			if (attr != null) {
				return attr.niceName;
			} else if (fieldInfo != null) {
				return fieldInfo.Name;
			} else if (propertyInfo != null) {
				return propertyInfo.Name;
			} else if (plugin != null) {
				return plugin.GetTitle();
			}
			return null;
		}
	}
	
	public object Value {
		get { 
			if (fieldInfo != null) {
				return fieldInfo.GetValue (component); 
			} else if (propertyInfo != null) {
				return propertyInfo.GetValue (component, null); 
			} else if (plugin != null) {
				return plugin.Inspect();
			}
			return null;
		}
	}
	
	public InspectableAttribute Attr {
		get {
			return attr;
		}
	}
}
#endif