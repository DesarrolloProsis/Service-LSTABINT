﻿using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Threading.Tasks;
using LSTABINT_SERV.Model;
using Telegram.Bot;
using Ionic.Zip;
using System.Threading;


namespace LSTABINT_SERV
{
    public partial class LSTABINTSERVICE : ServiceBase
    {
        private System.Timers.Timer tmGenera = null;
        private GTDBEntities1 db = new GTDBEntities1();
        string[] TamañoLista = new string[2];

        public LSTABINTSERVICE()
        {
            InitializeComponent();
        }
        public void Inicio() //Evento para probar desde Windows Form
        {
            tmGenera = new System.Timers.Timer
            {
                Interval = 10000 //10 segundos
            };
            tmGenera.Elapsed += new System.Timers.ElapsedEventHandler(TmGenera_Elapsed);
            tmGenera.Enabled = true;
            tmGenera.Start();
        }
        protected override void OnStart(string[] args)
        {
            tmGenera = new System.Timers.Timer
            {
                Interval = 300000 //5 minutos = 300 segundos
            };
            tmGenera.Elapsed += new System.Timers.ElapsedEventHandler(TmGenera_Elapsed);
            tmGenera.Enabled = true;
            tmGenera.Start();
        }

        protected override void OnStop()
        {
            File.WriteAllText(@"C:\temporal\LSTABINTSERVStopped.txt", "Se detuvo");
            SendMessage("LSTABINT Service se detuvo");
        }

        private void TmGenera_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmGenera.Enabled = false;
            tmGenera.Stop();

            var task = Task.Run(() => GeneraArchivo());
            if (!task.Wait(TimeSpan.FromMinutes(5)))
                SendMessage("La generación de una LSTABINT tardó demasiado, se recomienda checar la conexión SQL");
        }
        public bool Conexion()
        {
            try
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send("10.1.10.111");
                //PingReply pingReply = ping.Send("192.168.0.69");
                if (pingReply.Status == IPStatus.Success)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private void GeneraArchivo()
        {
            try
            {
                if (!Conexion())
                {
                    SendMessage("La conexión al servidor ha fallado");
                }
                else
                {
                    foreach (var item in Directory.GetFiles(@"C:\temporal\", "LSTABINT.*"))
                    {
                        File.Delete(item);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        bool Validado = false;
                        Parametrizables Parametros = new Parametrizables();
                        variables varparametros = new variables();

                        Parametros = GetParametros(i);
                        CreateLSTABINT(Parametros, i);
                        Validado = Validaciones(Parametros, i);

                        if (Validado)
                        {
                            db.HistoricoListas.Add(new HistoricoListas
                            {
                                Fecha_Creacion = DateTime.Now,
                                Tamaño = TamañoLista[i] + " KB",
                                Extension = Parametros.extension.ToString("D3"),
                                Tipo = i == 0 ? "MultiModal" : "Exclusivo"
                            });
                            var listas = db.Parametrizables.ToArray();
                            listas[i].extension++;
                            if (listas[i].extension > 999)
                            {
                                listas[i].extension = 1;
                            }
                        }
                        else
                        {
                            SendMessage("No coincide el número de tags en listas con el número de Tags existentes");
                        }
                    }

                    db.SaveChanges();
                }

                tmGenera.Enabled = true;
                tmGenera.Start();
            }
            catch (Exception Ex)
            {
                string ErrorPath = @"C:\temporal\Errores\LSTABINT\";
                string[] ArrayFecha = DateTime.Now.ToString("dd MMMM yyyy").Split(' ');
                ErrorPath += ArrayFecha[2] + @"\" + ArrayFecha[1] + @"\" + ArrayFecha[0] + @"\";
                CreateDirectory(ErrorPath);

                File.WriteAllText(ErrorPath + "Error" + db.Parametrizables.FirstOrDefault().extension + ".txt", Ex.Message + Ex.StackTrace);
                SendMessage("Ocurrió un error en la generación de la LSTABINT." + db.Parametrizables.FirstOrDefault().extension + ": " + Ex.Message);

                tmGenera.Enabled = true;
                tmGenera.Start();
            }

        }

        public Parametrizables GetParametros(int i)
        {
            Parametrizables Parametros = new Parametrizables();
            try
            {
                var parametrizable = db.Parametrizables.ToList();

                Parametros.origen = parametrizable[i].origen;
                Parametros.destino = parametrizable[i].destino;
                Parametros.extension = parametrizable[i].extension;

                CreateDirectory(Parametros.origen);
                Parametros.origen = Parametros.origen + Parametros.extension.ToString("D3");
                return Parametros;
            }
            catch (Exception)
            {
                return Parametros;
            }

        }

        public void CreateLSTABINT(Parametrizables Parametros, int i)
        {
            string consulta;
            SqlConnection SQLConn = new SqlConnection("data source=.;initial catalog=GTDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");

            SQLConn.Open();
            if (i == 0)
                consulta = "Exec master..xp_Cmdshell 'bcp " + "\"select iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),24-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),24-LEN(NumTag)))+(''01'')+ IIF(StatusTag = 1, ''01'',''00'') + IIF(SaldoTag>9999999,CONVERT(nvarchar,SUBSTRING(CONVERT(nvarchar,CAST(SaldoTag as numeric)),LEN(CAST(SaldoTag as numeric))-7,8)),REPLICATE(''0'',8-LEN(CAST(SaldoTag as numeric)) )+ CONVERT(nvarchar,CAST(SaldoTag as numeric)))+iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),19-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),19-LEN(NumTag)))+ IIF(StatusResidente = 1,''01'',''00'') + REPLICATE(''0'',49) from GTDB.dbo.Tags ORDER BY NumTag ASC;\" queryout \"" + Parametros.origen + "\"" + " -T -c -t \\0'";

            else
                consulta = "Exec master..xp_Cmdshell 'bcp " + "\"select iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),24-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),24-LEN(NumTag)))+(''01'')+ IIF(SaldoTag>=13500 AND StatusTag = 1, ''01'',''00'') + IIF(SaldoTag>9999999,CONVERT(nvarchar,SUBSTRING(CONVERT(nvarchar,CAST(SaldoTag as numeric)),LEN(CAST(SaldoTag as numeric))-7,8)),REPLICATE(''0'',8-LEN(CAST(SaldoTag as numeric)) )+ CONVERT(nvarchar,CAST(SaldoTag as numeric)))+iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),19-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),19-LEN(NumTag)))+ IIF(StatusResidente = 1,''01'',''00'') + REPLICATE(''0'',49) from GTDB.dbo.Tags ORDER BY NumTag ASC;\" queryout \"" + Parametros.origen + "\"" + " -T -c -t \\0'";

            var cmd = new SqlCommand(consulta, SQLConn);
            cmd.CommandTimeout = 3 * 60;
            cmd.ExecuteNonQuery();
            SQLConn.Close();
        }

        public bool Validaciones(Parametrizables Parametros, int i)
        {
            var creationdate = DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmm");
            string[] lines = System.IO.File.ReadAllLines(Parametros.origen);
            var countlines = lines.LongLength.ToString("D6");

            if (CountValidation(lines.LongLength))
            {
                countlines = countlines.Substring(countlines.Length - 6, 6);

                string encabezados = "63" + creationdate + creationdate + "0100" + Parametros.extension.ToString("D3") + countlines;

                string[] header = new string[1] { encabezados };
                File.Delete(Parametros.origen);
                File.AppendAllText(Parametros.origen, encabezados);
                File.AppendAllLines(Parametros.origen, lines);
                MoverLstabint(Parametros, i);
                return true;
            }
            else
            {
                File.Delete(Parametros.origen);
                return false;
            }
        }


        public bool CountValidation(long ListaCount)
        {
            long TagsCount = db.Tags.Count();

            if (ListaCount == TagsCount)
                return true;
            else
                return false;
        }

        private void MoverLstabint(Parametrizables Parametros, int i)
        {
            string nuevoorigen;
            if (i == 1)
            {
                foreach (var item in Directory.GetFiles(@"\\10.1.10.111\geaint\MONTOMINIMO", "LSTABINT.*"))
                {
                    File.Delete(item);
                }
            }

            nuevoorigen = GuardarLSTABINT(Parametros, i);
            TamañoLista[i] = new FileInfo(nuevoorigen).Length.ToString();
            //File.Copy(nuevoorigen, var.VDestino + var.extension);
            File.Delete(nuevoorigen);
        }

        public string GuardarLSTABINT(Parametrizables Parametros, int i)
        {
            string LstabintPath = db.Parametrizables.ToArray()[i].destinoresidente;
            string[] ArrayFecha = DateTime.Now.ToString("dd MMMM yyyy").Split(' ');
            LstabintPath += ArrayFecha[2] + @"\" + ArrayFecha[1] + @"\" + ArrayFecha[0];
            CreateDirectory(LstabintPath);

            LstabintPath += @"\LSTABINT." + Parametros.extension.ToString("D3"); 

            if (File.Exists(LstabintPath))
                File.Delete(LstabintPath);
            else if (File.Exists(LstabintPath + ".zip"))
                File.Delete(LstabintPath + ".zip");

            File.Move(Parametros.origen, LstabintPath);

            ZipFile zips = new ZipFile();
            zips.AddFile(LstabintPath);
            zips.Save(LstabintPath + ".zip");

            return LstabintPath;
        }

        public async void SendMessage(string Mensaje)
        {
            TelegramBotClient Bot = new TelegramBotClient("834404388:AAG8JcPTHi9API16h1TF5C_EgsB78QToaP8");
            await Bot.SendTextMessageAsync(-364639169, Mensaje);
        }

        public void CreateDirectory(string Directory)
        {
            bool Exists = System.IO.Directory.Exists(Directory);
            if (!Exists)
            {
                System.IO.Directory.CreateDirectory(Directory);
            }
        }
    }
}
