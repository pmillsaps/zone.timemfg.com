﻿using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.MediaLibrary.Services;
using Orchard.MediaLibrary.ViewModels;
using Orchard.Themes;
using Orchard.UI.Admin;

namespace Orchard.MediaLibrary.Controllers {
    [Admin, Themed(false)]
    public class WebSearchController : Controller {
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IContentManager _contentManager;

        public WebSearchController(IMediaLibraryService mediaManagerService, IContentManager contentManager) {
            _mediaLibraryService = mediaManagerService;
            _contentManager = contentManager;
        }

        public ActionResult Index(string folderPath, string type) {
            var viewModel = new ImportMediaViewModel {
                FolderPath = folderPath,
                Type = type
            };

            return View(viewModel);
        }


        [HttpPost]
        public JsonResult ImagePost(string folderPath, string type, string url) {

            try {
                var buffer = new WebClient().DownloadData(url);
                var stream = new MemoryStream(buffer);
                
                var mediaPart = _mediaLibraryService.ImportMedia(stream, folderPath, Path.GetFileName(url), type);
                _contentManager.Create(mediaPart);

                return new JsonResult {
                    Data = new {folderPath, MediaPath = mediaPart.FileName}
                };
            }
            catch(Exception e) {
                return new JsonResult {
                    Data = new { error= e.Message }
                };
            }
            
        }
    }
}