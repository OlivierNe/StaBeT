using System.Web;

namespace StageBeheersTool.Models.Domain
{
    public interface IImageService
    {
        string SaveImage(HttpPostedFileBase image, string currentImageUrl, string dest);
        bool IsValidImage(HttpPostedFileBase image);
        bool HasValidSize(HttpPostedFileBase fotoFile);
        int MaxSize();
    }
}
