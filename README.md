# Workday notifier bot

Tg bot to notify users about their workday duration

## Availble commands: 

`/starttime [HH:MM]` - set start work time

`/status` - prints user settings

`/duration [HH]` - set workday duration in hours

`/period [MM]` - set notification period in minutes

`/lastworkday [MM:DD:YYYY]` - set last work day

`/dayswowork` - prints date since last work day

`/enable` - enable notifications

`/tillend` - prints time till end of work day

## Run

Clone repo

Rename `appsettings.example.json` to `appsettings.json` and paste your tg bot token  

`cd WorkdayNotifierBot`

### Localy

`dotnet run`

### Docker

`docker-compose up --build`