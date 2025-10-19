# 🛒 PF.Sol.EC – E-commerce API Modular (.NET 8)

Proyecto de arquitectura modular para e-commerce, construido con Minimal APIs en .NET 8, contenedores Docker, y múltiples motores de base de datos. Cada módulo representa un contexto independiente dentro del dominio del comercio electrónico. 

## 🧱 Arquitectura
La solución está compuesta por 5 microservicios independientes, cada uno con su propia base de datos:

### 🛠️ Servicios

- #### 1. Sol.EC.Catalogo (Puerto 5001)
   - Base de datos: PostgreSQL (Catalogs)
   - Tablas: Cliente, Articulo
   - Funcionalidad: Gestión de catálogo de productos y clientes

- #### 2. Sol.EC.Comprobantes (Puerto 5002)
  - Base de datos: MySQL (Orders)
  - Tablas: Comprobante, ComprobanteItem
  - Funcionalidad: Gestión de órdenes y facturación

  - Funcionalidad: Control de movimientos de almacén
 
- #### 5. Sol.EC.Consultas (Puerto 5005)
  - Base de datos: MongoDB (Queries)
  - Colección: Comprobante (con items anidados)
  - Funcionalidad: Consultas agregadas y reportes


## 🧪 Tecnologías Utilizadas
- .NET 8 - Framework principal para construir APIs modernas y eficientes
- Minimal APIs - Enfoque ligero para definir endpoints REST sin controladores
- Entity Framework Core - ORM para bases de datos relacionales como SQL Server o PostgreSQL
- MongoDB.Driver - Driver oficial para trabajar con MongoDB
- Serilog - Logging estructurado con soporte para sinks como consola, archivos, Seq, etc.
  
## 🗄️ Bases de Datos
- SQL Server 2022 (Express) – Base de datos relacional para el microservicio de pedidos
- MySQL 8.0 – Base de datos relacional para el microservicio de pagos
- MongoDB 7.0 – Base de datos NoSQL para el microservicio de consultas
- PostgreSQL 16 – Base de datos relacional para la instancia de Keycloak

## 🔐 Seguridad y Autenticación
- Keycloak 26 – Proveedor de identidad y autenticación basado en OAuth2 y OpenID Connect
- JWT (JSON Web Tokens) – Autenticación y autorización entre microservicios

## 📦 Mensajería y Comunicación
- Apache Kafka 3.6.0 (Bitnami) – Plataforma de mensajería distribuida para eventos y comunicación entre microservicios
- Kafka UI (Provectus) – Interfaz visual para monitorear tópicos y mensajes de Kafka

## 📊Observabilidad y Trazabilidad
- Jaeger – Trazabilidad distribuida para seguimiento de peticiones entre microservicios
- Health Checks (Keycloak & DBs) – Verificación de estado de servicios para orquestación y monitoreo

## 🧪 Documentación y Desarrollo
- Swagger / OpenAPI – Documentación interactiva de APIs
- Postman – Pruebas manuales de endpoints y autenticación

## 🐳 Contenedores y Orquestación
- Docker – Empaquetado y ejecución de microservicios en contenedores
- Docker Compose – Orquestación de múltiples servicios en entornos locales
- Red de Docker personalizada (sol-ec-network) – Comunicación entre contenedores

## ✨ Características
- ✅ Logging con Serilog - Logs estructurados en archivos ```/logs```
- ✅ Swagger integrado - Documentación automática de APIs
- ✅ Datos de prueba - 5 registros por tabla con datos realistas
- ✅ Docker ignore - Excluye archivos de logs de las imágenes
- ✅ CRUD completo - Operaciones Create, Read, Update, Delete
- ✅ Validaciones - Data annotations y validaciones de negocio
- ✅ Manejo de errores - Respuestas HTTP apropiadas

## 🧬 Estructura de Datos
### Cliente (Catalogo)
```json
{
  "idCliente": 1,
  "nombreCliente": "Juan Pérez"
}
```
### Articulo (Catalogo)
```json
{
  "idArticulo": 1,
  "nombreArticulo": "Laptop HP Pavilion"
}
```
### Comprobante (Comprobantes)
```json
{
  "idComprobante": 1,
  "fechaComprobante": "2025-01-16T10:30:00Z",
  "idCliente": 1,
  "items": [
    {
      "idComprobanteItem": 1,
      "idArticulo": 1,
      "cantidad": 2
    }
  ]
}
```
### Saldo (Saldos)
```json
{
  "idSaldo": 1,
  "idArticulo": 1,
  "cantidad": 100
}
```
### MovAlmacen (Kardex)
```json
{
  "idMovAlmacen": 1,
  "tipoMovimiento": "I",
  "cantidad": 50,
  "idVenta": 1
}
```
### Comprobante (Consultas - MongoDB)
```json
{
  "id": "ObjectId",
  "codComprobante": 1,
  "fechaComprobante": "2025-01-16T10:30:00Z",
  "idCliente": 1,
  "nombreCliente": "Juan Pérez",
  "items": [
    {
      "idArticulo": 1,
      "nombreArticulo": "Laptop HP Pavilion",
      "cantidad": 2
    }
  ]
}
```

## 🚀 Instalación y Ejecución
### Prerrequisitos
- Docker Desktop
- .NET 8 SDK (para desarrollo local)

### Ejecutar con Docker Compose
```bash
# Clonar el repositorio
git clone <repository-url>
cd Sol.EC

# Ejecutar todos los servicios
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener servicios
docker-compose down
```

### Desarrollo Local
```bash
# Restaurar paquetes
dotnet restore

# Ejecutar servicios individualmente
cd Sol.EC.Catalogo
dotnet run

# O ejecutar desde la solución
dotnet run --project Sol.EC.Catalogo
```
## 🔗 Endpoints de las APIs
### Sol.EC.Catalogo (Puerto 5001)
- ```GET /api/clientes``` - Obtener todos los clientes
- ```GET /api/clientes/{id}``` - Obtener cliente por ID
- ```POST /api/clientes``` - Crear cliente
- ```PUT /api/clientes/{id}``` - Actualizar cliente
- ```DELETE /api/clientes/{id}``` - Eliminar cliente
- ```GET /api/articulos``` - Obtener todos los artículos
- ```GET /api/articulos/{id}``` - Obtener artículo por ID
- ```POST /api/articulos``` - Crear artículo
- ```PUT /api/articulos/{id}``` - Actualizar artículo
- ```DELETE /api/articulos/{id}``` - Eliminar artículo
### Sol.EC.Comprobantes (Puerto 5002)
- ```GET /api/comprobantes``` - Obtener todos los comprobantes
- ```GET /api/comprobantes/{id}``` - Obtener comprobante por ID
- ```POST /api/comprobantes``` - Crear comprobante
- ```PUT /api/comprobantes/{id}``` - Actualizar comprobante
- ```DELETE /api/comprobantes/{id}``` - Eliminar comprobante
- ```GET /api/comprobante-items``` - Obtener todos los items
- ```GET /api/comprobante-items/comprobante/{id}``` - Obtener items por comprobante
### Sol.EC.Saldos (Puerto 5003)
- ```GET /api/saldos``` - Obtener todos los saldos
- ```GET /api/saldos/{id}``` - Obtener saldo por ID
- ```GET /api/saldos/articulo/{idArticulo}``` - Obtener saldos por artículo
- ```POST /api/saldos``` - Crear saldo
- ```PUT /api/saldos/{id}``` - Actualizar saldo
- ```DELETE /api/saldos/{id}``` - Eliminar saldo
- ```POST /api/saldos/validar``` - Validar si hay saldo suficiente
- ```POST /api/saldos/restar``` - Restar cantidad del saldo
- ```POST /api/saldos/sumar``` - Sumar cantidad al saldo
### Sol.EC.Kardex (Puerto 5004)
- ```GET /api/mov-almacenes``` - Obtener todos los movimientos
- ```GET /api/mov-almacenes/{id}``` - Obtener movimiento por ID
- ```GET /api/mov-almacenes/tipo/{tipo}``` - Obtener movimientos por tipo (I/S)
- ```POST /api/mov-almacenes``` - Crear movimiento
- ```PUT /api/mov-almacenes/{id}``` - Actualizar movimiento
- ```DELETE /api/mov-almacenes/{id}``` - Eliminar movimiento
### Sol.EC.Consultas (Puerto 5005)
- ```GET /api/comprobantes``` - Obtener todos los comprobantes
- ```GET /api/comprobantes/{id}``` - Obtener comprobante por ID MongoDB
- ```GET /api/comprobantes/codigo/{codigo}``` - Obtener comprobante por código
- ```GET /api/comprobantes/cliente/{idCliente}``` - Obtener comprobantes por cliente
- ```POST /api/comprobantes``` - Crear comprobante
- ```PUT /api/comprobantes/{id}``` - Actualizar comprobante
- ```DELETE /api/comprobantes/{id}``` - Eliminar comprobante

## 📚 Swagger UI
Cada servicio expone su documentación Swagger en:
- Catalogo: http://localhost:5001/swagger
- Comprobantes: http://localhost:5002/swagger
- Saldos: http://localhost:5003/swagger
- Kardex: http://localhost:5004/swagger
- Consultas: http://localhost:5005/swagger

## 📋 Logs
Los logs se generan en la carpeta ```/logs``` de cada servicio con el formato:

- ```catalogo-YYYY-MM-DD.txt```
- ```comprobantes-YYYY-MM-DD.txt```
- ```saldos-YYYY-MM-DD.txt```
- ```kardex-YYYY-MM-DD.txt```
- ```consultas-YYYY-MM-DD.txt```

## 🧾 Datos de Prueba
Cada servicio incluye 5 registros de prueba con datos realistas:

### Clientes
1. Juan Pérez
2. María Luisa García
3. Carlos Rodríguez
4. Ana Martínez
5. Luis Fernández
### Artículos
1. Laptop HP Pavilion
2. Mouse Inalámbrico Logitech
3. Teclado Mecánico Razer
4. Monitor Samsung 24 pulgadas
5. Auriculares Sony WH-1000XM4

## 📈 Monitoreo

Para monitorear los servicios:
```bash
# Ver estado de contenedores
docker-compose ps

# Ver logs en tiempo real
docker-compose logs -f sol-ec-catalogo

# Ver uso de recursos
docker stats
```

## Desarrollo
### Estructura del Proyecto
```plaintext
Sol.EC/
├── Sol.EC.Catalogo/
│   ├── Models/
│   ├── Data/
│   ├── DTOs/
│   ├── Services/
│   ├── Dockerfile
│   └── .dockerignore
├── Sol.EC.Comprobantes/
├── Sol.EC.Saldos/
├── Sol.EC.Kardex/
├── Sol.EC.Consultas/
├── docker-compose.yml
└── README.md
```
### Agregar Nuevos Endpoints
1. Agregar método en el servicio correspondiente
2. Agregar endpoint en ```Program.cs```
3. Actualizar documentación Swagger
4. Probar con Docker Compose

## 💰 Métodos Especiales de Saldos
### Validar Saldo Suficiente
Endpoint: ```POST /api/saldos/validar```

Valida si un artículo tiene saldo suficiente para una cantidad específica.
### Request Body:
```json
{
  "idArticulo": 1,
  "cantidad": 50
}
```
### Response:
```json
{
  "idArticulo": 1,
  "cantidadSolicitada": 50,
  "tieneSaldoSuficiente": true
}
```
### Lógica:

- Si el artículo no existe → ```false```
- Si no hay saldo → ```false```
- Si saldo actual >= cantidad solicitada → ```true```
- Si saldo actual < cantidad solicitada → ```false```

### Restar Saldo
Endpoint: ```POST /api/saldos/restar```

Resta una cantidad del saldo existente de un artículo.
### Request Body:
```json
{
  "idArticulo": 1,
  "cantidad": 25
}
```
### Response:
```json
{
  "idArticulo": 1,
  "cantidadRestada": 25,
  "operacionExitosa": true
}
```
### Lógica:
- Si el artículo no existe → ```false```
- Si el saldo actual < cantidad a restar → ```false```
- Si el saldo actual >= cantidad a restar → Resta y retorna ```true```

### Sumar Saldo
Endpoint: ```POST /api/saldos/sumar```

Suma una cantidad al saldo existente de un artículo.

### Request Body:
```json
{
  "idArticulo": 1,
  "cantidad": 50
}
```
### Response:
```json
{
  "idSaldo": 1,
  "idArticulo": 1,
  "cantidad": 150
}
```
### Lógica:
- Si el artículo no existe → Crea nuevo saldo con la cantidad especificada
- Si el artículo existe → Suma la cantidad al saldo actual
- Retorna el saldo actualizado

## 🧯 Troubleshooting
### Problemas Comunes
1. Puerto en uso: Cambiar puertos en ```docker-compose.yml```
2. Base de datos no conecta: Verificar connection strings
3. Logs no aparecen: Verificar permisos de carpeta ```/logs```
4. Swagger no carga: Verificar que el servicio esté ejecutándose
5. MongoDB Authentication Error:
  -Para desarrollo local: ```Usar appsettings.Development.json``` (sin autenticación)
  -Para Docker: Usar ```appsettings.Production.json``` (con autenticación)
- Error: ```Command find requires authentication``` → Verificar credenciales en connection string
### Comandos Útiles
```bash
# Reconstruir imágenes
docker-compose build --no-cache

# Limpiar volúmenes
docker-compose down -v

# Ver logs específicos
docker logs sol-ec-catalogo

# Ejecutar comando en contenedor
docker exec -it sol-ec-catalogo bash
```

### Contribución

1. Fork el proyecto
2. Crear branch para feature (```git checkout -b feature/nueva-funcionalidad```)
3. Commit cambios (```git commit -am 'Agregar nueva funcionalidad'```)
4. Push al branch (```git push origin feature/nueva-funcionalidad```)
5. Crear Pull Request
