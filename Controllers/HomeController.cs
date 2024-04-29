using System;
using System.Diagnostics;
using System.IO.Ports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Port.Models;
using Microsoft.AspNetCore.SignalR;
using SignalR;
using Microsoft.Extensions.Logging;
namespace Port.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SerialPort _serialPort;

        private readonly IHubContext<MiHub> _hubContext;

        public HomeController(ILogger<HomeController> logger, IHubContext<MiHub> hubContext)
        {
            _hubContext = hubContext;
            _logger = logger;
            _serialPort = new SerialPort("COM3", 9600); // Cambiar el puerto y el baud rate según tu configuración
        }

        public IActionResult Index()
        {
            var model = new EspDataViewModel();

          _serialPort.DataReceived += async (sender, e) =>
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting(); // Leer los datos recibidos
            _logger.LogInformation("Datos recibidos: " + data);

             // Enviar datos a través de SignalR
            await _hubContext.Clients.All.SendAsync("SendData", data);
        };
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
                    

            _serialPort.Open(); // Abrir el puerto serie

            // Esperar hasta que se cierre la aplicación para cerrar el puerto serie
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => _serialPort.Close();

            // Esperar hasta que se cierre la aplicación para cerrar el puerto serie
            Console.CancelKeyPress += (sender, e) => _serialPort.Close();

            return View(model);
            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
