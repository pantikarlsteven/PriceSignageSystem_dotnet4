using PriceSignageSystem.Models.Constants;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Code
{
    public class TextToImage
    {
        public bool IsSLBrand { get; set; }
        public bool IsSLDescription { get; set; }
        //Note: This code will generate an image from text and will verify the width of an
        //image if the brand name and description are single line or double line depending on the report size.
        //Whole: Brand Single Line Max Width=64.63975, Description Single Line Max Width=63.692482
        //1/8: Brand Single Line Max Width=24.58957, Description Single Line Max Width=21.57397
        //Skinny: Brand Single Line Max Width=50.2095566, Description Single Line Max Width=41.0242958
        //jewelry: Brand Single Line Max Width=20.0512123, Description Single Line Max Width=17.0183887
        public void GetImageWidth(string brand, string desc, int sizeId)
        {
            //Default is Whole
            //sizeId = 1;
            Font brandFont = new Font("Arial Black", (float)4.375);
            Font descFont = new Font("Impact", (float)1.875);
            var brandMaxWidth = 52.6784821;
            var descMaxWidth = 39.09749;
            switch (sizeId)
            {
                case ReportConstants.Size.OneEight:
                    brandFont = new Font("Arial Black", (float)1.5625);
                    descFont = new Font("Impact", (float)1.125);
                    brandMaxWidth = 20.2399311;
                    descMaxWidth = 19.7897911;
                    break;
                case ReportConstants.Size.Jewelry:
                    brandFont = new Font("Arial Black", 1);
                    descFont = new Font("Impact", (float)0.75);
                    brandMaxWidth = 15.1657963;
                    descMaxWidth = 13.9070625;
                    break;
                case ReportConstants.Size.Skinny:
                    brandFont = new Font("Arial Black", (float)2.188);
                    descFont = new Font("Impact", (float)1.25);
                    brandMaxWidth = 50.2095566;
                    descMaxWidth = 41.0242958;
                    break;
            }

            Image fakeImage = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(fakeImage);
            SizeF brandSize = graphics.MeasureString(brand, brandFont);
            SizeF descSize = graphics.MeasureString(desc, descFont);

            if (brandSize.Width <= brandMaxWidth)
                IsSLBrand = true;

            if (descSize.Width <= descMaxWidth)
                IsSLDescription = true;
        }
    }
}