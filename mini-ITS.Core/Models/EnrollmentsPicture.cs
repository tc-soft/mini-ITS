using System;

namespace mini_ITS.Core.Models
{
    public class EnrollmentsPicture
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

        public EnrollmentsPicture() { }

        public EnrollmentsPicture(
            Guid id,
            Guid enrollmentId,
            DateTime dateAddPicture,
            DateTime dateModPicture,
            Guid userAddPicture,
            string userAddPictureFullName,
            Guid userModPicture,
            string userModPictureFullName,
            string pictureName,
            string picturePath,
            string pictureFullPath)
        {
            Id = id;
            EnrollmentId = enrollmentId;
            DateAddPicture = dateAddPicture;
            DateModPicture = dateModPicture;
            UserAddPicture = userAddPicture;
            UserAddPictureFullName = userAddPictureFullName;
            UserModPicture = userModPicture;
            UserModPictureFullName = userModPictureFullName;
            PictureName = pictureName;
            PicturePath = picturePath;
            PictureFullPath = pictureFullPath;
        }
    }
}