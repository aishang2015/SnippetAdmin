using SkiaSharp;

namespace SnippetAdmin.Core.Helpers
{
    public static class CaptchaHelper
    {
        private static readonly SKColor[] Colors = new[]
        {
            SKColors.DeepSkyBlue,SKColors.DodgerBlue,SKColors.CornflowerBlue,SKColors.Aqua,SKColors.Aquamarine,SKColors.Cyan,SKColors.LightGreen,
            SKColors.SteelBlue,SKColors.DodgerBlue,SKColors.Black, SKColors.Red, SKColors.DarkBlue, SKColors.Green, SKColors.Orange, SKColors.Brown,
            SKColors.BlueViolet,SKColors.Turquoise,SKColors.CadetBlue,SKColors.Violet,SKColors.Tomato,SKColors.Tan,SKColors.MediumSeaGreen
        };

        private static readonly char[] Codes = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ".ToCharArray();

        /// <summary>
        /// 生成验证码图片
        /// </summary>
        public static (byte[], string) GenerateCaptcha(int length = 4)
        {
            var random = new Random(DateTime.Now.Millisecond);

            // 生成随机字符串
            var code = new string(Enumerable.Repeat(Codes, length).Select(s => s[random.Next(s.Length)]).ToArray());

            var width = length * 30;        //图像的宽度
            var height = 50;          //图像的高度
            var zaos = length * 50;        //噪点数

            var bitMap = new SKBitmap(width, height); //创建画布
            var canvas = new SKCanvas(bitMap);//创建画笔
            canvas.Clear(SKColors.Transparent);//透明背景

            // 噪点
            for (var i = 0; i < zaos; i++)
            {
                var x = random.Next(bitMap.Width); 
                var y = random.Next(bitMap.Height);
                var color = Colors[random.Next(Colors.Length)]; 
                bitMap.SetPixel(x, y, color);                    
            }

            // 文字
            var xPosition = 2f;
            for (var i = 0; i < code.Length; i++)
            {
                var charCode = code.Substring(i, 1);
                var fontColor = Colors[random.Next(Colors.Length)];

                using var paint = new SKPaint()
                {
                    TextEncoding = SKTextEncoding.Utf8,
                    Color = fontColor,
                    StrokeWidth = 1,
                    IsAntialias = true,
                    TextSize = 40,
                    FilterQuality = SKFilterQuality.High,

                };

                canvas.DrawText(charCode, new SKPoint(xPosition, 36), paint);

                var charWidth = paint.MeasureText(charCode);
                xPosition += (charWidth > 24) ? charWidth : 24;
                xPosition += 2;
            }
            var image = SKImage.FromBitmap(bitMap);
            return (image.Encode(SKEncodedImageFormat.Png, 100).ToArray(), code);
        }
    }
}
