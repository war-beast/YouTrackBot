# Телеграм-бот для постановки задач в YouTrack
Консольное приложение для Телеграм-бота, создающего задачи в YouTrack

#Задание: 
Мы используем для проектной работы YouTrack (постановка заадч) и Телеграм (коммуникация). Было бы удобно, если бы можно было писать боту поставить задачу прямо в проектном чате. Для создания задач можно использовать REST API: https://www.jetbrains.com/help/youtrack/standalone/youtrack-rest-api-reference.html

Формат сообщения боту: 
@bot project task/feature/bug name description///, где "project" - краткое имя проекта (ID), "///" -признак конца команды

Бот ставит задачу и отвечает ссылкой на созданную задачу или сообщает об ошибке.

#Настройка бота
1. Необходимо зарегистрировать бота в Телеграм с помощью @BotFather.
2. Токен доступа вписать в appsettings.json, параметр TelegramKey.
3. Настроить публичный доступ /setprivacy
4. Настроить встроенные запросы /setinline, шаблон команды: project task/feature/bug name description///
5. Настроить прокси для доступа к Телеграму, в проекте используется прокси Tor. Параметры appsettings.json: SocksProxy и ProxyPort

#Настройка YouTrack
1. Получить токен авторизации администратора в YouTrak и вставить его в appsettings.json, параметр YouTrackToken.
2. URL-адрес панели управления вписать в appsettings.json, параметр YouTrackUrl.
