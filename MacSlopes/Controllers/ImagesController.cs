﻿using MacSlopes.Models.PostsViewModel;
using MacSlopes.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using MacSlopes.Entities.Data;
using Microsoft.EntityFrameworkCore;

namespace MacSlopes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IFileManager _fileManager;
        private readonly DataContext _repository;

        public ImagesController(IFileManager fileManager, DataContext repository)
        {
            _fileManager = fileManager;
            _repository = repository;
        }

        [HttpGet("{image}")]
        public IActionResult StreamPhoto(string image)
        {
            var mime = image.Substring(image.LastIndexOf('.') + 1);
            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }

    }
}