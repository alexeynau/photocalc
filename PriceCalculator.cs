using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoOrderCalculator
{
    /// <summary>
    /// Класс для расчёта стоимости заказа.
    /// Формулу легко изменить в методе CalculateTotal.
    /// </summary>
    public static class PriceCalculator
    {
        // За каждые первые 3 фото в позиции — фиксированная цена
        private const int BasePackCount = 3;
        private const decimal BasePackPrice = 1600m;
        
        // Каждое фото сверх первых трёх в позиции
        private const decimal AdditionalPhotoPrice = 400m;

        /// <summary>
        /// Рассчитывает стоимость одной позиции.
        /// </summary>
        public static decimal CalculatePositionTotal(int count)
        {
            if (count <= 0)
                return 0m;

            // При 1–3 фото берём фиксированную цену за пакет.
            if (count <= BasePackCount)
                return BasePackPrice;

            int extra = count - BasePackCount;
            return BasePackPrice + extra * AdditionalPhotoPrice;
        }
        
        /// <summary>
        /// Рассчитывает общую стоимость заказа.
        /// </summary>
        /// <param name="positions">Список позиций (количество фото в каждой)</param>
        /// <returns>Итоговая сумма</returns>
        public static decimal CalculateTotal(IEnumerable<int> positions)
        {
            return positions.Sum(CalculatePositionTotal);
        }
    }
}
