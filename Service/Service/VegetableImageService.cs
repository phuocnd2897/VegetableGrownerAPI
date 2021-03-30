using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;

namespace VG.Service.Service
{
    public interface IVegetableImageService
    {
        VegetableImage UploadImage(IFormFile image, string vegDescriptionId, string savePath,string url);
        VegetableImage Update(IEnumerable<IFormFile> image, string vegDescriptionId, string savePath, string url);
        void Delete(string postId);
        IEnumerable<VegetableImage> Get(string vegDescriptionId);
    }
    public class VegetableImageService : IVegetableImageService
    {
        private IVegetableImageRepository _vegetableImageRepository;
        public VegetableImageService(IVegetableImageRepository vegetableImageRepository)
        {
            _vegetableImageRepository = vegetableImageRepository;
        }

        public void Delete(string vegDescriptionId)
        {
            var post = this._vegetableImageRepository.GetMulti(s => s.VegetableDescriptionId == vegDescriptionId);
            if (post.Count() > 0)
            {
                this._vegetableImageRepository.Delete(post);
            }
            this._vegetableImageRepository.Commit();
        }

        public IEnumerable<VegetableImage> Get(string vegDescriptionId)
        {
            return this._vegetableImageRepository.GetMulti(s => s.VegetableDescriptionId == vegDescriptionId);
        }

        public VegetableImage Update(IEnumerable<IFormFile> image, string vegDescriptionId, string savePath, string url)
        {
            VegetableImage result = null;
            var post = this._vegetableImageRepository.GetMulti(s => s.VegetableDescriptionId == vegDescriptionId);
            if (post.Count() > 0)
            {
                this._vegetableImageRepository.Delete(post);
            }
            this._vegetableImageRepository.Commit();
            foreach (var item in image)
            {
                result = UploadImage(item, vegDescriptionId, savePath, url);
            }
            return result;
        }

        public VegetableImage UploadImage(IFormFile image, string vegDescriptionId, string savePath, string url)
        {
            string imageName = new string(Path.GetFileNameWithoutExtension(image.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(image.FileName);
            var imagePath = Path.Combine(savePath, "wwwroot/Image", imageName);
            using (FileStream fileStream = new FileStream(imagePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
                fileStream.Flush();
            }
            var postImage = this._vegetableImageRepository.Add(new VegetableImage
            {
                Name = imageName,
                LocalUrl = imagePath,
                Url = url + "/Image//"+ imageName,
                Thumbnail = url + "/Image//" + imageName,
                VegetableDescriptionId = vegDescriptionId,
            });
            return postImage;
        }
        public static string FilePathToFileUrl(string filePath)
        {
            StringBuilder uri = new StringBuilder();
            foreach (char v in filePath)
            {
                if ((v >= 'a' && v <= 'z') || (v >= 'A' && v <= 'Z') || (v >= '0' && v <= '9') ||
                  v == '+' || v == '/' || v == ':' || v == '.' || v == '-' || v == '_' || v == '~' ||
                  v > '\xFF')
                {
                    uri.Append(v);
                }
                else if (v == Path.DirectorySeparatorChar || v == Path.AltDirectorySeparatorChar)
                {
                    uri.Append('/');
                }
                else
                {
                    uri.Append(String.Format("%{0:X2}", (int)v));
                }
            }
            if (uri.Length >= 2 && uri[0] == '/' && uri[1] == '/') // UNC path
                uri.Insert(0, "file:");
            else
                uri.Insert(0, "file:///");
            return uri.ToString();
        }
    }
}
