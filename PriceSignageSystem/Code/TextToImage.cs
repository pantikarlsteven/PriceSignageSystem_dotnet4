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
        public bool IsBiggerFont { get; set; } = true;
        public int OneEightDescTotalLines { get; set; }
        //Note: This code will generate an image from text and will verify the width of an
        //image if the brand name and description are single line or double line depending on the report size.
        //Whole: Brand Single Line Max Width=64.63975, Description Single Line Max Width=63.692482
        //1/8: Brand Single Line Max Width=24.58957, Description Single Line Max Width=21.57397
        //Skinny: Brand Single Line Max Width=50.2095566, Description Single Line Max Width=41.0242958
        //jewelry: Brand Single Line Max Width=20.0512123, Description Single Line Max Width=17.0183887
        public void GetImageWidth(string brand, string desc, int sizeId)
        {

            var bnTotalLines = GetBrandAndDescriptionTotalLines(brand.Split(' '));
            var dnTotalLines = GetBrandAndDescriptionTotalLines(desc.Split(' '));
            OneEightDescTotalLines = GetDescriptionTotalLinesOneEight(desc.Split(' '));

            if ((bnTotalLines + dnTotalLines) > 4)
                IsBiggerFont = false;

            //Default is Whole
            //sizeId = 1;
            Font brandFont = new Font("Arial", (float)4.688);
            Font descFont = new Font("Arial", (float)4.688);
            var brandMaxWidth = 57.88784;
            var descMaxWidth = 57.88784;
            switch (sizeId)
            {
                case ReportConstants.Size.OneEight:
                    brandFont = new Font("Calibri", (float)1.625, FontStyle.Bold);
                    descFont = new Font("Calibri", (float)1.625, FontStyle.Bold);
                    brandMaxWidth = 18.5982762;
                    descMaxWidth = 18.5982762;
                    break;
                case ReportConstants.Size.Jewelry:
                    brandFont = new Font("Calibri", (float)1.125, FontStyle.Bold);
                    descFont = new Font("Calibri", (float)1.125, FontStyle.Bold);
                    brandMaxWidth = 15.1740694;
                    descMaxWidth = 14.2768526;
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

        public int GetBrandAndDescriptionTotalLines(string[] words)
        {
            var totalLines = 0;
            var lines = "";

            // Iterate through the words and print them
            foreach (string word in words)
            {

                if (lines.Length > 0)
                    lines += " " + word;
                else
                    lines = word;

                var brandMaxWidth = 57.88784;
                Font brandFont = new Font("Arial", (float)4.688);
                Image fakeImage = new Bitmap(1, 1);
                Graphics graphics = Graphics.FromImage(fakeImage);
                SizeF brandSize = graphics.MeasureString(lines, brandFont);
                var bsWidth = float.Parse(brandSize.Width.ToString("F5"));
                var bmWidth = float.Parse(brandMaxWidth.ToString("F5"));

                if (bsWidth > bmWidth)
                {
                    totalLines++;
                    lines = word;
                }

            }

            if (lines.Length > 0)
                totalLines++;

            return totalLines;
        }

        public int GetDescriptionTotalLinesOneEight(string[] words)
        {
            var totalLines = 0;
            var lines = "";

            // Iterate through the words and print them
            foreach (string word in words)
            {

                if (lines.Length > 0)
                    lines += " " + word;
                else
                    lines = word;

                var brandMaxWidth = 18.5982762;
                Font brandFont = new Font("Calibri", (float)1.625);
                Image fakeImage = new Bitmap(1, 1);
                Graphics graphics = Graphics.FromImage(fakeImage);
                SizeF brandSize = graphics.MeasureString(lines, brandFont);
                var bsWidth = float.Parse(brandSize.Width.ToString("F5"));
                var bmWidth = float.Parse(brandMaxWidth.ToString("F5"));

                if (bsWidth > bmWidth)
                {
                    totalLines++;
                    lines = word;
                }

            }

            if (lines.Length > 0)
                totalLines++;

            return totalLines;
        }
    }
}