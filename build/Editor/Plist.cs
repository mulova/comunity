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

namespace build
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using UnityEngine;

    internal class Plist
    {
        private const string LSApplicationQueriesSchemesKey = "LSApplicationQueriesSchemes";
        private const string NSAppTransportSecurityKey = "NSAppTransportSecurity";
        private const string NSExceptionDomainsKey = "NSExceptionDomains";
        private const string NSIncludesSubdomainsKey = "NSIncludesSubdomains";
        private const string NSExceptionRequiresForwardSecrecyKey = "NSExceptionRequiresForwardSecrecy";
        private const string CFBundleURLTypesKey = "CFBundleURLTypes";
        private const string CFBundleURLSchemesKey = "CFBundleURLSchemes";
        private const string CFBundleURLName = "CFBundleURLName";
        private const string FacebookCFBundleURLName = "facebook-unity-sdk";
        private const string FacebookAppIDKey = "FacebookAppID";

        private static readonly IList<object> FacebookLSApplicationQueriesSchemes = new List<object>()
        {
            "fbapi",
            "fb-messenger-api",
            "fbauth2",
            "fbshareextension"
        };

        private static readonly PlistDict FacebookNSExceptionDomainsEntry = new PlistDict()
        {
            { NSIncludesSubdomainsKey, true },
            { NSExceptionRequiresForwardSecrecyKey, false }
        };

        private static readonly PlistDict FacebookNSExceptionDomainsKey = new PlistDict()
        {
            { "facebook.com", new PlistDict(FacebookNSExceptionDomainsEntry) },
            { "fbcdn.net", new PlistDict(FacebookNSExceptionDomainsEntry) },
            { "akamaihd.net", new PlistDict(FacebookNSExceptionDomainsEntry) },
        };

        private static readonly PlistDict FacebookNSAppTransportSecurity = new PlistDict()
        {
            { Plist.NSExceptionDomainsKey, FacebookNSExceptionDomainsKey }
        };

        private static readonly PlistDict FacebookUrlSchemes = new PlistDict()
        {
            { Plist.CFBundleURLName, Plist.FacebookCFBundleURLName },
        };

        private string filePath;

        public Plist(string fullPath)
        {
            this.filePath = fullPath;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;
            XmlReader plistReader = XmlReader.Create(this.filePath, settings);

            XDocument doc = XDocument.Load(plistReader);
            XElement plist = doc.Element("plist");
            XElement dict = plist.Element("dict");
            this.XMLDict = new PlistDict(dict);
            plistReader.Close();
        }

        public PlistDict XMLDict { get; set; }

        public void UpdateFBSettings(string appID, string urlSuffix, ICollection<string> appLinkSchemes)
        {
            // Set the facbook app ID
            this.XMLDict[Plist.FacebookAppIDKey] = appID;

            // Set the requried schemas for this app
            SetCFBundleURLSchemes(this.XMLDict, appID, urlSuffix, appLinkSchemes);

            // iOS 9+ Support
            WhilelistFacebookServersForNetworkRequests(this.XMLDict);
            WhitelistFacebookApps(this.XMLDict);
        }

        public void WriteToFile()
        {
            // Corrected header of the plist
            string publicId = "-//Apple//DTD PLIST 1.0//EN";
            string stringId = "http://www.apple.com/DTDs/PropertyList-1.0.dtd";
            string internalSubset = null;
            XDeclaration declaration = new XDeclaration("1.0", Encoding.UTF8.EncodingName, null);
            XDocumentType docType = new XDocumentType("plist", publicId, stringId, internalSubset);

            this.XMLDict.Save(this.filePath, declaration, docType);
        }

        private static void WhilelistFacebookServersForNetworkRequests(PlistDict plistDict)
        {
            if (!ContainsKeyWithValueType(plistDict, Plist.NSAppTransportSecurityKey, typeof(PlistDict)))
            {
                // We don't have a NSAppTransportSecurity entry. We can easily add one
                plistDict[Plist.NSAppTransportSecurityKey] = Plist.FacebookNSAppTransportSecurity;
                return;
            }

            var appTransportSecurityDict = (PlistDict)plistDict[Plist.NSAppTransportSecurityKey];
            if (!ContainsKeyWithValueType(appTransportSecurityDict, Plist.NSExceptionDomainsKey, typeof(PlistDict)))
            {
                appTransportSecurityDict[Plist.NSExceptionDomainsKey] = Plist.FacebookNSExceptionDomainsEntry;
                return;
            }

            var exceptionDomains = (PlistDict)appTransportSecurityDict[Plist.NSExceptionDomainsKey];
            foreach (var key in Plist.FacebookNSExceptionDomainsEntry.Keys)
            {
                // Instead of just updating overwrite values to keep things up to date
                exceptionDomains[key] = FacebookNSExceptionDomainsEntry[key];
            }
        }

        private static void WhitelistFacebookApps(PlistDict plistDict)
        {
            if (!ContainsKeyWithValueType(plistDict, Plist.LSApplicationQueriesSchemesKey, typeof(IList<object>)))
            {
                // We don't have a LSApplicationQueriesSchemes entry. We can easily add one
                plistDict[Plist.LSApplicationQueriesSchemesKey] = Plist.FacebookLSApplicationQueriesSchemes;
                return;
            }

            var applicationQueriesSchemes = (IList<object>)plistDict[Plist.LSApplicationQueriesSchemesKey];
            foreach (var scheme in Plist.FacebookLSApplicationQueriesSchemes)
            {
                if (!applicationQueriesSchemes.Contains(scheme))
                {
                    applicationQueriesSchemes.Add(scheme);
                }
            }
        }

        private static void SetCFBundleURLSchemes(
            PlistDict plistDict,
            string appID,
            string urlSuffix,
            ICollection<string> appLinkSchemes)
        {
            IList<object> currentSchemas;
            if (ContainsKeyWithValueType(plistDict, Plist.CFBundleURLTypesKey, typeof(IList<object>)))
            {
                currentSchemas = (IList<object>)plistDict[Plist.CFBundleURLTypesKey];
            }
            else
            {
                // Didn't find any CFBundleURLTypes, let's create one
                currentSchemas = new List<object>();
                plistDict[Plist.CFBundleURLTypesKey] = currentSchemas;
            }

            PlistDict facebookBundleUrlSchemes = Plist.GetFacebookUrlSchemes(currentSchemas);

            // Clear and set the CFBundleURLSchemes for the facebook schemes
            var facebookUrlSchemes = new List<object>();
            facebookBundleUrlSchemes[Plist.CFBundleURLSchemesKey] = facebookUrlSchemes;
            AddAppID(facebookUrlSchemes, appID, urlSuffix);
            AddAppLinkSchemes(facebookUrlSchemes, appLinkSchemes);
        }

        private static PlistDict GetFacebookUrlSchemes(ICollection<object> plistSchemes)
        {
            foreach (var plistScheme in plistSchemes)
            {
                PlistDict bundleTypeNode = plistScheme as PlistDict;
                if (bundleTypeNode != null)
                {
                    // Check to see if the url scheme name is facebook
                    object bundleURLName;
                    if (bundleTypeNode.TryGetValue(Plist.CFBundleURLName, out bundleURLName) &&
                        bundleURLName.Equals(Plist.FacebookCFBundleURLName))
                    {
                        return bundleTypeNode;
                    }
                }
            }

            // We didn't find a facebook scheme so lets create one
            PlistDict facebookUrlSchemes = new PlistDict(Plist.FacebookUrlSchemes);
            plistSchemes.Add(facebookUrlSchemes);
            return facebookUrlSchemes;
        }

        private static void AddAppID(ICollection<object> schemesCollection, string appID, string urlSuffix)
        {
            string modifiedID = appID;
            if (!string.IsNullOrEmpty(urlSuffix))
            {
                modifiedID += urlSuffix;
            }

            schemesCollection.Add((object)modifiedID);
        }

        private static void AddAppLinkSchemes(ICollection<object> schemesCollection, ICollection<string> appLinkSchemes)
        {
            foreach (var appLinkScheme in appLinkSchemes)
            {
                schemesCollection.Add(appLinkScheme);
            }
        }

        private static bool ContainsKeyWithValueType(IDictionary<string, object> dictionary, string key, Type type)
        {
            if (dictionary.ContainsKey(key) &&
                type.IsAssignableFrom(dictionary[key].GetType()))
            {
                return true;
            }

            return false;
        }
    }
}
