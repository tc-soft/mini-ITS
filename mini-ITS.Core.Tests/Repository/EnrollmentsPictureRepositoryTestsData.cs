using System;
using System.Collections.Generic;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    public class EnrollmentsPictureRepositoryTestsData
    {
        public static IEnumerable<EnrollmentsPicture> EnrollmentsPictureCases
        {
            get
            {
                yield return new EnrollmentsPicture
                {
                    Id = new Guid("DDEFC28A-0212-4783-95E5-0F8E5C7C8C62"),
                    EnrollmentId = new Guid("6AC164EC-C578-43EB-9B4A-0478C316DD03"),
                    DateAddPicture = new DateTime(2023, 9, 1, 0, 1, 0),
                    DateModPicture = new DateTime(2023, 9, 1, 0, 1, 0),
                    UserAddPicture = new Guid("6F929C62-3A50-4F29-AF99-F4D188754FC0"),
                    UserAddPictureFullName = "Eva Pedrov",
                    UserModPicture = new Guid("6F929C62-3A50-4F29-AF99-F4D188754FC0"),
                    UserModPictureFullName = "Eva Pedrov",
                    
                    PictureName = "20230901_000001.jpg",
                    PicturePath = "/Files/6ac164ec-c578-43eb-9b4a-0478c316dd03/8732dcba-ed68-4920-bee4-f1401e229d66.jpg",
                    PictureFullPath = "/app/wwwroot/Files/6ac164ec-c578-43eb-9b4a-0478c316dd03/8732dcba-ed68-4920-bee4-f1401e229d66.jpg"
                };
                yield return new EnrollmentsPicture
                {
                    Id = new Guid("93433E3A-E10A-4760-AD62-B5868B5091DA"),
                    EnrollmentId = new Guid("D11A5A6B-F1FA-49D6-B412-F4ADDD7F7BE5"),
                    DateAddPicture = new DateTime(2023, 9, 4, 0, 1, 0),
                    DateModPicture = new DateTime(2023, 9, 4, 0, 1, 0),
                    UserAddPicture = new Guid("DA30DCAC-27E8-4B53-B2C4-FE5220420106"),
                    UserAddPictureFullName = "Brigita Bartles",
                    UserModPicture = new Guid("DA30DCAC-27E8-4B53-B2C4-FE5220420106"),
                    UserModPictureFullName = "Brigita Bartles",

                    PictureName = "20230904_000001.jpg",
                    PicturePath = "/Files/d11a5a6b-f1fa-49d6-b412-f4addd7f7be5/90d8c666-e576-4b81-8510-282e32f9e0c8.jpg",
                    PictureFullPath = "/app/wwwroot/Files/d11a5a6b-f1fa-49d6-b412-f4addd7f7be5/90d8c666-e576-4b81-8510-282e32f9e0c8.jpg"
                };
                yield return new EnrollmentsPicture
                {
                    Id = new Guid("24BE85BE-12BC-422C-B92E-6BCAAFD1AF9F"),
                    EnrollmentId = new Guid("9F38B77C-3444-4E69-BF2E-C1A71E4B5B6E"),
                    DateAddPicture = new DateTime(2023, 9, 7, 0, 1, 0),
                    DateModPicture = new DateTime(2023, 9, 7, 0, 1, 0),
                    UserAddPicture = new Guid("DA38CA53-D59F-49FB-B5E8-4118A13AABD9"),
                    UserAddPictureFullName = "Louis Gilliam",
                    UserModPicture = new Guid("DA38CA53-D59F-49FB-B5E8-4118A13AABD9"),
                    UserModPictureFullName = "Louis Gilliam",

                    PictureName = "20230907_000001.jpg",
                    PicturePath = "/Files/9f38b77c-3444-4e69-bf2e-c1a71e4b5b6e/f6bd44fd-eaf7-4fbc-8b11-a5e4f7d33159.jpg",
                    PictureFullPath = "/app/wwwroot/Files/9f38b77c-3444-4e69-bf2e-c1a71e4b5b6e/f6bd44fd-eaf7-4fbc-8b11-a5e4f7d33159.jpg"
                };
                yield return new EnrollmentsPicture
                {
                    Id = new Guid("A7415989-635A-4E3D-80EF-9003771824BD"),
                    EnrollmentId = new Guid("9F38B77C-3444-4E69-BF2E-C1A71E4B5B6E"),
                    DateAddPicture = new DateTime(2023, 9, 7, 0, 4, 0),
                    DateModPicture = new DateTime(2023, 9, 7, 0, 4, 0),
                    UserAddPicture = new Guid("DA38CA53-D59F-49FB-B5E8-4118A13AABD9"),
                    UserAddPictureFullName = "Louis Gilliam",
                    UserModPicture = new Guid("DA38CA53-D59F-49FB-B5E8-4118A13AABD9"),
                    UserModPictureFullName = "Louis Gilliam",

                    PictureName = "20230907_000004.jpg",
                    PicturePath = "/Files/9f38b77c-3444-4e69-bf2e-c1a71e4b5b6e/1196bdc6-23c5-461e-b7ef-84de7a3b012d.jpg",
                    PictureFullPath = "/app/wwwroot/Files/9f38b77c-3444-4e69-bf2e-c1a71e4b5b6e/1196bdc6-23c5-461e-b7ef-84de7a3b012d.jpg"
                };
            }
        }
    }
}