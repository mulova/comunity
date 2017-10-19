#if FULL
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/**
 * 실시간으로 멤버 변수값을 확인한다.
 * 1. 변수 값을 확인하고자 하는 Component와 동일한 GameObject에 ComponentInspector를 추가한다.
 * 2. variable/property이름(Private포함)을 memberNames에 지정한다.
 * 3. 기본적으로는 collider를 사용하여 mouse hover일 경우에 값을 볼수 있다.
 * 4. collider가 없거나 항상 값을 보고자 할 경우 alwaysTitle을 지정한다.
 * 5. Variable이나 Property가 아닌 값을 보고 싶을 경우에는 InspectorPlugin을 상속받아 구현한 후 ComponentInspector와 함께 추가한다.
 * 
 */
public class NameInspector : Inspector
{
	public string[] memberNames;
	
	protected override void AddDescriptors(List<Descriptor> list) {
		foreach (Component component in gameObject.GetComponents<Component>()) {
			if (component == this) {
				continue;
			}
			if (component is InspectorPlugin) {
				list.Add (new Descriptor ((InspectorPlugin)component));
			} else {
				foreach (string name in memberNames) {
					FieldInfo field = component.GetType().GetField(name, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.inst);
					if (field != null) {
						list.Add (new Descriptor (component, field, null));
						continue;
					}
					PropertyInfo prop = component.GetType().GetProperty(name, BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.inst);
					if (prop != null) {
						list.Add (new Descriptor (component, prop, null));
					}
				}
			}
			
		}
	}
}
#endif