# MovieVotesApi (под кальку)

Минимальное Web API (Minimal API) с двумя эндпоинтами и хранением в PostgreSQL. 
Использованы подходы из конспекта: `app.Map*`, `app.Run`, `AddProblemDetails`, `AddExceptionHandler`, DI через `builder.Services`, route-constraint `{movieId:int}`.

## Эндпоинты
- `GET /movies` — список фильмов с текущими голосами.
- `POST /vote/{movieId:int}` — проголосовать за фильм.

## Как запустить через Docker
```bash
docker compose up --build
```

## Проверка через curl
1) Список фильмов до голосования:
```bash
curl -s http://localhost:8080/movies | jq
```

2) Проголосовать за фильм с ID 1:
```bash
curl -i -X POST http://localhost:8080/vote/1
```

3) Список фильмов после голосования:
```bash
curl -s http://localhost:8080/movies | jq
```

### Замечания
- Таблицы создаются автоматически на старте (`EnsureCreated`), сиды: “The Matrix”, “Inception”, “Interstellar”.
- Голоса хранятся как целочисленное поле `Votes` в таблице `movies`.
- Обработчик ошибок: `AddProblemDetails()` + `AddExceptionHandler()` и `app.UseExceptionHandler()`.
