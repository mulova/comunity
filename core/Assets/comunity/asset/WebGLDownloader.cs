using System;
using System.Net;
using System.ComponentModel;
using UnityEngine;
using System.Collections;


namespace comunity
{
    public class WebGLDownloader : Script
	{
		public bool caching = true;
		public delegate void DownloadProgressHandler(WWW www, float progress);
		public delegate void DownloadCompleteHandler(WWW www, object userState);
		public const int ASSET_VERSION = 1;
		private bool busy;
		private float progress;
		private WWW www;
		public event DownloadCompleteHandler DownloadFileCompleted;
		public event DownloadProgressHandler DownloadProgressChanged;

		public bool IsBusy
		{
			get { return www != null && !www.isDone; }
		}

		void Update()
		{
			if (DownloadProgressChanged != null && IsBusy)
			{
				DownloadProgressChanged(www, www.progress);
			}
		}

		public void DownloadFile(string src, object userState) {
			StartCoroutine(DownloadWeb(src, userState));
		}

		private IEnumerator DownloadWeb(string src, object userState)
		{
			AssetCache c = caching? Web.cache: Web.noCache;
			www = (c.GetImpl() as WebAssetLoader).CreateWWW(src);
			yield return www;
			WWW backup = www;
			if (DownloadFileCompleted != null)
			{
				DownloadFileCompleted(www, userState);
			}
			if (backup == www)
			{
				Dispose();
			} else
			{
				backup.DisposeEx();
			}
		}

		public void Dispose()
		{
			if (www != null)
			{
				www.DisposeEx();
				www = null;
			}
		}
	}
}
