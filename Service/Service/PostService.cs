using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VG.Data.Repository;
using VG.Model.Model;
using VG.Model.RequestModel;
using VG.Model.ResponseModel;

namespace VG.Service.Service
{
    public interface IPostService
    {
        Post Add(PostRequestModel newItem, string phoneNumber, string savePath, string domain);
        IEnumerable<PostResponseModel> GetAllPost();
        PostRequestModel Update(PostRequestModel newItem, string phoneNumber, string savePath, string domain);
        void Delete(string Id);
    }
    public class PostService : IPostService
    {
        private IPostRepository _postRepository;
        private IAccountRepository _accountRepository;
        private IPostImageService _postImageService;
        public PostService(IPostRepository postRepository, IAccountRepository accountRepository, IPostImageService postImageService)
        {
            _postRepository = postRepository;
            _accountRepository = accountRepository;
            _postImageService = postImageService;
        }
        public Post Add(PostRequestModel newItem, string phoneNumber, string savePath, string domain)
        {
            string accountId = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber).Id;
            var post = this._postRepository.Add(new Post
            {
                Tittle = newItem.Tittle,
                PostContent = newItem.Content,
                Description = newItem.Description,
                Feature = newItem.Feature,
                CreatedDate = DateTime.Now,
                AccountId = accountId
            });
            foreach (IFormFile item in newItem.Images)
            {
                var image = this._postImageService.UploadImage(item, post.Id, savePath);
                post.PostImages.Add(image);
            }
            this._postRepository.Commit();
            return post;
        }

        public void Delete(string Id)
        {
            var post = this._postRepository.GetSingle(s => s.Id == Id, new string[] { "PostImages" });
            this._postRepository.Delete(post);
            this._postImageService.Delete(post.Id);
            this._postRepository.Commit();
        }

        public IEnumerable<PostResponseModel> GetAllPost()
        {
            var result = this._postRepository.GetAll(new string[] { "PostImages"}).Select(s => new PostResponseModel
            {
                Id = s.Id,
                Title = s.Tittle,
                PostContent = s.PostContent,
                Description = s.Description,
                Feature = s.Feature,
                CreatedDate = s.CreatedDate,
                AccountId  = s.AccountId,
                listImage = s.PostImages.Where(p => p.PostId == s.Id).Select(p => p.Url),
            });
            return result;
        }

        public PostRequestModel Update(PostRequestModel newItem, string phoneNumber, string savePath, string domain)
        {
            var result = this._postRepository.GetSingle(s => s.Id == newItem.Id);
            result.Tittle = newItem.Tittle;
            result.PostContent = newItem.Content;
            result.Description = newItem.Description;
            result.Feature = newItem.Feature;
            this._postImageService.Update(newItem.Images, newItem.Id, savePath);
            this._postRepository.Update(result);
            this._postRepository.Commit();
            return newItem;
        }
    }
}
