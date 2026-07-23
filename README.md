# MyShopClean

یک فروشگاه اینترنتی فارسی با ASP.NET Core MVC، Entity Framework Core، SQL Server، Identity و معماری لایه‌ای است. نسخه فعلی روی **.NET 10 LTS** اجرا می‌شود و برای اجرای محلی یک تنظیم کامل Docker دارد.

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
$env:Database__ApplyMigrationsOnStartup = "true" # فقط برای توسعه محلی
$env:SeedDemoData__Enabled = "true"
dotnet run --project .\MyShop\MyShop.csproj
```

تنظیمات اختیاری ایمیل و reCAPTCHA نیز باید با User Secrets یا متغیرهای محیطی مانند `MailSettings__Password` و `reCAPTCHA__SecretKey` وارد شوند؛ هیچ کلید واقعی نباید در `appsettings.json` commit شود.

محیط Development از کلیدهای تست عمومی reCAPTCHA استفاده می‌کند که همیشه اجازه عبور می‌دهند؛ برای Production حتماً کلیدهای دامنه خودتان را تنظیم کنید.

برای فعال‌کردن پرداخت، مقادیر `Payments__Zarinpal__MerchantId` یا `Payments__IdPay__ApiKey` و
`Payments__CallbackBaseUrl` را فقط از طریق Secret Manager یا متغیر محیطی تنظیم کنید. هیچ کلید واقعی را داخل
`appsettings.json` یا Git ثبت نکنید. کلید IDPay که قبلاً در تاریخچه مخزن قرار گرفته باید در پنل درگاه باطل و
تعویض شود.

در Production، migrationها هنگام بالا آمدن هر Replica اجرا نمی‌شوند. آن‌ها را یک‌بار در مرحله استقرار اجرا کنید:

```powershell
dotnet ef database update --project .\Infra.Data --startup-project .\MyShop
```

## تنظیمات پویا و اتصال فروشگاه‌ها

بعد از ورود مدیر، از منوی مدیریت وارد **تنظیمات سایت** شوید. نام و شعار سایت، سه رنگ اصلی، متن‌های فوتر، آدرس، تلفن، ایمیل پشتیبانی و آدرس عمومی سایت از این بخش تغییر می‌کنند و در دیتابیس ذخیره می‌شوند.

برای آماده‌سازی خروجی محصولات ترب و ایمالز، آدرس عمومی سایت را با دامنه نهایی مقداردهی و سرویس موردنظر را فعال کنید. در صورت نیاز یک توکن غیرقابل‌حدس هم تعیین کنید:

```text
GET /integrations/torob/products
GET /integrations/emalls/products
GET /integrations/emalls/products.xml
X-Integration-Token: YOUR_TOKEN
```

قیمت خروجی‌ها بر حسب تومان است. فعال‌کردن endpoint به‌تنهایی فروشگاه را در ترب یا ایمالز ثبت نمی‌کند؛ آدرس خروجی را پس از ثبت و تأیید فروشگاه، مطابق فرمت نهایی مورد قبول پنل آن سرویس در اختیار پشتیبانی آن قرار دهید.

## بررسی پروژه

```powershell
dotnet restore MyShopClean.sln
dotnet build MyShopClean.sln -c Release --no-restore
dotnet test MyShopClean.sln -c Release --no-build
& .\scripts\smoke-test.ps1
& .\scripts\site-audit.ps1
```

تست آخر، تنظیمات پویا و feedهای فروشگاه را با یک دیتابیس موقت بررسی می‌کند و از هفت صفحه عمومی در اندازه موبایل، تبلت و دسکتاپ داخل `artifacts/responsive-audit` تصویر می‌گیرد. اجرای آن به SQL Server LocalDB و Google Chrome نیاز دارد.

> migrationهای قدیمی این شاخه schema ناسازگار تولید می‌کردند و با یک migration اولیه تمیز جایگزین شده‌اند. برای این نسخه از یک دیتابیس تازه استفاده کنید؛ اگر دیتابیس قبلی دارید، ابتدا از آن پشتیبان بگیرید.
