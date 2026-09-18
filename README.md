# Payment Orders

Esqueleto académico de una API de órdenes de pago en .NET 8, C#, Entity Framework Core y SQLite local. La solución aplica Clean Architecture:

```text
Api → Application → Domain ← Infrastructure
```

La API, persistencia, reglas de dominio, estados e idempotencia están cableadas. Los patrones Factory, Builder y Strategy + Factory permanecen intencionalmente incompletos para que el estudiante los implemente.

## Actividad en dos clases

La actividad está diseñada para estudiantes de quinto semestre y se resuelve en dos sesiones. El trabajo se limita a los patrones creacionales pendientes: la persistencia local, los endpoints y la arquitectura ya están entregados.

- Clase 1: comprender la solución y completar Factory y Builder.
- Clase 2: completar Strategy + Factory, ejecutar pruebas y preparar la explicación individual.

Pueden usar inteligencia artificial para entender conceptos, revisar errores y formular preguntas. Cada estudiante debe escribir e integrar su propia solución; no puede sustituirla por una respuesta completa generada por IA ni modificar las pruebas.

La actividad tiene doble nota, con el mismo peso:

- 50%: implementación funcional del equipo y cumplimiento de las pruebas.
- 50%: sustentación individual de las decisiones de diseño.

Las comisiones que debe calcular el ejercicio son: nacional 1%, internacional 3% y programada 0.5%. Redondea el resultado a dos decimales.

Consulta la [guía de la actividad en dos clases](docs/actividad-2-clases.md) antes de comenzar.

## Estructura

```text
src/
  PaymentOrders.Api/             REST, Swagger y composición
  PaymentOrders.Application/     Casos de uso, DTOs y puertos
  PaymentOrders.Domain/          Agregado, invariantes y ejercicios
  PaymentOrders.Infrastructure/  EF Core, SQLite y repositorios
tests/
  PaymentOrders.Domain.Tests/        Invariantes y estados
  PaymentOrders.ArchitectureTests/  Dependencias prohibidas de Domain
  PaymentOrders.ContractTests/      Muestras públicas que fallan hasta completar ejercicios
migrations/                      Nota sobre la base de datos local
docs/                            Diseño y plan de implementación
```

## Ejecución local

No necesitas Docker ni PostgreSQL. La API usa SQLite local y aplica las migraciones de EF Core al iniciar.

```powershell
dotnet tool restore
dotnet ef database update --project src/PaymentOrders.Infrastructure/PaymentOrders.Infrastructure.csproj --startup-project src/PaymentOrders.Api/PaymentOrders.Api.csproj
dotnet run --project src/PaymentOrders.Api/PaymentOrders.Api.csproj
```

Swagger queda disponible en `http://localhost:5205/swagger`.

Para crear una migración después de cambiar el modelo:

```powershell
dotnet ef migrations add <NombreDescriptivo> --project src/PaymentOrders.Infrastructure/PaymentOrders.Infrastructure.csproj --startup-project src/PaymentOrders.Api/PaymentOrders.Api.csproj --output-dir Persistence/Migrations
```

## MySQL opcional

Si el equipo tiene MySQL instalado o accesible, puede usarlo como proveedor alternativo. No requiere Docker, pero sí una instancia MySQL disponible.

Configura el proveedor y la conexión en la misma sesión de PowerShell:

```powershell
$env:Database__Provider = "MySql"
$env:ConnectionStrings__PaymentOrders = "Server=localhost;Port=3306;Database=payment_orders;User=root;Password=tu_clave"
dotnet ef database update --project src/PaymentOrders.Infrastructure.MySqlMigrations/PaymentOrders.Infrastructure.MySqlMigrations.csproj --startup-project src/PaymentOrders.Api/PaymentOrders.Api.csproj
dotnet run --project src/PaymentOrders.Api/PaymentOrders.Api.csproj
```

La migración MySQL vive en `src/PaymentOrders.Infrastructure.MySqlMigrations`. Para volver a SQLite, cierra la sesión de PowerShell o ejecuta `$env:Database__Provider = "Sqlite"` y usa la migración del proyecto `PaymentOrders.Infrastructure`.

## Endpoints

- `POST /api/payment-orders` — requiere el encabezado `Idempotency-Key`.
- `GET /api/payment-orders/{orderId}` — consulta una orden.
- `GET /api/payment-orders` — filtra por `orderType`, `status`, fecha y paginación.
- `POST /api/payment-orders/{orderId}/cancel` — cancela solo desde `Pending`.
- `POST /api/payment-orders/{orderId}/process` — lleva una orden de `Created` a `Pending`, `Processing` y `Completed`.
- `GET /api/payment-orders/{orderId}/events` — consulta la auditoría inmutable.

## Archivos que debe completar el estudiante

No modifiques los contratos públicos ni los tests. Implementa solamente los cuerpos marcados como ejercicio en:

- `src/PaymentOrders.Domain/Orders/Patterns/PaymentOrderFactory.cs`
- `src/PaymentOrders.Domain/Orders/Patterns/PaymentOrderBuilder.cs`
- `src/PaymentOrders.Domain/Orders/Patterns/CommissionStrategies.cs`
- `src/PaymentOrders.Domain/Orders/Patterns/CommissionStrategyFactory.cs`

Los contratos de muestra en `PaymentOrders.ContractTests` fallan hoy de forma deliberada hasta que estos patrones funcionen. Es una muestra pública reducida: la evaluación real incorpora una suite privada.

## Pruebas

Ejecuta toda la solución para observar el estado completo, incluidos los tres contratos pendientes:

```powershell
dotnet test PaymentOrders.slnx --no-restore
```

Mientras los ejercicios sigan pendientes, ejecuta las pruebas ya implementadas sin ocultar ni modificar los contratos:

```powershell
dotnet test PaymentOrders.slnx --no-restore --filter "FullyQualifiedName!~CreationalPatternContractSamples"
```

## Guía para la defensa técnica

Documenta tus respuestas aquí antes de la defensa:

### Por qué Factory

Usamos Factory para que las otras partes del programa no tengan que saber qué clase específica deben crear dependiendo del tipo de orden. 
Entonces dependiendo del tipo de orden, la Factory se encarga de tomar esa decisión y entregar el Builder especifico para ese tipo.
De esta forma Application y Api no tienen que estar preguntando constantemente qué tipo de orden es ni crear directamente las clases especificas.
Si quisieramos agregar un nuevo tipo de orden tendríamos que agregar el nuevo tipo en OrderType y actualizar la Factory para que sepa qué hacer con ese nuevo tipo.

### Por qué Builder

Usamos Builder porque una orden tiene varios datos que necesitamos configurar antes de poder crearla. Entonces en lugar de crear la orden de una sola vez con muchos datos, podemos ir agregándolos paso a paso:
Ej: .WithId(), .WithSourceAccount(...) etc. Y finalmente se usa .Build() para revisar que estén los datos necesarios y después se crea la orden correspondiente.

### Qué ocurriría sin ellos

Sin estos patrones, probablemente tendríamos que poner en otras partes del programa decisiones como dependiendo del tipo de orden crear esta clase o crear esta otra
Eso haría que Application o Api tuvieran que conocer las clases concretas e involuvrarse demas en la creacion de estas.
También podríamos terminar repitiendo estas mismas decisiones en diferentes lugares.
Con estos patrones dejamos la toma de esas desiciones de una forma más organizada y es más fácil cambiar o probar el código.

### Dónde viven las reglas de negocio

Las reglas principales están dentro de PaymentOrder. Por ejemplo:
El monto debe ser mayor que 0 y no superar 1.000.000.
La cuenta origen y la cuenta destino deben ser diferentes.
Una orden internacional necesita un código SWIFT válido.
Una orden programada necesita una fecha futura.

### Cómo se garantiza idempotencia

La idempotencia sirve para que si llega dos veces la misma solicitud, no terminemos creando dos veces la misma operación.
El proyecto utiliza idempotency_records con una clave única para identificar una solicitud que ya fue procesada.
Entonces, antes de procesar una solicitud, se revisa si ya existe ese registro.
En caso de que existiera previamente se maneja como una solicitud repetida y no se vuelve a procesar de la misma manera.

### Cómo se protegen las transiciones de estado

Una orden no puede cambiar de cualquier estado a cualquier otro pare esto en PaymentOrder se controla qué cambio está permitido. Por ejemplo:
Created → Pending
Pending → Processing etc,
Si intentamos hacer un cambio que no corresponde al estado actual, PaymentOrder lanza una excepción y rechaza el cambio.

