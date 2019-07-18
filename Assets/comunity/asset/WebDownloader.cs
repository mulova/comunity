using System.IO;
using System.Net;
using System;
using commons;
using System.Text.Ex;

namespace comunity
{
    public class WebDownloader : WebClient {
        
        public int? Timeout { get; set; }
        public int? ConnectionLimit { get; set; }
        public bool? KeepAlive { get; set; }
        public string reqMethod;
        
        public static void UseSecureHttp() {
            ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
        }
        //  ServicePointManager.UseNagleAlgorithm = true;
        //  ServicePointManager.Expect100Continue = true;
        //  ServicePointManager.CheckCertificateRevocationList = true;
        //  ServicePointManager.DefaultConnectionLimit = 6;
        
        protected override WebRequest GetWebRequest(Uri address) {
            WebRequest req = base.GetWebRequest(address);
            if (reqMethod.IsNotEmpty()) {
                req.Method = reqMethod;
            }
            if (req is HttpWebRequest) {
                HttpWebRequest httpReq = req as HttpWebRequest;
                //          httpReq.AllowAutoRedirect = true;
                if (KeepAlive.HasValue) {
                    httpReq.KeepAlive = KeepAlive.Value;
                }
                if (ConnectionLimit.HasValue) {
                    httpReq.ServicePoint.ConnectionLimit = ConnectionLimit.Value;
                }
            } else if (req is FtpWebRequest) {
                ((FtpWebRequest)req).UsePassive = false;
            }
            if (Timeout.HasValue) {
                req.Timeout = Timeout.Value;
            }
            return req;
        }
        
        public void DownloadFileAsyncEx(Uri src, string dst, object userState) {
            string dir = PathUtil.GetDirectory(dst);
            if (!Directory.Exists (dir)) {
                Directory.CreateDirectory(dir);
            }
#if !UNITY_5_5_OR_NEWER
            Threading.InvokeLater (() => {
#endif
                PlatformMethods.inst.SetNoBackupFlag(dst);
#if !UNITY_5_5_OR_NEWER
            });
#endif
            DownloadFileAsync(src, dst, userState);
        }
    }
}

public enum DownloadStep {
	Null, Downloading, Unzip, Done, Canceled
}