## 🛒 Marketplace - Cart Module

Módulo de carrito de compras desarrollado en ASP.NET Core siguiendo
arquitectura por capas (Controller, Service, Repository) y acceso a datos
mediante SQL puro con PostgreSQL y Npgsql.

### Características
- Creación automática de carrito por usuario
- Agregar productos al carrito
- Actualización de cantidades
- Eliminación de productos
- Cálculo de total del carrito
- Manejo de relaciones y claves foráneas en PostgreSQL

### Tecnologías
- ASP.NET Core Web API
- PostgreSQL
- Npgsql
- Arquitectura limpia (Clean Architecture principles)

### Endpoints principales
- `GET /api/cart/{userId}`
- `POST /api/cart/{userId}`
- `PUT /api/cart`
- `DELETE /api/cart/{id}`
- `GET /api/cart/total/{userId}`

### Nota técnica
Durante el desarrollo se identificó y corrigió un problema real de
integridad referencial entre `cart_items` y `carts`, asegurando
consistencia de datos en producción.
