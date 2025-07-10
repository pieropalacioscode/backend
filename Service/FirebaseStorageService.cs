using Firebase;
using Firebase.Storage;
using FirebaseAdmin.Auth;
using IService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private FirebaseStorage _firebaseStorage;

        public FirebaseStorageService(IConfiguration configuration)
        {
            InitializeFirebaseAsync(configuration).GetAwaiter().GetResult();
        }

        private async Task InitializeFirebaseAsync(IConfiguration configuration)
        {
            var firebaseApp = await FirebaseAppManager.GetInstanceAsync();
            var bucket = configuration["Firebase:StorageBucket"];

            _firebaseStorage = new FirebaseStorage(bucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = async () =>
                {
                    var firebaseToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync("generic-uid");
                    return firebaseToken;
                }
            });
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (_firebaseStorage == null)
            {
                throw new InvalidOperationException("Firebase Storage no se ha inicializado correctamente.");
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            using (var stream = file.OpenReadStream())
            {
                var task = _firebaseStorage
                    .Child(folderName)
                    .Child(fileName)
                    .PutAsync(stream);

                var url = await task;
                return url;
            }
        }

        public async Task<string> UploadPedidosImageAsync(IFormFile image)
        {
            return await UploadFileAsync(image, "pedidosimagenes");
        }

        public Task DeleteFileAsync(string fileUrl)
        {
            throw new NotImplementedException();
        }


        public async Task<string> UploadPedidoPdfTemporalAsync(byte[] pdfBytes, string fileNameWithoutExtension)
        {
            if (_firebaseStorage == null)
            {
                throw new InvalidOperationException("Firebase Storage no se ha inicializado correctamente.");
            }

            var nombreEmpresa = "Libreria Saber SAC";
            var nombreLimpio = nombreEmpresa
                .ToLowerInvariant()
                .Replace(" ", "_")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("-", "")
                .Replace("ñ", "n")
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u");

            var fechaHoy = DateTime.Now.ToString("yyyyMMdd");
            var nombreArchivo = $"Pedido_{nombreLimpio}_{fechaHoy}_{Guid.NewGuid()}.pdf";

            var folderName = "pedidospdf";

            using (var stream = new MemoryStream(pdfBytes))
            {
                var task = _firebaseStorage
                    .Child(folderName)
                    .Child(nombreArchivo)
                    .PutAsync(stream);

                var url = await task;

                // 🔥 Programar eliminación después de 5 minutos
                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromMinutes(20));
                    try
                    {
                        await _firebaseStorage
                            .Child(folderName)
                            .Child(nombreArchivo)
                            .DeleteAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error eliminando el PDF temporal: {ex.Message}");
                    }
                });

                return url;
            }
        }

    }
}
