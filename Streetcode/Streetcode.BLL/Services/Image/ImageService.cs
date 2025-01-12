using AutoMapper;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Image;

namespace Streetcode.BLL.Services.Image
{
    public class ImageService : IImageService
    {
        private readonly IMapper _mapper;
        private readonly IBlobService _blobService;

        public ImageService(IMapper mapper, IBlobService blobService)
        {
            _mapper = mapper;
            _blobService = blobService;
        }

        public Domain.Entities.Media.Images.Image ConfigureImage(ImageFileBaseCreateDto imageDTO)
        {
            string hashBlobStorageName = _blobService.SaveFileInStorage(
                        imageDTO.BaseFormat,
                        imageDTO.Title,
                        imageDTO.Extension).Result;

            var image = _mapper.Map<Domain.Entities.Media.Images.Image>(imageDTO);

            image.BlobName = $"{hashBlobStorageName}.{imageDTO.Extension}";
            return image;
        }

        public string? ImageBase64(ImageDto imageDTO)
        {
            return _blobService.FindFileInStorageAsBase64(imageDTO.BlobName).Result;
        }

        public void DeleteImage(string blobName)
        {
            _blobService.DeleteFileInStorage(blobName);
        }
    }
}
