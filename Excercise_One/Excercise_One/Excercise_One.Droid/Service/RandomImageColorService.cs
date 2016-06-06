using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Threading.Tasks;
using Exercise_One.Droid.Common;
using Android.Graphics;
using Android.Util;

namespace Exercise_One.Droid.Service
{
    #region <-Interface->
    public interface IRandomImageColorService
    {
        Task<Bitmap> GetRandomImagePattern(string url);
        string GetRandomColorHex(string url);
    }
    #endregion


    #region <-Implementation->
    public class RandomImageColorService : IRandomImageColorService
    {

        #region <-PublicMethods->
        public async Task<Bitmap> GetRandomImagePattern(string url)
        {
            try
            {
                Bitmap bitmap = null;

                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.ContentType = "application/xml";
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var data = streamReader.ReadToEnd();

                            if (String.IsNullOrWhiteSpace(data))
                            {
                                Log.Info(Constants.APP, "[GetRandomImagePattern] Empty data!!!");
                            }
                            else
                            {
                                var doc = XDocument.Parse(data);
                                var s = doc.Descendants(XName.Get("imageUrl")).FirstOrDefault();

                                if (s != null)
                                {
                                    bitmap = GetImageBitmapFromUrl(s.Value);
                                    System.Diagnostics.Debug.WriteLine(s.Value);
                                }
                            }
                        }
                    }
                }
                return bitmap;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public string GetRandomColorHex(string url)
        {
            try
            {
                var hextStr = string.Empty;
                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.ContentType = "application/xml";
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            var data = streamReader.ReadToEnd();

                            if (String.IsNullOrWhiteSpace(data))
                            {
                                Log.Info(Constants.APP, "[GetRandomColorHex] Empty data!!!");
                            }
                            else
                            {
                                var doc = XDocument.Parse(data);
                                var hex = doc.Descendants(XName.Get("hex")).FirstOrDefault();

                                if (hex != null)
                                {
                                    hextStr = hex.Value;
                                    System.Diagnostics.Debug.WriteLine(hextStr);
                                }
                            }
                        }
                    }
                }

                return hextStr;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }
            return imageBitmap;
        }
    }
    #endregion
}

/*CommentedCode*/
