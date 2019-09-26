using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Security;
using LSTABINT_SERV.Model;
using System.Data.Common;
using Telegram.Bot;

namespace LSTABINT_SERV
{
    public partial class LSTABINTSERVICE : ServiceBase
    {
        private System.Timers.Timer tmGenera = null;
        SqlConnection SQLConn = new SqlConnection("data source=.;initial catalog=GTDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
        private static readonly TelegramBotClient Bot = new TelegramBotClient("834404388:AAG8JcPTHi9API16h1TF5C_EgsB78QToaP8");
        private GTDBEntities1 db = new GTDBEntities1();
        string[] TamañoLista = new string[2];

        public LSTABINTSERVICE()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            tmGenera = new System.Timers.Timer
            {
                Interval = 300000
            };
            tmGenera.Elapsed += new System.Timers.ElapsedEventHandler(TmGenera_Elapsed);
            tmGenera.Enabled = true;
            tmGenera.Start();
        }
        public void Inicio()
        {
            tmGenera = new System.Timers.Timer
            {
                Interval = 10000
            };
            tmGenera.Elapsed += new System.Timers.ElapsedEventHandler(TmGenera_Elapsed); 
            tmGenera.Enabled = true;
            tmGenera.Start();
        }
        private void TmGenera_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmGenera.Enabled = false;
            tmGenera.Stop();
            GeneraArchivo();
        }
        public bool Conexion()
        {
            Ping ping = new Ping();
            PingReply pingReply = ping.Send("10.1.10.111");
            //PingReply pingReply = ping.Send("192.168.0.69");
            if (pingReply.Status == IPStatus.Success)
                return true;
            else
                return true;
        }
        private void GeneraArchivo()
        {
            try
            {
                variables varparametros2 = new variables();
                CreateLSTABINT(varparametros2, 0);
                if (!Conexion())
                {
                    SendMessage("La conexión al servidor ha fallado");
                    File.WriteAllText(@"C:\temporal\Error" + db.Parametrizables.FirstOrDefault().extension + ".txt", "La conexión al servidor ha fallado");
                    tmGenera.Enabled = true;
                    tmGenera.Start();
                }
                else
                {
                    bool Validado = false;
                    foreach (var item in Directory.GetFiles(@"C:\temporal\", "LSTABINT.*"))
                    {
                        File.Delete(item);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        variables varparametros = new variables();
                        varparametros = GetParametros(varparametros, i);
                        CreateLSTABINT(varparametros, i);
                        Validado = Validaciones(varparametros, i);

                        if (Validado)
                        {
                            db.HistoricoListas.Add(new HistoricoListas
                            {
                                Fecha_Creacion = DateTime.Now,
                                Tamaño = TamañoLista[i] + " KB",
                                Extension = varparametros.extension,
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
                    tmGenera.Enabled = true;
                    tmGenera.Start();
                }
            }
            catch (Exception Ex)
            {
                string ErrorPath = @"C:\temporal\Errores\LSTABINT\";
                string[] ArrayFecha = DateTime.Now.ToString("dd MMMM yyyy").Split(' ');
                ErrorPath += ArrayFecha[2] + @"\" + ArrayFecha[1] + @"\" + ArrayFecha[0] + @"\";
                CreateDirectory(ErrorPath);

                File.WriteAllText(ErrorPath + "Error" + db.Parametrizables.FirstOrDefault().extension + ".txt", Ex.Message + Ex.StackTrace);
                SendMessage("Ocurrió un error en la generación de la LSTABINT." + db.Parametrizables.FirstOrDefault().extension + ": "+ Ex.Message);

                tmGenera.Enabled = true;
                tmGenera.Start();
            }

        }
        protected override void OnStop()
        {
            //SendMessage("LSTABINT Service se detuvo");
            File.WriteAllText(@"C:\temporal\Stop.txt", "Se detuvo");
        }

        public variables GetParametros(variables nuevasvar, int i)
        {
            var parametrizable = db.Parametrizables.ToList();
            if (parametrizable.Count == 2)
            {
                nuevasvar.VOrigen = parametrizable[i].origen;
                nuevasvar.VDestino = parametrizable[i].destino;
                nuevasvar.extension = parametrizable[i].extension.ToString();
                char[] prueba = nuevasvar.extension.ToCharArray();
                nuevasvar.extension = nuevasvar.extension.PadLeft(3, '0');
            }
            else
            {
                SendMessage("El número de registros en la tabla Parametrizable es diferemte a 2");
            }
            CreateDirectory(nuevasvar.VOrigen);
            nuevasvar.VOrigen = nuevasvar.VOrigen + nuevasvar.extension;
            return nuevasvar;
        }
        public bool Validaciones(variables varenca, int i)
        {
            var aplicationdate = DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmm");
            var creationdate = DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmm");
            string formato = "000000";
            string[] lines = System.IO.File.ReadAllLines(varenca.VOrigen);
            var countlines = lines.LongLength.ToString(formato);

            if (CountValidation(lines.LongLength))
            {
                countlines = countlines.Substring(countlines.Length - 6, 6);
                countlines = Convert.ToInt32(countlines).ToString();
                countlines = countlines.PadLeft(6, '0');
                string encabezados = "63" + aplicationdate + creationdate + "0100" + varenca.extension + countlines;
                foreach (var item in lines)
                {
                    item.Trim();
                }
                string[] header = new string[1] { encabezados };
                File.Delete(varenca.VOrigen);
                File.AppendAllLines(varenca.VOrigen, header);
                File.AppendAllLines(varenca.VOrigen, lines);
                MoverLstabint(varenca, i);
                return true;
            }
            else
            {
                File.Delete(varenca.VOrigen);
                return false;
            }
        }


        public bool CountValidation(long ListaCount)
        {
            long TagsCount = db.Tags.Count();

            if (ListaCount == TagsCount)
            {
                SQLConn.Close();
                return true;
            }
            else
            {
                SQLConn.Close();
                return false;
            }
        }

        private void MoverLstabint(variables var, int i)
        {
            string nuevoorigen;
            if (i == 1)
            {
                foreach (var item in Directory.GetFiles(@"\\10.1.10.111\geaint\MONTOMINIMO", "LSTABINT.*"))
                {
                    File.Delete(item);
                }
            }
            if (File.Exists(var.VDestino + var.extension))
                File.Delete(var.VDestino + var.extension);
            nuevoorigen = GuardarLSTABINT(var.VOrigen, i);
            TamañoLista[i] = new FileInfo(nuevoorigen).Length.ToString();
            File.Copy(nuevoorigen, var.VDestino + var.extension);
        }

        public string GuardarLSTABINT(string actualpath, int i)
        {
            string LstabintPath;
            if (i == 0)
            {
                LstabintPath = @"C:\temporal\LSTABINT\";
            }
            else
            {
                LstabintPath = @"C:\temporal\MontoMinimo\";
            }
            string dia = DateTime.Now.ToString("dd MMMM yyyy");
            string[] ArrayFecha = dia.Split(' ');
            LstabintPath += ArrayFecha[2] + @"\" + ArrayFecha[1] + @"\" + ArrayFecha[0];
            CreateDirectory(LstabintPath);

            LstabintPath += actualpath.Substring(actualpath.Length - 13, 13);  //13 caracteres = "\LSTABINT.###"

            if (File.Exists(LstabintPath))
            {
                File.Delete(LstabintPath);
            }

            File.Move(actualpath, LstabintPath);
            return LstabintPath;
        }

        public void CreateLSTABINT(variables variableslistas, int i)
        {
            string final = "\\0'";
            string consulta;

            SQLConn.Open();
            if (i == 0)
                consulta = "Exec master..xp_Cmdshell 'bcp " + "\"select iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),24-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),24-LEN(NumTag)))+(''01'')+ IIF(StatusTag = 1, ''01'',''00'') + IIF(SaldoTag>9999999,CONVERT(nvarchar,SUBSTRING(CONVERT(nvarchar,CAST(SaldoTag as numeric)),LEN(CAST(SaldoTag as numeric))-7,8)),REPLICATE(''0'',8-LEN(CAST(SaldoTag as numeric)) )+ CONVERT(nvarchar,CAST(SaldoTag as numeric)))+iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),19-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),19-LEN(NumTag)))+ IIF(StatusResidente = 1,''01'',''00'') + REPLICATE(''0'',49) from GTDB.dbo.Tags ORDER BY NumTag ASC;\" queryout \"" + variableslistas.VOrigen + "\"" + " -T -c -t" + final;

            else
                consulta = "Exec master..xp_Cmdshell 'bcp " + "\"select iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),24-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),24-LEN(NumTag)))+(''01'')+ IIF(SaldoTag>=13500 AND StatusTag = 1, ''01'',''00'') + IIF(SaldoTag>9999999,CONVERT(nvarchar,SUBSTRING(CONVERT(nvarchar,CAST(SaldoTag as numeric)),LEN(CAST(SaldoTag as numeric))-7,8)),REPLICATE(''0'',8-LEN(CAST(SaldoTag as numeric)) )+ CONVERT(nvarchar,CAST(SaldoTag as numeric)))+iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),19-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),19-LEN(NumTag)))+ IIF(StatusResidente = 1,''01'',''00'') + REPLICATE(''0'',49) from GTDB.dbo.Tags ORDER BY NumTag ASC;\" queryout \"" + variableslistas.VOrigen + "\"" + " -T -c -t" + final;

            var cmd = new SqlCommand(consulta, SQLConn);
            cmd.CommandTimeout = 3 * 60;
            cmd.ExecuteNonQuery();
            SQLConn.Close();
        }

        public async void SendMessage(string Mensaje)
        {
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
