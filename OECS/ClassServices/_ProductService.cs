using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.ClassServices
{
    public class _ProductService : _Product
    {
        public _ProductService(oecsEntities _dbContext)
        {
            dbContext = _dbContext;
        }

        public bool HasColor(int? productID, int? colorID)
        {
            return dbContext.ProductImage
                            .Where(pi => pi.ProductDetail.ProductID == productID && pi.ProductDetail.ColorID == colorID)
                            .Join(dbContext.DisplayColor, pi => pi.ProductImageID, dc => dc.ProductImageID, (pi, dc) => new
                            {
                                dc.ProductImageID
                            }).Any();
        }

        public int GetDisplayColorKey(int? productID, int? colorID)
        {
            return dbContext.DisplayColor
                            .Where(dc => dc.ProductImage.ProductDetail.ProductID == productID && dc.ProductImage.ProductDetail.ColorID == colorID && dc.isDisplay == true)
                            .Join(dbContext.ProductImage, dc => dc.ProductImageID, pi => pi.ProductImageID, (dc, pi) => new
                            {
                                dc.DisplayColorID
                            }).FirstOrDefault().DisplayColorID;
        }

        public bool HasSize(int? productID, int? sizeID)
        {
            return dbContext.ProductImage
                            .Where(pi => pi.ProductDetail.ProductID == productID && pi.ProductDetail.SizeID == sizeID)
                            .Join(dbContext.DisplaySize, pi => pi.ProductImageID, ds => ds.ProductImageID, (pi, ds) => new
                            {
                                ds.ProductImageID
                            })
                            .Any();
        }

        public int GetDisplaySizeKey(int? productID, int? sizeID)
        {
            return dbContext.DisplaySize
                            .Where(ds => ds.ProductImage.ProductDetail.ProductID == productID && ds.ProductImage.ProductDetail.SizeID == sizeID && ds.isDisplay == true)
                            .Select(ds => new
                            {
                                ds.DisplaySizeID
                            }).FirstOrDefault().DisplaySizeID;
        }

        public int GetDisplayImageKey(int? productDetailID)
        {
            return (int)dbContext.ProductImage
                                 .Where(pi => pi.ProductDetailID == productDetailID)
                                 .Join(dbContext.DisplaySize, pi => pi.ProductImageID, ds => ds.ProductImageID, (pi, ds) => new
                                 {
                                     pi.ImageID
                                 })
                                 .FirstOrDefault().ImageID;
        }

        public int GetProductImageID(int? productDetailID, int? imageID)
        {
            return dbContext.ProductImage
                            .Where(pi => pi.ProductDetailID == productDetailID && pi.ImageID == imageID)
                            .Select(pi => new 
                            { 
                                pi.ProductImageID 
                            })
                            .FirstOrDefault().ProductImageID;
        }
    }
}