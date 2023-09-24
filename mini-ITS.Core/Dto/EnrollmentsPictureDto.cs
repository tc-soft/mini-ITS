using System;

namespace mini_ITS.Core.Dto
{
    public class EnrollmentsPictureDto
    {
        public Guid Id { get; set; }
        public Guid EnrollmentId { get; set; }
        public DateTime DateAddPicture { get; set; }
        public DateTime DateModPicture { get; set; }

        public Guid UserAddPicture { get; set; }
        public string UserAddPictureFullName { get; set; }
        public Guid UserModPicture { get; set; }
        public string UserModPictureFullName { get; set; }

        public string PictureName { get; set; }
        public string PicturePath { get; set; }
        public string PictureFullPath { get; set; }
    }
}