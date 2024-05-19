using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WEB_GAME.Models;


namespace WEB_GAME.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://www.guayabagame.somee.com/api/"); // Reemplaza con la URL de tu API publicada
        }
       
        // GET: /Login
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Por favor, ingresa el nombre de usuario y la contraseña");
                return View("Index");
            }

            var response = await _httpClient.GetAsync($"Users?nameUser={username}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<UserViewModel> users = JsonConvert.DeserializeObject<List<UserViewModel>>(content);
                var user = users.FirstOrDefault(u => u.NameUser.Equals(username, StringComparison.OrdinalIgnoreCase));

                if (user != null)
                {
                    // Verificar si el usuario está activo
                    if (!user.Active)
                    {
                        ModelState.AddModelError(string.Empty, "Usuario inactivo. Por favor, contacte con el administrador.");
                        return View("Index");
                    }

                    var encryptedPassword = EncryptPassword(password);

                    if (user.Password == encryptedPassword)
                    {
                        // Almacenar el nombre de usuario en la sesión
                        HttpContext.Session.SetString("Username", user.NameUser);

                        // Almacenar si el usuario es admin en la sesión
                        bool isAdmin = user.NameUser.Equals("admin", StringComparison.OrdinalIgnoreCase);
                        HttpContext.Session.SetString("IsAdmin", isAdmin.ToString());
                        return RedirectToAction("Index", "MainMenu");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrecta");
                        return View("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "El nombre de usuario no existe. ¿Deseas registrarte?");
                    return View("Index");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al consultar la API");
                return View("Index");
            }
        }




        // GET: /Login/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Login/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegistroViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verifica si el nombre de usuario ya existe
                var response = await _httpClient.GetAsync($"Users?nameUser={model.NameUser}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    List<UserViewModel> users = JsonConvert.DeserializeObject<List<UserViewModel>>(content);
                    var existingUser = users.FirstOrDefault(u => u.NameUser.Equals(model.NameUser, StringComparison.OrdinalIgnoreCase));

                    if (existingUser != null)
                    {
                        ModelState.AddModelError(string.Empty, "El nombre de usuario ya existe. Por favor, elige otro nombre de usuario.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al consultar la API para la validación del nombre de usuario");
                    return View(model);
                }

                // Encripta la contraseña antes de enviarla a la API
                model.Password = EncryptPassword(model.Password);

                // Si el nombre de usuario no existe, procede con el registro
                DateTime fechaHoraActual = DateTime.Now;
                string fechaHoraFormateada = fechaHoraActual.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                model.RegistrationDate = DateTime.Parse(fechaHoraFormateada);
                var jsonContent = JsonConvert.SerializeObject(model);
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                response = await _httpClient.PostAsync("Users", contentString);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al registrar el usuario");
                }
            }
            return View(model);
        }
        private string EncryptPassword(string password)
        {
            // Esta es una implementación básica de encriptación, se recomienda mejorarla para un entorno de producción
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        // POST: /Login/Logout
        [HttpPost]
        public IActionResult Logout()
        {
            // Eliminar los datos de la sesión
            HttpContext.Session.Clear();

            // Redirigir al usuario de vuelta a la página de inicio de sesión
            return RedirectToAction("Index", "Login");
        }



        public string GetLoggedUsername()
        {
            // Suponiendo que tienes la lógica para obtener el nombre de usuario
            var username = "nombreDeUsuario"; // Aquí debes obtener el nombre de usuario de alguna manera
            return username;
        }
    }

}

#region Desencriptar
//public class PasswordEncryption
//{
//    private const string Key = "YourEncryptionKey"; // Clave secreta para la encriptación (asegúrate de guardarla de forma segura)

//    public static string Encrypt(string plainText)
//    {
//        using (Aes aesAlg = Aes.Create())
//        {
//            aesAlg.Key = Encoding.UTF8.GetBytes(Key);
//            aesAlg.IV = new byte[16]; // Vector de inicialización (IV) predeterminado para simplificar

//            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
//            byte[] encryptedBytes = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(plainText), 0, plainText.Length);
//            return Convert.ToBase64String(encryptedBytes);
//        }
//    }

//    public static string Decrypt(string cipherText)
//    {
//        using (Aes aesAlg = Aes.Create())
//        {
//            aesAlg.Key = Encoding.UTF8.GetBytes(Key);
//            aesAlg.IV = new byte[16]; // Vector de inicialización (IV) predeterminado para simplificar

//            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
//            byte[] cipherBytes = Convert.FromBase64String(cipherText);
//            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
//            return Encoding.UTF8.GetString(decryptedBytes);
//        }
//    }
//}

//class Program
//{
//    static void Main(string[] args)
//    {
//        string originalPassword = "Password123";

//        // Encriptar la contraseña
//        string encryptedPassword = PasswordEncryption.Encrypt(originalPassword);
//        Console.WriteLine($"Contraseña encriptada: {encryptedPassword}");

//        // Desencriptar la contraseña
//        string decryptedPassword = PasswordEncryption.Decrypt(encryptedPassword);
//        Console.WriteLine($"Contraseña desencriptada: {decryptedPassword}");
//    }
//}
//Este ejemplo utiliza el algoritmo AES para encriptar y desencriptar una cadena. 
//    Sin embargo, ten en cuenta que almacenar contraseñas desencriptadas en tu 
//    sistema introduce riesgos de seguridad y no se recomienda. Siempre es mejor 
//    almacenar solo hashes irreversibles de las contraseñas.
#endregion
