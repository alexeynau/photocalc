using System;
using System.IO;
using System.Text;

namespace PhotoOrderCalculator
{
    /// <summary>
    /// Сохраняет заказы в CSV файл.
    /// </summary>
    public static class OrderStorage
    {
        private static readonly string DataDirectory;
        private static readonly string CsvFilePath;
        
        static OrderStorage()
        {
            // Храним данные в папке приложения
            DataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PhotoOrderCalculator"
            );
            
            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);
            
            CsvFilePath = Path.Combine(DataDirectory, "orders.csv");
        }

        /// <summary>
        /// Сохраняет заказ в CSV файл.
        /// </summary>
        public static void SaveOrder(Order order)
        {
            bool fileExists = File.Exists(CsvFilePath);
            
            using var writer = new StreamWriter(CsvFilePath, append: true, Encoding.UTF8);
            
            // Записываем заголовок, если файл новый
            if (!fileExists)
            {
                writer.WriteLine("id;date_time;total_photos;total_amount;positions_json");
            }
            
            // Экранируем JSON (заменяем ; на другой символ или оборачиваем в кавычки)
            string positionsJson = order.GetPositionsJson().Replace("\"", "\"\"");
            
            writer.WriteLine(
                $"{order.Id};" +
                $"{order.DateTime:yyyy-MM-dd HH:mm:ss};" +
                $"{order.TotalPhotos};" +
                $"{order.TotalAmount:F2};" +
                $"\"{positionsJson}\""
            );
        }

        /// <summary>
        /// Возвращает путь к файлу с данными.
        /// </summary>
        public static string GetDataFilePath() => CsvFilePath;
    }
}
