using Streetcode.BLL.DTO.Media.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.Interfaces.Image
{
    public interface IImageService
    {
        public DAL.Entities.Media.Images.Image ConfigureImage(ImageFileBaseCreateDTO imageDTO);
        public string? ImageBase64(ImageDTO imageDTO);
    }
}
