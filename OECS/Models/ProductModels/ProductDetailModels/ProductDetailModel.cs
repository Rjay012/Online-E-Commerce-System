using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace OECS.Models.ProductModels.ProductDetailModels
{
    public class ProductDetailModel
    {
        public int ProductDetailID { get; set; }
        public int ProductImageID { get; set; }
        public int[] ImageID { get; set; }
        public int[] FileToRemove { get; set; }
        public int IconID { get; set; }
        public int ProductID { get; set; }
        public int ColorID { get; set; }
        public int NewColorID { get; set; }  //holds the new color id when updating
        public int SID { get; set; }  //single valued size id
        public int[] SizeID { get; set; }
        public string[] NewSizeQuantity { get; set; }  //handles sizeID and its quantity
        public int[] ToRemoveSizeID { get; set; }  //holds the sizeID selected by the user to remove
        public bool? IsDisplay { get; set; }
        public bool ToDisplay { get; set; }
        public int IsDisplayPosition { get; set; }
        public string Path { get; set; }
        public string IconPath { get; set; }
        public HttpPostedFileBase IconFile { get; set; }
        public HttpPostedFileBase[] Files { get; set; }

        public IEnumerable<SelectListItem> ColorList { get; set; }

        public IEnumerable<SelectListItem> SizeList { get; set; }

        public List<ProductImage> ProductImage { get; set; }
    }
}