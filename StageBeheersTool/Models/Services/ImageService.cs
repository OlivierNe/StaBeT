using System;
using StageBeheersTool.Models.Domain;
using System.Web;
using Microsoft.AspNet.Identity;
using System.IO;

namespace StageBeheersTool.Models.Services
{
    public class ImageService : IImageService
    {
        private const int _maxSize = 512000;

        public string SaveImage(HttpPostedFileBase image, string currentImageUrl, string dest)
        {
            if (File.Exists(currentImageUrl))
            {
                File.Delete(currentImageUrl);
            }
            string filename = HttpContext.Current.User.Identity.GetUserId() + Path.GetExtension(image.FileName);
            string newImageUrl = dest + "/" + filename;
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
            return fotoFile.ContentLength <= _maxSize;
        }

        public int MaxSize()
        {
            return _maxSize;
        }

        public Foto GetFoto(HttpPostedFileBase file, string naam = null)
        {
            if (file == null) return null;
            if (IsValidImage(file) == false)
            {
                throw new ApplicationException("Ongeldig bestandtype.");
            }
            if (HasValidSize(file) == false)
            {
                throw new ApplicationException(string.Format(Resources.ErrorOngeldigeAfbeeldingGrootte, (MaxSize() / 1024)));
            }
            Stream stream = file.InputStream;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            var foto = new Foto
            {
                ContentType = file.ContentType,
                FotoData = buffer,
                Naam = naam
            };
            return foto;
        }
    }
}