﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Models.ProductModels
{
    public class ProductColorModel
    {
        public int ProductDetailID { get; set; }
        public int ProductImageID { get; set; }
        public int[] ImageID { get; set; }
        public int IconID { get; set; }
        public int ProductID { get; set; }
        public int ColorID { get; set; }
        public int SID { get; set; }  //single valued size id
        public int[] SizeID { get; set; }
        public string[] NewSizeQuantity { get; set; }  //handles sizeID and its quantity
        public int[] SizeQuantity { get; set; }
        public int IsDisplayPosition { get; set; }
        public bool? IsDisplay { get; set; }
        public bool ToDisplay { get; set; }
        public string Path { get; set; }
        public string IconPath { get; set; }
        public HttpPostedFileBase IconFile { get; set; }
        public HttpPostedFileBase[] Files { get; set; }

        public IEnumerable<SelectListItem> ColorList
        {
            get
            {
                oecsEntities dbContext = new oecsEntities();
                List<SelectListItem> ColorListTempStorage = new List<SelectListItem>();
                var color = dbContext.Color.ToList();
                foreach (var item in color)
                {
                    ColorListTempStorage.Add(new SelectListItem
                    {
                        Value = item.ColorID.ToString(),
                        Text = item.color1
                    });
                }
                return ColorListTempStorage;
            }
        }

        public IEnumerable<SelectListItem> SizeList
        {
            get
            {
                oecsEntities dbContext = new oecsEntities();
                List<SelectListItem> SizeListTempStorage = new List<SelectListItem>();
                var size = dbContext.Size.ToList();
                foreach (var item in size)
                {
                    SizeListTempStorage.Add(new SelectListItem
                    {
                        Value = item.SideID.ToString(),
                        Text = item.size1
                    });
                }
                return SizeListTempStorage;
            }
        }

        public List<ProductImage> ProductImage { get; set; }

        //public NewSizeQuantity[] NSizeQuantity { get; set; }
    }

    //public class NewSizeQuantity
    //{
    //    public int SizeID { get; set; }
    //    public int Quantity { get; set; }
    //}
}