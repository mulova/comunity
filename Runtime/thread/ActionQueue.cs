﻿using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.Ex;
using mulova.commons;

namespace mulova.comunity
{
	public class ActionQueue
	{
		private List<Action> actions = new List<Action>();
		private List<Action> actionsLater = new List<Action>();
		public static readonly ILog log = LogManager.GetLogger(nameof(ActionQueue));

		public bool isEmpty
		{
			get { return actions.IsEmpty()&&actionsLater.IsEmpty(); }
		}

		public void Update()
		{
			if (!actions.IsEmpty())
			{
				foreach (Action a in actions)
				{
					try
					{
						a.Call();
					} catch (Exception ex)
					{
						log.Error(ex);
					}
				}
				actions.Clear();
			}
			lock(this)
			{
				var temp = actionsLater;
				actionsLater = actions;
				actions = temp;
			}
		}

		public void Add(Action a)
		{
			lock(this)
			{
				actionsLater.Add(a);
			}
		}
	}
}

