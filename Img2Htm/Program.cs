// -----------------------------------------------------------------
// <copyright>Copyright (C) 2018, David Ruiz.</copyright>
// Licensed under the Apache License, Version 2.0.
// You may not use this file except in compliance with the License: 
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Software is distributed on an "AS IS", WITHOUT WARRANTIES 
// OR CONDITIONS OF ANY KIND, either express or implied.
// -----------------------------------------------------------------

namespace Img2Htm
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || !args.Any())
            {
                Console.WriteLine("Usage: " + AppDomain.CurrentDomain.FriendlyName + " <path>");
                return;
            }

            string dir = args[0];

            if (!Directory.Exists(dir))
            {
                Console.WriteLine("Error: Directory not found");
                return;
            }

            WriteImageTextStreams(dir);
        }

        private static void WriteImageTextStreams(string dir)
        {
            string[] filters = { "jpg", "jpeg", "png", "gif", "tiff", "bmp" };
            string[] files = filters
                .SelectMany(p => Directory.GetFiles(dir, "*." + p))
                .ToArray();

            if (!files.Any()) return;

            string target = Path.Combine(dir, "ImagesAsBase64.html");

            string images = string.Join(Environment.NewLine,files.Select(Img2Base64));
            File.WriteAllText(target, string.Format(HtmTemplate, images));
        }

        private static string Img2Base64(string file)
        {
            Bitmap image = new Bitmap(file);
            string type = Path.GetExtension(file)
                .TrimStart('.')
                .ToLower();

            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                byte[] imageBytes = ms.ToArray();
                ms.Close();

                return string.Format(
                    "<img src=\"data:image/{0};base64,{1}\" title=\"{2}\" />",
                    type,
                    Convert.ToBase64String(imageBytes), 
                    Path.GetFileName(file)
                );
            }
        }

        private const string HtmTemplate = @"<!DOCTYPE html><html><body>{0}</body></html>";
    }
}