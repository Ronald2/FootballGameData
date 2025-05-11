# Football Game Data API

API profesional para la gestión y exportación de datos de partidos de fútbol, diseñada para integrarse fácilmente con vMix Data Sources Manager y otros sistemas de automatización de gráficos.

## Contexto y objetivo
Esta aplicación permite gestionar equipos, jugadores, alineaciones, datos de partidos y condiciones climáticas, exportando la información en formatos compatibles con vMix (JSON, CSV, Excel, etc.).

Fue creada para reemplazar el flujo manual basado en archivos Excel, permitiendo una gestión más ágil, robusta y extensible de los datos de juego.

## Características principales
- Gestión de equipos, jugadores, alineaciones y partidos.
- API RESTful moderna y segura.
- Exportación de datos en formatos JSON, CSV y Excel (próximamente).
- Validación robusta de datos y manejo profesional de errores.
- Listados paginados y búsqueda eficiente.
- Listo para integración con vMix y otros sistemas de gráficos.

## Instalación y ejecución
1. Clona el repositorio:
   ```powershell
   git clone <url-del-repo>
   cd FootballGameData
   ```
2. Restaura los paquetes y compila:
   ```powershell
   dotnet restore
   dotnet build
   ```
3. Ejecuta la API:
   ```powershell
   dotnet run --project Football.API
   ```

## Pruebas
Para ejecutar los tests automatizados:
```powershell
dotnet test
```

## Exportación de datos
Próximamente: endpoints para exportar datos en JSON, CSV y Excel listos para vMix.

## Consumo de endpoints de exportación (vMix y otros)

La API expone endpoints para exportar datos en formatos JSON y CSV, ideales para integrarse con vMix Data Sources Manager o para ser usados en otras herramientas.

### Endpoints disponibles

- Equipos: `/api/teams/export/json` y `/api/teams/export/csv`
- Jugadores de un equipo: `/api/teams/{teamId}/players/export/json` y `/api/teams/{teamId}/players/export/csv`
- Partidos: `/api/games/export/json` y `/api/games/export/csv`
- Alineaciones de un partido: `/api/games/{gameId}/lineups/export/json` y `/api/games/{gameId}/lineups/export/csv`
- Clima de un partido: `/api/games/{gameId}/weather/export/json` y `/api/games/{gameId}/weather/export/csv`

### Ejemplo de consumo desde vMix Data Sources Manager

1. Abre vMix y ve a "Data Sources".
2. Elige "Add" y selecciona el tipo de fuente:
   - Para JSON: elige "JSON".
   - Para CSV: elige "CSV, Text/CSV".
3. En la URL, coloca la ruta completa al endpoint, por ejemplo:
   - `http://localhost:5000/api/teams/export/csv`
   - `http://localhost:5000/api/games/export/json`
4. Configura el refresco automático si lo deseas.
5. vMix leerá y mostrará los datos listos para usar en tus gráficos.

### Ejemplo de descarga manual (curl o navegador)

- Descargar equipos en CSV:
  ```powershell
  curl -o equipos.csv http://localhost:5000/api/teams/export/csv
  ```
- Descargar jugadores de un equipo en JSON:
  ```powershell
  curl -o jugadores.json http://localhost:5000/api/teams/1/players/export/json
  ```
- También puedes abrir la URL en tu navegador y se descargará el archivo.

### Parámetros opcionales

Puedes usar los parámetros `page` y `pageSize` en los endpoints para limitar la cantidad de datos exportados:

```
/api/teams/export/csv?page=1&pageSize=50
```

---

¿Tienes dudas sobre cómo integrar algún formato específico? ¡Contáctame o revisa la documentación de vMix para Data Sources!

## Contribuciones
Pull requests y sugerencias son bienvenidas.

## Licencia
MIT

# Cobertura de código

![Cobertura de código](https://img.shields.io/badge/dynamic/json?color=brightgreen&label=coverage&query=percent&url=https://raw.githubusercontent.com/${{ github.repository }}/gh-pages/coverage-summary.json)

Este badge muestra la cobertura de código generada automáticamente por el pipeline de CI.

> Para actualizar el badge, asegúrate de que el workflow suba el archivo coverage-summary.json a la rama gh-pages o a un endpoint público.
