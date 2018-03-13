/**
 * Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
 *
 * You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
 * copy, modify, and distribute this software in source code or binary form for use
 * in connection with the web services and APIs provided by Facebook.
 *
 * As with any software that integrates with the Facebook platform, your use of
 * this software is subject to the Facebook Developer Principles and Policies
 * [http://developers.facebook.com/policy/]. This copyright notice shall be
 * included in all copies or substantial portions of the software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace build
{
	public class AndroidManifest
	{
		private string inputFile;
		private string _ns;
		private XmlNode manNode;
		private XmlNode appNode;
		private XmlDocument _doc;

		public XmlDocument doc
		{
			get
			{
				if (_doc == null)
				{
					Read();
				}
				return _doc;
			}
		}

		public string packageName 
		{
			get
			{
				if (manNode.Attributes != null)
				{
					XmlAttribute p = manNode.Attributes["package"];
					if (p != null)
					{
						return p.Value;
					}
				}
				return null;
			}
			set
			{
				XmlAttribute p = manNode.Attributes["package"];
				p.Value = value;
			}
		}

		public string ns
		{
			get
			{
				if (_ns == null)
				{
					_ns = manNode.GetNamespaceOfPrefix("android");
					if (_ns == null)
					{
						if (appNode != null)
						{
							_ns = appNode.GetNamespaceOfPrefix("android");
						}
					}
				}
				return null;
			}
		}

		public AndroidManifest()
		{
			inputFile = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");

			// only copy over a fresh copy of the AndroidManifest if one does not exist
			if (!File.Exists(inputFile))
			{
				var refFile = Path.Combine(
					EditorApplication.applicationContentsPath,
					"PlaybackEngines/androidplayer/AndroidManifest.xml");
				if (!File.Exists(refFile))
				{
					// Unity moved this file. Try to get it at its new location
					refFile = Path.Combine(
						EditorApplication.applicationContentsPath,
						"PlaybackEngines/AndroidPlayer/Apk/AndroidManifest.xml");
				}

				File.Copy(refFile, inputFile);
			}
		}

		public AndroidManifest(string inputFile)
		{
			this.inputFile = inputFile;
		}

		public void Read()
		{
			if (_doc == null)
			{
				_doc = new XmlDocument();
				_doc.Load(inputFile);
				manNode = FindChildNode(doc, "manifest");
				if (manNode != null)
				{
					appNode = FindChildNode(manNode, "application");
				}
			}
		}

		public void Write()
		{
			//            if (ns == null)
			//            {
			//                Debug.LogError("Error parsing "+inputFile);
			//                return;
			//            }

			//            AddAppLinkingActivity(schemes);

			//            SetMeta(metaKey, metaValue);

			// Add the facebook content provider
			// <provider
			//   android:name="com.facebook.FacebookContentProvider"
			//   android:authorities="com.facebook.app.FacebookContentProvider<APPID>"
			//   android:exported="true" />
			//            XmlElement contentProviderElement = CreateContentProviderElement(doc, ns, appId);
			//            SetOrReplaceXmlElement(dict, contentProviderElement);

			// Save the document formatted
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "  ",
				NewLineChars = "\r\n",
				NewLineHandling = NewLineHandling.Replace
			};

			using (XmlWriter xmlWriter = XmlWriter.Create(inputFile, settings))
			{
				doc.Save(xmlWriter);
			}
		}

		public void SetMeta(string metaName, string metaValue)
		{
			// add the app id
			// <meta-data android:name="com.facebook.sdk.ApplicationId" android:value="\ fb<APPID>" />
			XmlElement appIdElement = doc.CreateElement("meta-data");
			appIdElement.SetAttribute("name", ns, metaName);
			appIdElement.SetAttribute("value", ns, metaValue);
			SetOrReplaceXmlElement(appNode, appIdElement);
		}

		private XmlAttribute FindAttribute(XmlNode node, string localName)
		{
			if (node.Attributes != null)
			{
				foreach (object a in node.Attributes)
				{
					XmlAttribute attr = a as XmlAttribute;
					if (attr.LocalName == localName)
					{
						return attr;
					}
				}
			}
			return null;
		}

		public XmlNode FindChildNode(XmlNode parent, string name)
		{
			XmlNode curr = parent.FirstChild;
			while (curr != null)
			{
				if (curr.Name.Equals(name))
				{
					return curr;
				}

				curr = curr.NextSibling;
			}

			return null;
		}

		public void SetOrReplaceXmlElement(
			XmlNode parent,
			XmlElement newElement)
		{
			string attrNameValue = newElement.GetAttribute("name");
			string elementType = newElement.Name;

			XmlElement existingElment;
			if (TryFindElementWithAndroidName(parent, attrNameValue, out existingElment, elementType))
			{
				parent.ReplaceChild(newElement, existingElment);
			} else
			{
				parent.AppendChild(newElement);
			}
		}

		private bool TryFindElementWithAndroidName(
			XmlNode parent,
			string attrNameValue,
			out XmlElement element,
			string elementType = "activity")
		{
			var curr = parent.FirstChild;
			while (curr != null)
			{
				var currXmlElement = curr as XmlElement;
				if (currXmlElement != null&&
					currXmlElement.Name == elementType&&
					currXmlElement.GetAttribute("name", ns) == attrNameValue)
				{
					element = currXmlElement;
					return true;
				}

				curr = curr.NextSibling;
			}

			element = null;
			return false;
		}

		public void AddSimpleActivity(string className, bool export = false)
		{
			XmlElement element = CreateActivityElement(doc, ns, className, export);
			SetOrReplaceXmlElement(appNode, element);
		}

		public XmlElement CreateOverlayActivity(string activityName)
		{
			// <activity android:name="activityName" android:configChanges="all|of|them" android:theme="@android:style/Theme.Translucent.NoTitleBar.Fullscreen">
			// </activity>
			XmlElement activity = CreateActivityElement(doc, ns, activityName);
			activity.SetAttribute("configChanges", ns, "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
			activity.SetAttribute("theme", ns, "@android:style/Theme.Translucent.NoTitleBar.Fullscreen");
			SetOrReplaceXmlElement(appNode, activity);
			return activity;
		}

		// Add the facebook activity
		// <activity
		//   android:name="com.facebook.FacebookActivity"
		//   android:configChanges="keyboard|keyboardHidden|screenLayout|screenSize|orientation"
		//   android:label="@string/app_name"
		//   android:theme="@android:style/Theme.Translucent.NoTitleBar" />
		//        private XmlElement CreateFacebookElement(XmlDocument doc, string ns)
		//        {
		//            // <activity android:name="com.facebook.unity.FBUnityGameRequestActivity" android:exported="true">
		//            // </activity>
		//            XmlElement activityElement = CreateActivityElement(doc, ns, FacebookActivityName);
		//            activityElement.SetAttribute("configChanges", ns, "keyboard|keyboardHidden|screenLayout|screenSize|orientation");
		//            activityElement.SetAttribute("label", ns, "@string/app_name");
		//            activityElement.SetAttribute("theme", ns, "@android:style/Theme.Translucent.NoTitleBar");
		//            return activityElement;
		//        }

		public XmlElement CreateContentProviderElement(string contentProviderName, string authFormat, string appId)
		{
			XmlElement provider = doc.CreateElement("provider");
			provider.SetAttribute("name", ns, contentProviderName);
			string authorities = string.Format(CultureInfo.InvariantCulture, authFormat, appId);
			provider.SetAttribute("authorities", ns, authorities);
			provider.SetAttribute("exported", ns, "true");
			SetOrReplaceXmlElement(appNode, provider);
			return provider;
		}

		private XmlElement CreateActivityElement(XmlDocument doc, string ns, string activityName, bool exported = false)
		{
			// <activity android:name="activityName" android:exported="true">
			// </activity>
			XmlElement activityElement = doc.CreateElement("activity");
			activityElement.SetAttribute("name", ns, activityName);
			if (exported)
			{
				activityElement.SetAttribute("exported", ns, "true");
			}

			return activityElement;
		}

		public void AddAppLinkingActivity(string activityName, List<string> schemes)
		{
			XmlElement element = CreateActivityElement(doc, ns, activityName, true);
			foreach (var scheme in schemes)
			{
				// We have to create an intent filter for each scheme since an intent filter
				// can have only one data element.
				XmlElement intentFilter = doc.CreateElement("intent-filter");

				var action = doc.CreateElement("action");
				action.SetAttribute("name", ns, "android.intent.action.VIEW");
				intentFilter.AppendChild(action);

				var category = doc.CreateElement("category");
				category.SetAttribute("name", ns, "android.intent.category.DEFAULT");
				intentFilter.AppendChild(category);

				XmlElement dataElement = doc.CreateElement("data");
				dataElement.SetAttribute("scheme", ns, scheme);
				intentFilter.AppendChild(dataElement);
				element.AppendChild(intentFilter);
			}

			SetOrReplaceXmlElement(appNode, element);
		}
	}
}
