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
    public interface IPostImageService
    {
        PostImage UploadImage(IFormFile image, string postId, string savePath);
        PostImage Update(IEnumerable<IFormFile> image, string postId, string savePath);
        void Delete(string postId);
    }
    public class PostImageService : IPostImageService
    {
        private IPostImageRepository _postImageRepository;
        public PostImageService(IPostImageRepository postImageRepository)
        {
            _postImageRepository = postImageRepository;
        }

        public void Delete(string postId)
        {
            var post = this._postImageRepository.GetMulti(s => s.PostId == postId);
            if (post.Count() > 0)
            {
                this._postImageRepository.Delete(post);
            }
            this._postImageRepository.CommitTransaction();
        }

        public PostImage Update(IEnumerable<IFormFile> image, string postId, string savePath)
        {
            PostImage result = null;
            var post = this._postImageRepository.GetMulti(s => s.PostId == postId);
            if (post.Count() > 0)
            {
                this._postImageRepository.Delete(post);
            }
            this._postImageRepository.Commit();
            foreach (var item in image)
            {
                result = UploadImage(item, postId, savePath);
            }
            return result;
        }

        public PostImage UploadImage(IFormFile image, string postId, string savePath)
        {
            string imageName = new string(Path.GetFileNameWithoutExtension(image.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(image.FileName);
            var imagePath = Path.Combine(savePath, "wwwroot/Image", imageName);
            using (FileStream fileStream = new FileStream(imagePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
                fileStream.Flush();
            }
            var postImage = this._postImageRepository.Add(new PostImage
            {
                Name = imageName,
                LocalUrl = imagePath,
                Url = imagePath + "\\" + imageName,
                Thumbnail = imagePath + "\\" + imageName,
                PostId = postId,
            });
            this._postImageRepository.Commit();
            return postImage;
        }
    }
}
