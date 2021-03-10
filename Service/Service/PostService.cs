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
        Post Add(PostRequestModel newItem, string phoneNumber);
        IEnumerable<PostResponseModel> GetAllPost();
        PostResponseModel Get(string Id);
        PostRequestModel Update(PostRequestModel newItem, string phoneNumber, string savePath, string domain);
        void Delete(string Id);
    }
    public class PostService : IPostService
    {
        private IPostRepository _postRepository;
        private IAccountRepository _accountRepository;
        private IVegetableService _vegetableService;
        public PostService(IPostRepository postRepository, IAccountRepository accountRepository, IVegetableService vegetableService)
        {
            _postRepository = postRepository;
            _accountRepository = accountRepository;
            _vegetableService = vegetableService;
        }
        public Post Add(PostRequestModel newItem, string phoneNumber)
        {
            string accountId = this._accountRepository.GetSingle(s => s.PhoneNumber == phoneNumber).Id;
            var post = this._postRepository.Add(new Post
            {
                PostContent = newItem.Content,
                CreatedDate = DateTime.Now,
                AccountId = accountId,
                Status = newItem.Status,
                GardenId = newItem.GardenId,
                NoVeg = newItem.NoVeg
            });
            this._postRepository.Commit();
            return post;
        }

        public void Delete(string Id)
        {
            var post = this._postRepository.GetSingle(s => s.Id == Id);
            this._postRepository.Delete(post);
            this._postRepository.Commit();
        }

        public PostResponseModel Get(string Id)
        {
            var result = this._postRepository.GetSingle(s => s.Id == Id);
            var veg = this._vegetableService.Get(result.NoVeg, result.GardenId);
            return new PostResponseModel
            {
                Id = result.Id,
                CreatedDate = result.CreatedDate,
                AccountId = result.AccountId,
                VegName = veg.Name,
                PostContent = result.PostContent,
                VegDescription = veg.Description,
                VegFeature = veg.Feature,
                Images = veg.Images
            };
        }

        public IEnumerable<PostResponseModel> GetAllPost()
        {
            List<PostResponseModel> postResponseModels = new List<PostResponseModel>();
            var result = this._postRepository.GetAll();
            foreach (var item in result.ToList())
            {

                var veg = this._vegetableService.Get(item.NoVeg, item.GardenId);
                postResponseModels.Add(new PostResponseModel 
                {
                    Id = item.Id,
                    CreatedDate = item.CreatedDate,
                    AccountId = item.AccountId,
                    VegName = veg.Name,
                    PostContent = item.PostContent,
                    VegDescription = veg.Description,
                    VegFeature = veg.Feature,
                    Images = veg.Images
                });
            }
            return postResponseModels;
        }

        public PostRequestModel Update(PostRequestModel newItem, string phoneNumber, string savePath, string domain)
        {
            var result = this._postRepository.GetSingle(s => s.Id == newItem.Id);
            result.PostContent = newItem.Content;
            result.GardenId = newItem.GardenId;
            result.NoVeg = newItem.NoVeg;
            this._postRepository.Update(result);
            this._postRepository.Commit();
            return newItem;
        }
    }
}
