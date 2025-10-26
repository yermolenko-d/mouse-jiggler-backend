# 🚀 Deployment Guide for MouseJiggler (.NET 8 + PostgreSQL + IIS)

This guide describes how to deploy updates and apply database migrations to your production VPS.

---

## 🧩 1. Build the project locally

Run this command from your development machine:

```bash
dotnet publish MouseJigglerBackend/MouseJigglerBackend.csproj \
  -c Release -r win-x64 --self-contained false -o ./publish
```
The output folder publish/ will contain all the necessary files (*.dll, web.config, static files, etc.).


📂 2. Copy the files to your VPS
Upload the contents of ./publish/ to your VPS directory:
C:\inetpub\MouseJiggler

The IIS Application Pool should use No Managed Code.
Environment variables (DB_HOST, DB_PORT, DB_NAME, DB_USER, DB_PASSWORD, ASPNETCORE_ENVIRONMENT=Production) must already be set.

🧱 3. Apply database migrations (Idempotent Script)
▶️ Step 3.1 — Generate migration script locally
On your local machine, run:

```bash
dotnet ef migrations script \
  --project MouseJigglerBackend.DAL \
  --startup-project MouseJigglerBackend \
  --idempotent > Migrations_Prod.sql
```

Save the file as UTF-8 (no BOM) and upload it to your VPS at:
C:\deploy\Migrations_Prod.sql
▶️ Step 3.2 — Run migration script on the VPS

In PowerShell (Run as Administrator):
```powershell
# Temporarily disable the site during migration
New-Item -Path "C:\inetpub\MouseJiggler\app_offline.htm" -ItemType File -Force | Out-Null

$env:PGPASSWORD="<your_dev_password>"
& "C:\Program Files\PostgreSQL\18\bin\psql.exe" `
  -h localhost -U dev -d jiggler_prod `
  -f "C:\deploy\Migrations_Prod.sql"
# Reactivate the site
Remove-Item "C:\inetpub\MouseJiggler\app_offline.htm" -Force
iisreset
```

🧾 4. Verify the deployment
✅ Check migrations applied:
```
SELECT "MigrationId", "ProductVersion"
FROM "__EFMigrationsHistory"
ORDER BY "MigrationId" DESC;
```
✅ Check your API:

http://localhost/health
or your production domain.