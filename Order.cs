using System;
using System.Collections.Generic;
using System.Text.Json;

namespace PhotoOrderCalculator
{
    /// <summary>
    /// Позиция заказа.
    /// </summary>
    public class OrderPosition
    {
        public int Position { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// Заказ целиком.
    /// </summary>
    public class Order
    {
        public string Id { get; set; } = "";
        public DateTime DateTime { get; set; }
        public int TotalPhotos { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderPosition> Positions { get; set; } = new();

        /// <summary>
        /// Сериализует позиции в JSON.
        /// </summary>
        public string GetPositionsJson()
        {
            return JsonSerializer.Serialize(Positions);
        }

        /// <summary>
        /// Генерирует уникальный ID заказа.
        /// </summary>
        public static string GenerateId()
        {
            return Guid.NewGuid().ToString("N")[..8].ToUpper();
        }
    }
}
