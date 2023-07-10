﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ProjectRoomChat.Data;
using ProjectRoomChat.Helpers;
using ProjectRoomChat.Hubs;
using ProjectRoomChat.Models;
using ProjectRoomChat.ViewModels;
using System.Text.RegularExpressions;

namespace ProjectRoomChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly int FileSizeLimit;
        private readonly string[] AllowExtension;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IFileValidator _fileValidator;
        public UploadController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment environment, IHubContext<ChatHub> hubContext, IConfiguration configruation, IFileValidator fileValidator)
        {
            _context = context;
            _mapper = mapper;
            _env = environment;
            _hubContext = hubContext;
            _fileValidator = fileValidator;

            FileSizeLimit = configruation.GetSection("FileUpload").GetValue<int>("FileSizeLimit");
            AllowExtension = configruation.GetSection("FileUpload").GetValue<string>("AllowedExtensions").Split(",");
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (!_fileValidator.IsValid(viewModel.File))
                {
                    return BadRequest("Validation failed!");
                }

                var fileName = DateTime.Now.ToString("yyyymmddMMss") + "_" + Path.GetFileName(viewModel.File.FileName);
                var folderPath = Path.Combine(_env.WebRootPath, "uploads");
                var filePath = Path.Combine(folderPath, fileName);

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.File.CopyToAsync(fileStream);
                }

                var user = _context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
                var room = _context.Rooms.Where(x => x.Id == viewModel.RoomId).FirstOrDefault();
                if (user == null || room == null)
                {
                    return NotFound();
                }

                string htmlImage = string.Format(
                    "<a href=\"/uploads/{0}\" target=\"_blank\">" +
                    "<img src=\"/uploads/{0}\" class=\"post-image\">" +
                    "</a>", fileName);

                var message = new Message()
                {
                    Content = Regex.Replace(htmlImage, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
                    Timestamp = DateTime.Now,
                    FromUser = user,
                    ToRoom = room
                };

                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();

                var messageViewModel = _mapper.Map<Message, MessageViewModel>(message);
                await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", messageViewModel);

                return Ok();
            }

            return BadRequest();
        }
    
    }
}
