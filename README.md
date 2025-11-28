# Photo Order Calculator

Десктоп-приложение для расчёта стоимости заказа фотографий.

## Требования

- Windows 10/11
- .NET 8.0 SDK (для сборки)
- .NET 8.0 Runtime (для запуска)

## Установка .NET 8.0

### Вариант 1: Скачать SDK (для разработки/сборки)
1. Перейдите на https://dotnet.microsoft.com/download/dotnet/8.0
2. Скачайте **SDK** для Windows x64
3. Запустите установщик

### Вариант 2: Через winget
```powershell
winget install Microsoft.DotNet.SDK.8
```

## Сборка

### Через командную строку:

```bash
# Перейти в папку проекта
cd PhotoOrderCalculator

# Собрать проект
dotnet build -c Release

# Запустить
dotnet run -c Release
```

### Создать готовый EXE-файл (самодостаточный):

```bash
# Сборка в один EXE файл (не требует .NET на целевой машине)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

# Результат будет в:
# bin\Release\net8.0-windows\win-x64\publish\PhotoOrderCalculator.exe
```

### Сборка компактного EXE (требует .NET Runtime на машине):

```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true

# Результат будет в:
# bin\Release\net8.0-windows\win-x64\publish\PhotoOrderCalculator.exe
```

## Использование

### Запуск
1. Запустите `PhotoOrderCalculator.exe`
2. Приложение свернётся в системный трей (область уведомлений)
3. Появится уведомление о запуске

### Горячая клавиша
- **Ctrl+Shift+K** — открыть форму ввода заказа

### Работа с заказом

1. Нажмите **Ctrl+Shift+K** или дважды кликните по иконке в трее
2. Введите количество фото для первой позиции
3. Нажмите **Tab** или **Enter** для перехода к следующей позиции (создаётся автоматически)
4. Повторите для всех позиций
5. Нажмите **"Показать клиенту"**
6. Покажите экран клиенту
7. Клиент нажимает **"Готово"**

### Кнопки формы фотографа
- **Добавить позицию** — добавить ещё одну строку
- **Показать клиенту** — сохранить заказ и открыть экран для клиента
- **Очистить** — сбросить все позиции

### Контекстное меню в трее
- **Открыть** — открыть форму ввода
- **Открыть папку с данными** — перейти к CSV-файлу с историей заказов
- **Выход** — закрыть приложение

## Формула расчёта

Текущая формула (можно изменить в `PriceCalculator.cs`):

- За позицию с 1–3 фото берётся фиксированная сумма **1600 ₽**
- Каждое фото сверх трёх в той же позиции: **+400 ₽** за штуку
- Скидки не используются

### Как изменить формулу

Откройте файл `PriceCalculator.cs` и измените константы:

```csharp
// За каждые первые BasePackCount фото в позиции
private const int BasePackCount = 3;
private const decimal BasePackPrice = 1600m;

// Каждое фото сверх BasePackCount
private const decimal AdditionalPhotoPrice = 400m;
```

Или полностью перепишите метод `CalculateTotal()` для своей логики.

## Хранение данных

Заказы сохраняются в CSV-файл:
```
%LOCALAPPDATA%\PhotoOrderCalculator\orders.csv
```

Пример пути:
```
C:\Users\ИмяПользователя\AppData\Local\PhotoOrderCalculator\orders.csv
```

### Формат CSV

```csv
id;date_time;total_photos;total_amount;positions_json
A1B2C3D4;2024-01-15 14:30:22;25;3187.50;"[{""position"":1,""count"":10},{""position"":2,""count"":15}]"
```

## Структура проекта

```
PhotoOrderCalculator/
├── PhotoOrderCalculator.csproj  # Файл проекта
├── Program.cs                   # Точка входа
├── TrayApplicationContext.cs    # Логика работы в трее
├── GlobalHotkey.cs              # Глобальные горячие клавиши
├── PhotographerForm.cs          # Форма для фотографа
├── ClientForm.cs                # Форма для клиента
├── PriceCalculator.cs           # Расчёт цены
├── Order.cs                     # Модель данных
├── OrderStorage.cs              # Сохранение в CSV
└── README.md                    # Эта инструкция
```

## Возможные проблемы

### "Не удалось зарегистрировать горячую клавишу"
Другое приложение уже использует Ctrl+Shift+K. Закройте конфликтующее приложение или измените комбинацию в коде (`TrayApplicationContext.cs`).

### Приложение не запускается
Убедитесь, что установлен .NET 8.0 или используйте self-contained сборку.

### Иконка в трее не отображается
Проверьте настройки Windows: "Параметры" → "Персонализация" → "Панель задач" → "Переполнение угла панели задач".
