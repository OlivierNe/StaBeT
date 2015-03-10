using StageBeheersTool.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using System.IO;

namespace StageBeheersTool.Models.Services
{
    public class ImageService : IImageService
    {
        private readonly int maxSize = 512000;

        public string SaveImage(HttpPostedFileBase image, string currentImageUrl, string dest)
        {
            if (File.Exists(currentImageUrl))
            {
                File.Delete(currentImageUrl);
            }
            string filename = HttpContext.Current.User.Identity.GetUserId() + Path.GetExtension(image.FileName);
            string newImageUrl = dest + "/" +  filename;
            string absolutePath = Path.Combine(HttpContext.Current.Server.MapPath(dest), Path.GetFileName(filename));
            image.SaveAs(absolutePath);
            return newImageUrl;
        }

        public bool IsValidImage(HttpPostedFileBase image)
        {
            return image != null && image.ContentLength > 0 && image.ContentType.StartsWith("image/");
        }


        public bool HasValidSize(HttpPostedFileBase fotoFile)
        {
            return fotoFile.ContentLength <= maxSize;
        }

        public int MaxSize()
        {
            return maxSize;
        }
    }
}