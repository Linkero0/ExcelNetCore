using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
  
   public class EstudentController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new List<EstudentModel>());
        }
        [HttpPost]
        public IActionResult Index(IFormCollection form)
        {



            List<EstudentModel> users = new List<EstudentModel>();
            var fileName = "D:/Calificaciones.xlsx";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                var reader = ExcelReaderFactory.CreateReader(stream);

                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
                    }
                };
                var dataSet = reader.AsDataSet(conf);
                var dataTable = dataSet.Tables[0];

                double mayor = Int32.Parse(dataTable.Rows[1][6].ToString());
                double menor = Int32.Parse(dataTable.Rows[1][6].ToString());
                double promedio = 0;

                for (var i = 0; i < dataTable.Rows.Count; i++)
                {

                    if (mayor > Double.Parse(dataTable.Rows[i][6].ToString())) mayor = mayor; else mayor = Double.Parse(dataTable.Rows[i][6].ToString());
                    if (menor < Double.Parse(dataTable.Rows[i][6].ToString())) menor = menor; else menor = Double.Parse(dataTable.Rows[i][6].ToString());

                    promedio += Double.Parse(dataTable.Rows[i][6].ToString());

                    users.Add(new EstudentModel
                    {
                        Nombres = dataTable.Rows[i][0].ToString(),
                        ApellidoPaterno = dataTable.Rows[i][1].ToString(),
                        ApellidoMaterno = dataTable.Rows[i][2].ToString(),
                        FechaNacimiento = dataTable.Rows[i][3].ToString(),
                        Grado = dataTable.Rows[i][4].ToString(),
                        Grupo = dataTable.Rows[i][5].ToString(),
                        Calificacion = dataTable.Rows[i][6].ToString(),
                        ClaveUsuario = generatePass(dataTable.Rows[i][0].ToString(), dataTable.Rows[i][2].ToString())+CalculaEdad(dataTable.Rows[i][3].ToString()).ToString()
                }); ;


                }

               
                ViewData["mejorCalificacion"] = mayor.ToString();
                ViewData["peorCalificación"] = menor.ToString();
                ViewData["promedio"] = (promedio / dataTable.Rows.Count).ToString();
               




            }
            return View(users);
        }
   
      
        public string generatePass(string nombre,string apellido)
        {
                               //012345678901234567890123456
            char[] letras = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ".ToCharArray();
            string debug = "";
            char[] arrNom = (nombre.Substring(0,2)+apellido.Substring(0,2)).ToUpper().ToCharArray(0, 4);
            
            for (int i = 0; i < arrNom.Length; i++)
            {
                int indice = Array.IndexOf(letras, arrNom[i]);

                if (indice == -1) continue;
                if (indice > 2) arrNom[i] = letras[indice - 3];
                if (indice < 2) arrNom[i] = letras[letras.Length -indice - 3];
            }

            for (int i = 0; i < arrNom.Length; i++)
            {

                debug = arrNom[i] + debug;
            }

            
           // string edad  = CalculaEdad(cumpleaños).ToString();
        
           return debug;
        }


        public int CalculaEdad(string fecha_nac)
        {
          
            DateTime fechaNacimiento = new DateTime(Convert.ToInt32(fecha_nac.Substring(6, 4)), Convert.ToInt32(fecha_nac.Substring(3, 2)),Convert.ToInt32(fecha_nac.Substring(0, 2)));

            DateTime now = DateTime.Today;
            int edad = DateTime.Today.Year - fechaNacimiento.Year;
            if (DateTime.Today < fechaNacimiento.AddYears(edad))

                edad--;
         
         
            return edad;

             }


      

    }

   

}
