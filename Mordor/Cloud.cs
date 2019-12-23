using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mordor.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Mordor
{
    public class CloudService
    {
        /// <summary>Connects to Cloudinary .</summary>
        /// <returns>Cloudinary instance</returns>
        public Cloudinary Connect()
        {
            Account account = new Account("dcvn1l2nx", "381332897178687", "Dqr3nC22qVxgxnhXjeJCqQStkkI");
            Cloudinary cloudinary = new Cloudinary(account);
            return cloudinary;
        }

        /// <summary>Uploads the specified file.</summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public async Task<ImageUploadResult> Upload(IFormFile file)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream())
            };
            Cloudinary cloudinary = Connect();
            Task<ImageUploadResult> imageUploadTask = cloudinary.UploadAsync(uploadParams);
            return await imageUploadTask; ;
        }
    }
}
