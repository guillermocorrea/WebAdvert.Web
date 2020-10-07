using System;
using System.IO;
using System.Threading.Tasks;
using AdvertApi.Models;
using AutoMapper;
// using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients;
// using WebAdvert.Web.Models.AdvertManagement;
// using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagement
    {
        public class AdvertManagementController : Controller
        {
            private readonly IFileUploader _fileUploader;
            private readonly IAdvertApiClient _advertApiClient;
            private readonly IMapper _mapper;

            public AdvertManagementController(IFileUploader fileUploader,
                IAdvertApiClient advertApiClient,
                IMapper mapper)
            {
                _fileUploader = fileUploader;
                _advertApiClient = advertApiClient;
                _mapper = mapper;
            }

            public IActionResult Create(CreateAdvertViewModel model)
            {
                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
            {
                if (ModelState.IsValid)
                {
                    var createAdvertDto = _mapper.Map<CreateAdvertDto>(model);

                    var apiCallResponse = await _advertApiClient.Create(createAdvertDto).ConfigureAwait(false);
                    var id = apiCallResponse.Id;

                    // bool isOkToConfirmAd = true;
                    string filePath = string.Empty;
                    if (imageFile != null)
                    {
                        var fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                        filePath = $"{id}/{fileName}";

                        try
                        {
                            using (var readStream = imageFile.OpenReadStream())
                            {
                                var result = await _fileUploader.UploadFileAsync(filePath, readStream)
                                    .ConfigureAwait(false);
                                if (!result)
                                    throw new Exception(
                                        "Could not upload the image to file repository. Please see the logs for details.");
                            }
                        }
                        catch (Exception e)
                        {
                            var confirmDto = new ConfirmAdvertDto()
                            {
                                Id = id,
                                FilePath = filePath,
                                Status = AdvertStatus.Pending
                            };
                            await _advertApiClient.Confirm(confirmDto).ConfigureAwait(false);
                            Console.WriteLine(e);
                        }
                    }

                    var confirmModel = new ConfirmAdvertDto()
                    {
                        Id = id,
                        FilePath = filePath,
                        Status = AdvertStatus.Active
                    };
                    await _advertApiClient.Confirm(confirmModel).ConfigureAwait(false);

                    return RedirectToAction("Index", "Home");
                }

                return View(model);

                /*var apiCallResponse = await _advertApiClient.CreateAsync(createAdvertModel).ConfigureAwait(false);
                 var id = apiCallResponse.Id;

                 bool isOkToConfirmAd = true;
                 string filePath = string.Empty;
                 if (imageFile != null)
                 {
                     var fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                     filePath = $"{id}/{fileName}";

                     try
                     {
                         using (var readStream = imageFile.OpenReadStream())
                         {
                             var result = await _fileUploader.UploadFileAsync(filePath, readStream)
                                 .ConfigureAwait(false);
                             if (!result)
                                 throw new Exception(
                                     "Could not upload the image to file repository. Please see the logs for details.");
                         }
                     }
                     catch (Exception e)
                     {
                         isOkToConfirmAd = false;
                         var confirmModel = new ConfirmAdvertRequest()
                         {
                             Id = id,
                             FilePath = filePath,
                             Status = AdvertStatus.Pending
                         };
                         await _advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
                         Console.WriteLine(e);
                     }


                 }

                 if (isOkToConfirmAd)
                 {
                     var confirmModel = new ConfirmAdvertRequest()
                     {
                         Id = id,
                         FilePath = filePath,
                         Status = AdvertStatus.Active
                     };
                     await _advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
                 }

                 return RedirectToAction("Index", "Home");
             }

             return View(model);*/
            }
        }
    }
}