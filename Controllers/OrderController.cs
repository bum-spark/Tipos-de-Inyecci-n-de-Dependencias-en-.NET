using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderSystem;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public readonly IOrderServiceScoped _orderScoped;
        public readonly IOrderServiceTransient _orderTransient;
        public readonly IOrderServiceSingleton _orderSingleton;

        public OrderController(IOrderServiceScoped orderScoped, IOrderServiceTransient orderTransient, IOrderServiceSingleton orderSingleton)
        {
            _orderScoped = orderScoped;
            _orderTransient = orderTransient;
            _orderSingleton = orderSingleton;
        }

        [HttpGet("transient/orders")]
        public IActionResult obtenerOrdenes()
        {
            return Ok(new
            {
                id = _orderTransient.GetInstanceId(),
                ordersCount = _orderTransient.GetOrdersCount(),
                orders = _orderTransient.GetOrders()
            });
        }

        [HttpPost("transient/orders")]
        public IActionResult AgregarOrdenTransient([FromBody] Order order)
        {
            _orderTransient.AddOrder(order);
            return Ok(new
            {
                mensaje = "Orden agregada a Transient",
                instanceId = _orderTransient.GetInstanceId()
            });
        }

        [HttpGet("scoped/orders")]
        public IActionResult ObtenerOrdenesScoped()
        {
            return Ok(new
            {
                id = _orderScoped.GetInstanceId(),
                ordersCount = _orderScoped.GetOrdersCount(),
                orders = _orderScoped.GetOrders()
            });
        }

        [HttpPost("scoped/orders")]
        public IActionResult AgregarOrdenScoped([FromBody] Order order)
        {
            _orderScoped.AddOrder(order);
            return Ok(new
            {
                mensaje = "Orden agregada a Scoped",
                instanceId = _orderScoped.GetInstanceId()
            });
        }

        [HttpGet("singleton/orders")]
        public IActionResult ObtenerOrdenesSingleton()
        {
            return Ok(new
            {
                id = _orderSingleton.GetInstanceId(),
                ordersCount = _orderSingleton.GetOrdersCount(),
                orders = _orderSingleton.GetOrders()
            });
        }

        [HttpPost("singleton/orders")]
        public IActionResult AgregarOrdenSingleton([FromBody] Order order)
        {
            _orderSingleton.AddOrder(order);
            return Ok(new
            {
                gui = _orderSingleton.GetInstanceId(), 
                mensaje = "Orden agregada a Singleton",
                instanceId = _orderSingleton.GetInstanceId()
            });
        }

        
    }
}
