using SmartHomeCLI;
using SmartHomeCLI.DTOs;
using System.Security;

#region Создаем ApiService и проверяем соединение

// Я вынес все методы для работы с API в отдельный класс ApiService, чтобы не загромождать код программы и не повторять код для отправки HTTP запросов в каждом методе.
// А еще вы частично сможете перенести его в свой ВПФ проект.
// Если вы обратите внимание на методы в ApiService, то увидите, что они все НЕ ИСПОЛЬЗУЮТ КОНСОЛЬ.
// Они просто отправляют запросы на сервер и возвращают данные, не заботясь о том, как эти данные будут отображаться пользователю.
// РАБОТА ПО ОТОБРАЖЕНИЮ ДАННЫХ В КОНСОЛИ ЛОЖИТСЯ НА ПРОГРАММУ, КОТОРАЯ ВЫЗЫВАЕТ МЕТОДЫ API, И ОТВЕЧАЕТ ЗА ВЗАИМОДЕЙСТВИЕ С ПОЛЬЗОВАТЕЛЕМ.
// В КОНСОЛИ ЭТО КОНСОЛЬ, В ВПФ ЭТО БУДУТ РАЗЛИЧНЫЕ ЭЛЕМЕНТЫ ИНТЕРФЕЙСА, КОТОРЫЕ БУДУТ ПОДПИСАНЫ НА ИЗМЕНЕНИЯ ДАННЫХ ЧЕРЕЗ МЕХАНИЗМ БИНДИНГА И ОБНОВЛЯТЬСЯ ПРИ ИЗМЕНЕНИИ ДАННЫХ В МОДЕЛИ ДАННЫХ, 
// КОТОРАЯ В СВОЮ ОЧЕРЕДЬ БУДЕТ ОБНОВЛЯТЬСЯ ПРИ ВЫЗОВЕ МЕТОДОВ API.
ApiService apiService = new ApiService();

Console.WriteLine("Проверка соединения с сервером...");

// Проверяем вообще жив ли сервер.
bool isConnected = await apiService.CheckConnectionAsync();
if (isConnected)
{
    // Зеленый зацкерский цвет если успех.
    Console.ForegroundColor = ConsoleColor.Green; 
    Console.WriteLine("✓ Соединение установлено!");
    // Ждем чуть-чуть, чтобы пользователь успел увидеть сообщение об успешном соединении.
    Task.Delay(1500).Wait();
    // Вернем цвет в норму для дальнейшего вывода. Иначе все сообщения будут зеленые.
    Console.ForegroundColor = ConsoleColor.White;
}
else
{
    // Красный цвет если не удалось подключиться.
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("✗ Не удалось подключиться к серверу. Пожалуйста, убедитесь, что сервер запущен и доступен по адресу http://localhost:5000");
    Task.Delay(2000).Wait();

    // Говорим ухади, ждем кнопку и выходим из программы, так как без сервера работать нет смысла.
    Console.WriteLine("Нажмите любую клавишу для выхода...");
    Console.ReadKey();
    Console.ForegroundColor = ConsoleColor.White;
    return;
}

Console.ForegroundColor = ConsoleColor.White;


#endregion

// Пример вызова метода получения всех квартир и их комнат, чтобы показать, что данные успешно получаются с сервера.
var apartList = apiService.GetApartmentsAsync().Result;

Console.WriteLine($"Найдено {apartList.Count} квартир:\n");

// Для каждой квартиры выводим ее информацию, а также информацию о комнатах, которые в ней находятся.
// Класс АпартментДТО не содержит информацию о кмонатах напрямую, но мы можем получить комнаты для каждой квартиры, вызвав отдельный метод GetRoomsByApartmentIdAsync, передав ему ID квартиры.
foreach (ApartmentDto apart in apartList)
{
    // Данные о квартире.
    Console.WriteLine($"ID: {apart.Id}\n> Номер: {apart.Number}\n> Описание: {apart.Description}\n> Кол-во комнат: {apart.RoomsCount}\n");

    // На каждую квартиру мы вызываем метод получения комнат по ID квартиры, и выводим информацию о каждой комнате, которая там находится.
    // Этот объект возвращает нам РумДТО с полной информацией о комнате, так что мы можем вывести все ее свойства, включая площадь, температуру и состояние света.
    var roomList = apiService.GetRoomsByApartmentIdAsync(apart.Id).Result;

    // красивенько выводим
    foreach (RoomDto room in roomList)
    {
        Console.WriteLine("-------------------------------");
        Console.WriteLine($"|\tID: {room.Id}\n|\t> Название: {room.Name}\n|\t> Тип: {room.RoomType}\n|\t> Температура: {room.Temperature}°C\n|\t> Площадь: {room.Area} м²\n|\t> Свет: {(room.LightState ? "Включен" : "Выключен")}\n");
        Console.WriteLine("-------------------------------");
    }
}

Console.ForegroundColor = ConsoleColor.White;

Console.WriteLine("\nВсе данные успешно получены с сервера.");
Console.WriteLine("\nНажмите любую клавишу для продолжения...");
Console.ReadKey();

List<string> commands = new List<string>
{
"1.\t[Квартиры] Получить список всех квартир",
"2.\t[Квартиры] Создать новую квартиру",
"3.\t[Квартиры] Обновить информацию о квартире",
"4.\t[Квартиры] Удалить квартиру по ID",
"5.\t[Квартиры] Получить список комнат для квартиры по ID",
"6.\t[Комнаты]  Получить список всех комнат",
"7.\t[Комнаты]  Создать новую комнату",
"8.\t[Комнаты]  Получить информацию о комнате по ID",
"9.\t[Комнаты]  Обновить метаданные комнаты (название, описание, тип)",
"10.\t[Комнаты]  Обновить датчики комнаты (площадь, температура, свет)",
"11.\t[Комнаты]  Переключить свет в комнате (Вкл/Выкл)",
"12.\t[Комнаты]  Удалить комнату по ID",
"13.\t[Квартиры]  Выход"
};

int selectedIndex = 0;

// Я не буду описывать весь этот код, так как он не имеет отношения к работе с API, это просто реализация консольного меню для удобства тестирования методов API.
// надеюсь вы не будете в этой лабе рассказывать про реализацию консольного меню, так как это не имеет отношения к работе с API, и я не хочу тратить время на объяснение этого кода, который просто выводит список команд, позволяет пользователю выбирать команду с помощью стрелочек и нажимать Enter для выполнения выбранной команды.
// а еще если вы дочитали до этого места, просто скажите на защите "Лефрут", только никому не рассказывайте что вы тут это прочитали, и тогда я все пойму и поставлю вам за это балл.

while (true)
{
    Console.Clear();
    Console.WriteLine("=== УПРАВЛЕНИЕ УМНЫМ ДОМОМ ===\n");

    for (int i = 0; i < commands.Count; i++)
    {
        if (i == selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("> " + commands[i]);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  " + commands[i]);
        }
    }
    Console.ResetColor();

    Console.WriteLine("\nСтрелочки ВВЕРХ/ВНИЗ — навигация. Enter — выбор. Q — выход.");

    var key = Console.ReadKey(true).Key;

    if (key == ConsoleKey.Q)
    {
        break;
    }
    else if (key == ConsoleKey.UpArrow)
    {
        selectedIndex = (selectedIndex - 1 + commands.Count) % commands.Count;
    }
    else if (key == ConsoleKey.DownArrow)
    {
        selectedIndex = (selectedIndex + 1) % commands.Count;
    }
    else if (key == ConsoleKey.Enter)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"--- {commands[selectedIndex].Substring(4).Trim()} ---\n");
        Console.ResetColor();

        switch (selectedIndex)
        {
            case 0: await GetAllApartmentsAsync(apiService); break;
            case 1: await CreateApartmentAsync(apiService); break;
            case 2: await UpdateApartmentAsync(apiService); break;
            case 3: await DeleteApartmentAsync(apiService); break;
            case 4: await GetRoomsByApartmentIdAsync(apiService); break;
            case 5: await GetAllRoomsAsync(apiService); break;
            case 6: await CreateRoomAsync(apiService); break;
            case 7: await GetRoomByIdAsync(apiService); break;
            case 8: await UpdateRoomMetadataAsync(apiService); break;
            case 9: await UpdateRoomSensorsAsync(apiService); break;
            case 10: await ToggleRoomLightAsync(apiService); break;
            case 11: await DeleteRoomAsync(apiService); break;
            case 12: return; // Выход
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
        Console.ResetColor();
        Console.ReadKey(true);
    }
}
// =========================================================
// МЕТОДЫ ДЛЯ КВАРТИР (Apartments)
// =========================================================
// По сути (вкусно), вы можете перенести эти методы в отдельный класс, например, ApartmentCommands, и вызывать их из этого меню, чтобы не загромождать код программы. 
// Но я оставил их здесь для удобства и наглядности.
// Опять же, эти методы не имеют никакого отношения к работе с API, это просто реализация взаимодействия с пользователем для получения данных, которые нужны для вызова методов API, и отображения результатов этих методов.
// И как бэ можно вынести эти методы в какой нибудь ApartmentConsoleHelper, и вызывать их из этого меню.
// мы бы отвязались от консоли и могли бы использовать эти методы в любом другом интерфейсе, например, в ВПФ, просто передавая им ApiService и необходимые параметры, 
// а внутри этих методов уже реализовать взаимодействие с пользователем через консоль или через элементы интерфейса в ВПФ.
// Чтобы вам сейчас взять эти методы к себе в программу, вам нужно будет их почистить от использования консоли.

static async Task GetAllApartmentsAsync(ApiService apiService)
{
    try
    {
        var apartments = await apiService.GetApartmentsAsync();
        if (apartments.Count == 0)
        {
            Console.WriteLine("Список квартир пуст.");
            return;
        }

        foreach (var apt in apartments)
        {
            Console.WriteLine($"[ID: {apt.Id}] Квартира №{apt.Number} | Комнат: {apt.RoomsCount} | Описание: {apt.Description}");
        }
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

static async Task CreateApartmentAsync(ApiService apiService)
{
    Console.Write("Введите номер квартиры: ");
    string number = Console.ReadLine() ?? string.Empty;

    Console.Write("Введите описание квартиры: ");
    string description = Console.ReadLine() ?? string.Empty;

    try
    {
        var apt = await apiService.CreateApartmentAsync(number, description);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nКвартира успешно создана! Присвоен ID: {apt.Id}");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

static async Task UpdateApartmentAsync(ApiService apiService)
{
    int id = GetValidInt("Введите ID квартиры для обновления: ");

    Console.Write("Введите новый номер квартиры: ");
    string number = Console.ReadLine() ?? string.Empty;

    Console.Write("Введите новое описание квартиры: ");
    string description = Console.ReadLine() ?? string.Empty;

    try
    {
        var apt = await apiService.UpdateApartmentAsync(id, number, description);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nКвартира успешно обновлена![ID: {apt.Id}] №{apt.Number}");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

static async Task DeleteApartmentAsync(ApiService apiService)
{
    int id = GetValidInt("Введите ID квартиры для удаления: ");

    try
    {
        await apiService.DeleteApartmentAsync(id);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nКвартира успешно удалена.");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

static async Task GetRoomsByApartmentIdAsync(ApiService apiService)
{
    int id = GetValidInt("Введите ID квартиры: ");

    try
    {
        var rooms = await apiService.GetRoomsByApartmentIdAsync(id);
        if (rooms.Count == 0)
        {
            Console.WriteLine("В этой квартире пока нет комнат.");
            return;
        }

        Console.WriteLine($"\nКомнаты в квартире ID {id}:");
        foreach (var room in rooms)
        {
            Console.WriteLine($"[ID: {room.Id}] {room.Name} ({room.RoomType}) | {room.Area} м² | {room.Temperature}°C | Свет: {(room.LightState ? "Вкл" : "Выкл")}");
        }
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

// =========================================================
// МЕТОДЫ ДЛЯ КОМНАТ (Rooms)
// =========================================================

static async Task GetAllRoomsAsync(ApiService apiService)
{
    try
    {
        var rooms = await apiService.GetAllRoomsAsync();
        if (rooms.Count == 0)
        {
            Console.WriteLine("Список комнат пуст.");
            return;
        }

        foreach (var room in rooms)
        {
            Console.WriteLine($"[ID: {room.Id}] Квартира ID: {room.ApartmentId} | {room.Name} ({room.RoomType}) | {room.Description}");
        }
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

static async Task CreateRoomAsync(ApiService apiService)
{
    int aptId = GetValidInt("Введите ID квартиры, где будет создана комната: ");

    Console.Write("Название комнаты: ");
    string name = Console.ReadLine() ?? string.Empty;

    Console.Write("Описание комнаты: ");
    string desc = Console.ReadLine() ?? string.Empty;

    Console.Write("Тип комнаты (например, Спальня): ");
    string type = Console.ReadLine() ?? string.Empty;

    double area = GetValidDouble("Площадь (кв.м): ");
    double temp = GetValidDouble("Температура (°C): ");

    Console.Write("Свет включен? (y/n): ");
    bool light = Console.ReadLine()?.ToLower() == "y";

    try
    {
        var room = await apiService.CreateRoomAsync(aptId, name, desc, type, area, temp, light);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nКомната успешно создана! Присвоен ID: {room.Id}");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

static async Task GetRoomByIdAsync(ApiService apiService)
{
    int id = GetValidInt("Введите ID комнаты: ");

    try
    {
        var room = await apiService.GetRoomByIdAsync(id);
        Console.WriteLine("\n--- ИНФОРМАЦИЯ О КОМНАТЕ ---");
        Console.WriteLine($"ID:\t\t{room.Id}");
        Console.WriteLine($"Название:\t{room.Name} ({room.RoomType})");
        Console.WriteLine($"Описание:\t{room.Description}");
        Console.WriteLine($"Площадь:\t{room.Area} м²");
        Console.WriteLine($"Температура:\t{room.Temperature}°C");
        Console.WriteLine($"Свет:\t\t{(room.LightState ? "Включен" : "Выключен")}");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

static async Task UpdateRoomMetadataAsync(ApiService apiService)
{
    int id = GetValidInt("Введите ID комнаты: ");

    Console.WriteLine("Оставьте поле пустым и нажмите Enter, если не хотите его менять.");

    Console.Write("Новое название комнаты: ");
    string name = Console.ReadLine();

    Console.Write("Новое описание комнаты: ");
    string desc = Console.ReadLine();

    Console.Write("Новый тип комнаты: ");
    string type = Console.ReadLine();

    string? finalName = string.IsNullOrWhiteSpace(name) ? null : name;
    string? finalDesc = string.IsNullOrWhiteSpace(desc) ? null : desc;
    string? finalType = string.IsNullOrWhiteSpace(type) ? null : type;

    try
    {
        var room = await apiService.UpdateRoomMetadataAsync(id, finalName, finalDesc, finalType);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nМетаданные комнаты успешно обновлены!");
        Console.ResetColor();
        Console.WriteLine($"[ID: {room.Id}] {room.Name} ({room.RoomType}) | {room.Description}");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

static async Task UpdateRoomSensorsAsync(ApiService apiService)
{
    int id = GetValidInt("Введите ID комнаты: ");

    Console.WriteLine("Оставьте поле пустым и нажмите Enter, если не хотите его менять.");

    double? area = GetOptionalDouble("Новая площадь (кв.м): ");
    double? temp = GetOptionalDouble("Новая температура (°C): ");
    bool? light = GetOptionalBool("Состояние света (y - вкл, n - выкл): ");

    try
    {
        var room = await apiService.UpdateRoomSensorsAsync(id, area, temp, light);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nДатчики комнаты успешно обновлены!");
        Console.ResetColor();
        Console.WriteLine($"[ID: {room.Id}] Площадь: {room.Area} м² | Температура: {room.Temperature}°C | Свет: {(room.LightState == true ? "Вкл" : "Выкл")}");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

static async Task ToggleRoomLightAsync(ApiService apiService)
{
    int id = GetValidInt("Введите ID комнаты: ");

    try
    {
        var isLightOn = await apiService.ToggleRoomLightAsync(id);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nСвет в комнате ID {id} теперь {(isLightOn ? "ВКЛЮЧЕН" : "ВЫКЛЮЧЕН")}");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

static async Task DeleteRoomAsync(ApiService apiService)
{
    int id = GetValidInt("Введите ID комнаты для удаления: ");

    try
    {
        await apiService.DeleteRoomAsync(id);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nКомната успешно удалена.");
    }
    catch (Exception ex) { Console.WriteLine(ex.Message); }
}

// =========================================================
// ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ДЛЯ ЧТЕНИЯ ВВОДА
// =========================================================

static int GetValidInt(string prompt)
{
    int result;
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out result))
            return result;
        Console.WriteLine("Ошибка: Введите целое число.");
    }
}

static double GetValidDouble(string prompt)
{
    double result;
    while (true)
    {
        Console.Write(prompt);
        if (double.TryParse(Console.ReadLine()?.Replace('.', ','), out result))
            return result;
        Console.WriteLine("Ошибка: Введите число (можно с десятичной точкой).");
    }
}

static double? GetOptionalDouble(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return null;

        if (double.TryParse(input.Replace('.', ','), out double result))
            return result;

        Console.WriteLine("Ошибка: Введите число или оставьте пустым.");
    }
}

static bool? GetOptionalBool(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string input = Console.ReadLine()?.ToLower();
        if (string.IsNullOrWhiteSpace(input)) return null;

        if (input == "y" || input == "д") return true;
        if (input == "n" || input == "н") return false;

        Console.WriteLine("Ошибка: Введите 'y' (вкл) или 'n' (выкл).");
    }
}