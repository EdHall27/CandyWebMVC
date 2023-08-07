namespace CandyWebMVC.Helper
{
    public class ImageHelper
    {
        public string SaveImageAndGetPath(IFormFile imageFile)
        {
            // Implement the logic to save the image to the desired location on the server
            // For example, you can use the "wwwroot/images" folder
            // Return the relative path to the image, which will be stored in the database

            // Example:
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine("wwwroot/uploads", fileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            return "/uploads/" + fileName; // Return the relative path
        }

        public void DeleteImageFile(string imagePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
} 
