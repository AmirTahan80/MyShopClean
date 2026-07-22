# MyShopClean

یک فروشگاه اینترنتی فارسی با ASP.NET Core MVC، Entity Framework Core، SQL Server، Identity و معماری لایه‌ای است. نسخه فعلی روی **.NET 8 LTS** اجرا می‌شود و برای اجرای محلی یک تنظیم کامل Docker دارد.

## اجرای سریع با Docker

پیش‌نیاز: Docker Desktop

```powershell
Copy-Item .env.example .env
docker compose up --build
```

سپس برنامه را در `http://localhost:8080` باز کنید. دیتابیس، migrationها، نقش‌ها، چند محصول نمونه و بنرهای دموی صفحه اصلی به‌صورت خودکار ساخته می‌شوند.

برای ورود مدیر از نام کاربری `admin` و مقدار `SEED_ADMIN_PASSWORD` داخل فایل `.env` استفاده کنید. قبل از انتشار عمومی، هر دو رمز نمونه در `.env` را تغییر دهید. فایل `.env` توسط Git نادیده گرفته می‌شود.

توقف برنامه:

```powershell
docker compose down
```

حذف کامل دیتابیس دموی محلی:

```powershell
docker compose down --volumes
```

## اجرای مستقیم با .NET

یک SQL Server در دسترس قرار دهید و connection string را با متغیر محیطی تنظیم کنید:

```powershell
$env:ConnectionStrings__ConnectToDataBase = "Server=localhost,1433;Database=MyShop;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
$env:SeedAdmin__Password = "YOUR_ADMIN_PASSWORD"
dotnet run --project .\MyShop\MyShop.csproj
```

تنظیمات اختیاری ایمیل و reCAPTCHA نیز باید با User Secrets یا متغیرهای محیطی مانند `MailSettings__Password` و `reCAPTCHA__SecretKey` وارد شوند؛ هیچ کلید واقعی نباید در `appsettings.json` commit شود.

محیط Development از کلیدهای تست عمومی reCAPTCHA استفاده می‌کند که همیشه اجازه عبور می‌دهند؛ برای Production حتماً کلیدهای دامنه خودتان را تنظیم کنید.

## بررسی پروژه

```powershell
dotnet restore MyShopClean.sln
dotnet build MyShopClean.sln -c Release --no-restore
dotnet test MyShopClean.sln -c Release --no-build
& .\scripts\smoke-test.ps1
```

> migrationهای قدیمی این شاخه schema ناسازگار تولید می‌کردند و با یک migration اولیه تمیز جایگزین شده‌اند. برای این نسخه از یک دیتابیس تازه استفاده کنید؛ اگر دیتابیس قبلی دارید، ابتدا از آن پشتیبان بگیرید.
