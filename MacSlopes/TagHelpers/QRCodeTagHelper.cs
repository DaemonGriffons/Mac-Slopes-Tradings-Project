using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;

namespace MacSlopes.TagHelpers
{
    [HtmlTargetElement("qrcode")]
    public class QRCodeTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = context.AllAttributes["content"].ToString();
            //var width = int.Parse(context.AllAttributes["width"].ToString());
            //var height = int.Parse(context.AllAttributes["height"].ToString());
            var width = 500;
            var height = 300;

            var codeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = 0
                }
            };

            var pixeldata = codeWriter.Write(content);
            using (var bitmap=new Bitmap(pixeldata.Width,pixeldata.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var memorystream=new MemoryStream())
                {
                    var bitmapData = bitmap.LockBits(
                        new Rectangle(0, 0, pixeldata.Width, pixeldata.Height),
                        System.Drawing.Imaging.ImageLockMode.WriteOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    try
                    {
                        System.Runtime.InteropServices.Marshal.Copy(pixeldata.Pixels, 0, bitmapData.Scan0, pixeldata.Pixels.Length);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    bitmap.Save(memorystream, System.Drawing.Imaging.ImageFormat.Png);
                    output.TagName = "img";
                    output.Attributes.Clear();
                    output.Attributes.Add("width", width);
                    output.Attributes.Add("height", height);
                    output.Attributes.Add("src", String.Format("data:image/png;base64,{0}", Convert.ToBase64String(memorystream.ToArray())));
                }
            }
        }
    }
}
