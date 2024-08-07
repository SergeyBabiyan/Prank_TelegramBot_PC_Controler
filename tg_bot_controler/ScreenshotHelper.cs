using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;


namespace tg_bot_controler
{
    internal class ScreenshotHelper
    {
        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(int nIndex);

        public async Task TakeScreenshot(string filePath)
        {
            // Получаем размеры экрана
            int screenWidth = GetSystemMetrics(0);
            int screenHeight = GetSystemMetrics(1);

            // Создаем пустое изображение размером с экран
            using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // Копируем изображение с экрана в bitmap
                    g.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
                }

                // Сохраняем скриншот в файл
                bitmap.Save(filePath, ImageFormat.Png);
            }

        }

    }
}
