"# Tipos de Inyección de Dependencias en .NET

Este es un proyecto Web API en .NET 8 que demuestra los tres tipos de ciclos de vida de dependencias: **Transient**, **Scoped** y **Singleton**. El proyecto implementa un sistema de gestión de pedidos (`OrderSystem`) donde cada tipo de inyección permite observar cómo el ciclo de vida afecta directamente el comportamiento de la aplicación.

---

## Implementación de los Servicios

### 1. Modelo de Datos (`Order`)

El modelo `Order` representa un pedido con dos propiedades:
```csharp
public class Order
{
    public string Articulo { get; set; }
    public int Cantidad { get; set; }
}
```

### 2. Interfaces del Servicio

Creé **tres interfaces separadas** para demostrar cada ciclo de vida:

**Archivo: `Services/IOrderService.cs`**
```csharp
public interface IOrderServiceTransient
{
    void AddOrder(Order order);
    List<Order> GetOrders();
    Guid GetInstanceId();
    int GetOrdersCount();
}

public interface IOrderServiceScoped
{
    void AddOrder(Order order);
    List<Order> GetOrders();
    Guid GetInstanceId();
    int GetOrdersCount();
}

public interface IOrderServiceSingleton
{
    void AddOrder(Order order);
    List<Order> GetOrders();
    Guid GetInstanceId();
    int GetOrdersCount();
}
```

Cada interfaz tiene los mismos métodos porque la funcionalidad es idéntica; lo que cambia es el ciclo de vida.

### 3. Implementación del Servicio (`OrderService`)

**Archivo: `Services/OrderService.cs`**

La clase `OrderService` implementa las tres interfaces simultáneamente:

```csharp
public class OrderService : IOrderServiceTransient, IOrderServiceScoped, IOrderServiceSingleton
{
    private readonly Guid _instanceId;
    private readonly List<Order> _orders;

    public OrderService()
    {
        _instanceId = Guid.NewGuid(); 
        _orders = new List<Order>();  
    }

    public void AddOrder(Order order) /* Lógica del servicio */
    public List<Order> GetOrders() /* Lógica del servicio */
    public Guid GetInstanceId() /* Lógica del servicio */
    public int GetOrdersCount() /* Lógica del servicio */
}
```

**¿Por qué generamos un Guid?**  
Para identificar cada instancia del servicio. Si dos peticiones tienen el mismo Guid, significa que están usando la misma instancia.

### 4. Registro de Servicios en `Program.cs`

**Archivo: `Program.cs`**

El mismo servicio `OrderService` se registra **tres veces** con diferentes ciclos de vida:

```csharp
builder.Services.AddTransient<IOrderServiceTransient, OrderService>();
builder.Services.AddScoped<IOrderServiceScoped, OrderService>();
builder.Services.AddSingleton<IOrderServiceSingleton, OrderService>();
```

Esto significa que se crearan instancias independientes, osea que cuando se manden a llamar, no tendran el mismo ciclo de vida.

### 5. Controlador con Endpoints

**Archivo: `Controllers/OrderController.cs`**

El controlador inyecta los **tres servicios simultáneamente** para poder compararlos:

```csharp
[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderServiceScoped _orderScoped;
    private readonly IOrderServiceTransient _orderTransient;
    private readonly IOrderServiceSingleton _orderSingleton;

    public OrderController(IOrderServiceScoped orderScoped, 
                          IOrderServiceTransient orderTransient, 
                          IOrderServiceSingleton orderSingleton)
    {
        _orderScoped = orderScoped;
        _orderTransient = orderTransient;
        _orderSingleton = orderSingleton;
    }
    
    // Endpoints implementados...
}
```

### Endpoints Implementados:

**GET** => `/api/order/transient` (Obtiene pedidos del servicio Transient)

**POST** => `/api/order/transient` (Agrega un pedido al servicio Transient)

**GET** => `/api/order/scoped` (Obtiene pedidos del servicio Scoped)

**POST** => `/api/order/scoped` (Agrega un pedido al servicio Scoped)

**GET** => `/api/order/singleton` (Obtiene pedidos del servicio Singleton)

**POST** => `/api/order/singleton` (Agrega un pedido al servicio Singleton)

---

## Comportamiento Observado en las Pruebas

Realicé pruebas con Postman para cada endpoint y estos fueron los resultados:

### **1. Transient**

**Prueba 1:** Agregar un pedido
```json
POST http://localhost:5000/api/order/transient
Body: { "articulo": "Laptop", "cantidad": 1 }

Respuesta:
{
    "mensaje": "Orden agregada a Transient",
    "instanceId": "a1b2c3d4-5678-90ab-cdef-1234567890ab"
}
```

**Prueba 2:** Obtener pedidos inmediatamente después
```json
GET http://localhost:5000/api/order/transient

Respuesta:
{
    "id": "e5f6g7h8-9012-34ij-klmn-567890abcdef",  ← ID DIFERENTE
    "ordersCount": 0,     ← LISTA VACÍA
    "orders": []
}
```

**Observación:**  
- El `instanceId` es diferente en cada petición
- La lista de pedidos NO se mantiene porque cada llamada crea una nueva instancia
- El contador siempre vuelve a 0

**Conclusión:**  
Transient crea una instancia completamente nueva cada vez que el servicio es llamado.

---

### **2. Scoped**

**Prueba 1:** Agregar un pedido
```json
POST http://localhost:5000/api/order/scoped
Body: { "articulo": "Mouse", "cantidad": 2 }

Respuesta:
{
    "mensaje": "Orden agregada a Scoped",
    "instanceId": "11111111-2222-3333-4444-555555555555"
}
```

**Prueba 2:** Obtener pedidos en la misma petición
- Si llamáramos a otro endpoint en la misma petición, tendríamos el mismo instanceId
- Pero entre peticiones HTTP diferentes, el ID si cambia

**Prueba 3:** Obtener pedidos en otra petición
```json
GET http://localhost:5000/api/order/scoped

Respuesta:
{
    "id": "66666666-7777-8888-9999-000000000000",  ← ID DIFERENTE
    "ordersCount": 0,                                ← LISTA VACÍA
    "orders": []
}
```

**Observación:**  
- El `instanceId` cambia entre peticiones 
- Pero por ejemplo si se llama la misma instancia más de una vez en la misma peticion, los datos se mantienen
- La lista de pedidos se reinicia con cada nueva petición

**Conclusión:**  
Scoped mantiene la misma instancia durante toda una petición, pero crea una nueva para cada petición diferente. 

---

### **3. Singleton**

**Prueba 1:** Agregar primer pedido
```json
POST http://localhost:5000/api/order/singleton
Body: { "articulo": "Teclado", "cantidad": 1 }

Respuesta:
{
    "gui": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
    "mensaje": "Orden agregada a Singleton",
    "instanceId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
}
```

**Prueba 2:** Agregar segundo pedido (otra petición)
```json
POST http://localhost:5000/api/order/singleton
Body: { "articulo": "Monitor", "cantidad": 1 }

Respuesta:
{
    "gui": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",  ← MISMO ID
    "mensaje": "Orden agregada a Singleton",
    "instanceId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
}
```

**Prueba 3:** Obtener todos los pedidos
```json
GET http://localhost:5000/api/order/singleton

Respuesta:
{
    "id": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",  ← MISMO ID
    "ordersCount": 2,                               ← ACUMULADO
    "orders": [
        { "articulo": "Teclado", "cantidad": 1 },
        { "articulo": "Monitor", "cantidad": 1 }
    ]
}
```

**Observación:**  
- El `instanceId` no cambia
- La lista de pedidos se mantiene y acumula entre todas las peticiones
- El contador incrementa con cada pedido agregado

**Conclusión:**  
Singleton crea una única instancia al iniciar la aplicación y la reutiliza para siempre. Los datos persisten durante toda la vida de la aplicación. 

---

## Escenarios de Uso en Proyectos Reales

Basándome en el comportamiento observado en este proyecto, aquí están los casos de uso que pensará apropiados:

### **Cuándo usar Transient**

**Casos apropiados:**
- **Servicios sencillos:** Validadores de datos, calculadoras, conversores de datos
- **Generación de identificadores únicos**: servicios que generan un ID (GUID, token de sesión temporal, etc.) para cada operación nueva. 
- **Servicios que no deben compartir información:** Cada operación necesita su propio contexto

---

### **Cuándo usar Scoped**

**Casos apropiados:**
- **Contextos de base de datos:** todas las operaciones CRUD dentro de una misma solicitud usan la misma instancia del contexto
- **Servicios que necesitan consistencia durante una petición HTTP:** Usuario autenticado, transacciones
- **Servicios de carrito de compras:** mientras el usuario realiza una petición completa (como confirmar una compra), los cálculos y validaciones comparten la misma instancia temporalmente.

---

### **Cuándo usar Singleton**

**Casos apropiados:**
- **Configuraciones globales:** Settings que no cambian, configuración predefinidas
- **Estadísticas globales:** Si se desara un aestadística actual de uso de algun lado, podría funcionar

---

### Diagramas: Ciclo de Vida según las Peticiones

```
TRANSIENT
════════════════════════════════════════════════════════════
```

<img width="463" height="408" alt="trasient" src="https://github.com/user-attachments/assets/ebcefbad-440a-4c19-aeb7-e9a6e91861ea" />

```
Los pedidos NO persisten entre llamadas


SCOPED 
════════════════════════════════════════════════════════════
```

<img width="491" height="425" alt="scoped" src="https://github.com/user-attachments/assets/cff48593-1b01-49bf-bb24-b615e6b816f2" />

```
Misma instancia EN la petición, nueva ENTRE peticiones


SINGLETON 
════════════════════════════════════════════════════════════
```

<img width="507" height="422" alt="singleton" src="https://github.com/user-attachments/assets/2dc95812-97b6-40f5-8494-2dea82f81add" />

```
SIEMPRE la misma instancia, TODOS los datos persisten
```
---

## Conclusiones del Proyecto

### Lo que entendí:

1. **Transient es seguro pero ineficiente para datos persistentes**
   - Cada llamada crea una nueva instancia
   - No sirve para mantener estado
   - Ideal para operaciones aisladas, que no dependen de otros datos entre peticiones

2. **Scoped es perfecto para operaciones de base de datos**
   - Una instancia por petición HTTP garantiza consistencia
   - Evita problemas de concurrencia entre usuarios

3. **Singleton es peligroso para datos de usuario**
   - TODO se comparte entre TODOS los usuarios
   - Perfecto para configuración 

---

**Autor:** Jordan Cazares

**Fecha:** Octubre 2025  

**Materia:** Sistemas Propietarios  

**Institución:** ITSES

**Repositorio:** https://github.com/bum-spark/Tipos-de-Inyecci-n-de-Dependencias-en-.NET" 
