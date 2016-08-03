using System;

namespace BestBuyProductDataToCSV
{
    /// <summary>
    /// Holds the fields used by our website for an indiviual product.
    /// All properties are strings just for ease of use.
    /// </summary>
    public class ProductData
    {
        // Data for WingTipToys product db

        public string ProductId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImagePath { get; set; } = "";
        public string Price { get; set; } = "";
        public string Category { get; set; } = "";

        // Additional data for category db
        public string CategoryId { get; set; } = "";

        /// <summary>
        /// Checks to see if this product has all the data we need.
        /// True if ProductId, Name, ImagePath, Price, Category, CategoryId
        /// all have values.
        /// </summary>
        /// <returns>True if all data exists.</returns>
        public bool HasAllImportantData()
        {
            int id;
            var parsed = int.TryParse(ProductId, out id);
            var hasData = ProductId.Length > 0
                          && parsed // Prevent overflow error when uploading.
                          && Name.Length > 0
                          && ImagePath.Length > 0
                          && Price.Length > 0
                          && Category.Length > 1
                          && CategoryId.Length > 0;
            return hasData;
        }
    }
}
