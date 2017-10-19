#if FULL
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/**
 * If alwaysTitle is not empty, Inspector is shown by the title
 */
public abstract class Inspector : MonoBehaviour
{
	public string alwaysTitle;
	public Rect bound = new Rect(0, 0, 300, 200);
	
	private List<Descriptor> descriptors = new List<Descriptor> ();
	private GameInspectorManager manager;
	private GameInspectorManager sharedManager;
	public List<Descriptor> Descriptors {
		get {
			return descriptors;
		}
	}
	
	void Start ()
	{
		if (Platform.isEditor) {
			GameObject managerObj = GameObject.Find("/InspectorManager");
			
			if (managerObj == null) {
				managerObj = new GameObject("InspectorManager");
				sharedManager = managerObj.AddComponent<GameInspectorManager>();
				GameObject.DontDestroyOnLoad(managerObj);
			} else {
				sharedManager = managerObj.GetComponent<GameInspectorManager>();
			}
			AddDescriptors(descriptors);
		} else {
			Debug.LogWarning("Remove Inspector");
			Destroy(this);
		}
	}
	
	void OnDisable() {
		if (manager != null) {
			manager.Hide(this);
		}
		sharedManager.Hide(this);
	}
	
	void Update() {
		if (alwaysTitle.Length > 0) {
			if (manager == null) {
				manager = gameObject.AddComponent<GameInspectorManager>();
			}
			manager.windowTitle = alwaysTitle;
			manager.followMouse = false;
			manager.bound = bound;
			manager.Show(this);
		} else {
			if (manager != null) {
				manager.Hide(this);
			}
		}
	}

	protected abstract void AddDescriptors(List<Descriptor> list);
	
	void OnMouseEnter() {
		sharedManager.Show(this);
	}
	
	void OnMouseExit() {
		sharedManager.Hide(this);
	}
}
#endif