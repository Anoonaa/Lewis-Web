# Lewis Stores Web + API

Lewis Stores is a React frontend (Vite) with an ASP.NET Core Web API backend.

## Local Development

### Frontend

1. Install packages:

	npm install

2. Start the app:

	npm run dev

### API

1. Move into the API project:

	cd LewisStores.Api

2. Run the API:

	dotnet run

3. Open API docs:

	http://localhost:5000/docs
	or
	https://localhost:5001/docs

## Deployment

This repository includes a Render blueprint at [render.yaml](render.yaml) for API-only deployment.

### API on Render (recommended)

1. Push this repo to GitHub.
2. In Render, click New + and choose Blueprint.
3. Select this repository.
4. Render will create:
	- lewis-stores-api (dotnet web service)

### API on Render (manual)

Create one service:

1. API service (Web Service)
	- Root directory: LewisStores.Api
	- Runtime: .NET
	- Build command: dotnet restore && dotnet publish -c Release -o out
	- Start command: dotnet out/LewisStores.Api.dll
	- Environment variables:
	  - ASPNETCORE_ENVIRONMENT=Production
	  - CONNECTION_STRINGS__DEFAULT_CONNECTION=Data Source=/var/data/lewis.db
	- Persistent Disk:
	  - Mount path: /var/data
	  - Size: 1 GB

### Frontend on Vercel

1. Import this repo into Vercel.
2. Framework preset: Vite.
3. Build command: npm run build.
4. Output directory: dist.
5. Set environment variable:
	- VITE_API_BASE_URL=https://<your-render-api>.onrender.com

## Production Notes

- The API now binds automatically to Render's PORT environment variable.
- SQLite should be stored on a mounted disk path (/var/data) so data survives redeploys.
- SQLite inside the API is fine for smaller workloads, but it is not ideal for high concurrency or multi-instance scaling.
- Swagger/OpenAPI UI is available at /docs and the JSON at /docs/v1/openapi.json.
