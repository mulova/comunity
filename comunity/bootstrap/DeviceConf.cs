using System.Collections.Generic;
using UnityEngine;
using System;

public class DeviceConf
{
    //public static readonly DeviceConf IPHONE_X = new DeviceConf(id: "iPhone10,6", displayName: "iPhoneX", width: 1125, height:2436, vPadding:102, deltaY: 0);
    public static readonly DeviceConf IPHONE_X = new DeviceConf(displayName: "iPhoneX(S)", vPadding:40, deltaY: 35, models: new[] { "iPhone10,3", "iPhone10,6", "iPhone11,2" });
	public static readonly DeviceConf IPHONE_XSMAX = new DeviceConf(displayName: "iPhoneXsMax", vPadding: 45, deltaY: 40, models: new[] { "iPhone11,6" });
	public static readonly DeviceConf IPHONE_XR = new DeviceConf(displayName: "iPhoneXR", vPadding:30, deltaY: 30, models: new[] { "iPhone11,8" });
	//public static readonly DeviceConf IPAD_2018 = new DeviceConf(id: "iPad8,3", displayName: "iPad Pro A1980", width: 1668, height:2388, vPadding:0, deltaY: 0);
	public static readonly DeviceConf OPPO_R15PRO = new DeviceConf(displayName: "OPPO R15 Pro", vPadding:20, deltaY: 40, models: new[] { "OPPO CPH1831", "OPPO CPH1833" });

    public readonly string displayName;
    public readonly string[] models;
    public readonly Rect safeArea;
    public readonly int deltaY;

    public static DeviceConf device
    {
        get
        {
            return (DeviceConf)Device.model;
        }
    }

    public static List<DeviceConf> devices { get; private set; }

    private DeviceConf(string displayName, int vPadding, int deltaY, params string[] models)
    {
        this.models = models;
        this.displayName = displayName;
        this.safeArea = new Rect(0, vPadding, Device.width, Device.height -vPadding*2-deltaY);
        this.deltaY = deltaY;
        if (devices == null)
        {
            devices = new List<DeviceConf>();
        }
        devices.Add(this);
    }

    public override string ToString()
    {
        return displayName;
    }

    public override int GetHashCode()
    {
        return displayName.GetHashCode();
    }

    public override bool Equals(object obj)
    {
		return obj == this;
    }

    //public static implicit operator string(DeviceConf d)
    //{
    //    if (d == null)
    //    {
    //        return null;
    //    }
    //    return d.ToString();
    //}

    public static implicit operator DeviceConf(string s)
    {
        if (s == null)
        {
            return null;
        }
        s = s.Trim();
        foreach (var d in devices)
        {
			foreach (var m in d.models)
			{
				if (s.Equals(m, StringComparison.OrdinalIgnoreCase))
				{
					return d;
				}
			}
			if (s.Equals(d.displayName, StringComparison.OrdinalIgnoreCase))
			{
				return d;
			}
        }
        return null;
    }
}

