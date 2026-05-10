# CARSTAR Frontend

React + TypeScript + Vite frontend для ASP.NET Core API.

## Структура

```
src/
├── styles/tokens.css          # CSS змінні (дизайн-система)
├── types/index.ts             # TypeScript типи = DTO з backend
├── api/index.ts               # Всі API ендпоінти
├── hooks/
│   ├── useAuth.tsx            # JWT авторизація
│   └── useCart.tsx            # Стан кошика
├── components/
│   ├── Header                 # Навігація
│   ├── HeroSection            # Головний банер
│   ├── AuthModal              # Логін / реєстрація
│   ├── CartSidebar            # Кошик + оформлення
│   └── ProductCard            # Картка товару
├── pages/
│   ├── CatalogPage            # Каталог з фільтрами
│   ├── ProductPage            # Сторінка товару
│   └── ProfilePage            # Профіль + замовлення
└── App.tsx                    # Роутинг
```

## Запуск

### 1. Backend (ASP.NET)
```bash
cd /path/to/carstar-api
dotnet run
# Слухає на http://localhost:5253
```

### 2. Frontend
```bash
npm install
npm run dev
# Відкрий http://localhost:5173
```

Vite автоматично проксує `/api` → `http://localhost:5253`.

## API ендпоінти

| Метод  | URL                              | Використання              |
|--------|----------------------------------|---------------------------|
| POST   | /api/account/register            | Реєстрація                |
| POST   | /api/account/login               | Логін → JWT токен         |
| GET    | /api/account/profile             | Профіль (Bearer token)    |
| GET    | /api/categories                  | Список категорій          |
| GET    | /api/products/filter             | Каталог з фільтрами       |
| GET    | /api/products/{id}               | Деталі товару             |
| GET    | /api/cart/{accountId}            | Кошик                     |
| POST   | /api/cart/{accountId}/add        | Додати товар              |
| PUT    | /api/cart/{accountId}/update     | Оновити кількість         |
| DELETE | /api/cart/{accountId}/delete/{id}| Видалити товар            |
| DELETE | /api/cart/{accountId}/clear      | Очистити кошик            |
| POST   | /api/orders                      | Оформити замовлення       |
| GET    | /api/orders/user/{accountId}     | Мої замовлення            |
| DELETE | /api/orders/{id}/cancel          | Скасувати замовлення      |

## Статуси замовлень

```
Pending → Processing → Shipped → Delivered
                    ↘ Cancelled
```

## Дизайн-система

Всі кольори та шрифти через CSS змінні в `tokens.css`:

```css
--bg      /* #080808 — фон */
--s1..s4  /* поверхні */
--t1..t4  /* текст */
--acc     /* акцент (білий) */
--red     /* #c0392b */
--grn     /* #27ae60 */
--ylw     /* #d4a017 */
```

Шрифти: **Barlow Condensed** (заголовки), **Barlow** (текст), **JetBrains Mono** (ціни, коди).
