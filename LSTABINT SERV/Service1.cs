﻿using System;
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
        private static readonly TelegramBotClient Bot = new TelegramBotClient("834404388:AAG8JcPTHi9API16h1TF5C_EgsB78QToaP8");
        private GTDBEntities db = new GTDBEntities();
        string[] TamañoLista = new string[2];

        public LSTABINTSERVICE()
        {
            InitializeComponent();
            //    if (!System.Diagnostics.EventLog.SourceExists("MySource"))
            //    {
            //        System.Diagnostics.EventLog.CreateEventSource("MySource", "MyNewLog");
            //    }
            //    eventLog1.Source = "MySource";
            //    eventLog1.Log = "MyNewLog";
        }
        protected override void OnStart(string[] args)
        {

            tmGenera = new System.Timers.Timer();
            tmGenera.Interval = 300000;
            tmGenera.Elapsed += TmGenera_Elapsed;
            tmGenera.Enabled = true;
            tmGenera.Start();
        }
        public void Inicio()
        {
            //eventLog1.WriteEntry("Termine");

            tmGenera = new System.Timers.Timer();
            tmGenera.Interval = 10000;
            tmGenera.Elapsed += TmGenera_Elapsed;
            tmGenera.Enabled = true;
            tmGenera.Start();
        }
        private void TmGenera_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmGenera.Enabled = false;
            GeneraArchivo();
        }
        public bool Conexion()
        {
            try
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send("10.1.10.111");
                //PingReply pingReply = ping.Send("192.168.0.69");
                if (pingReply.Status == IPStatus.Success)
                {

                    return true;
                }
                else
                    return false;
            }
            catch (Exception Ex)
            {
                tmGenera.Enabled = true;
                File.WriteAllText(@"C:\temporal\Error" + db.Parametrizables.FirstOrDefault().extension + ".txt", Ex.Message);
                return false;
            }
        }
        private void GeneraArchivo()
        {
            try
            {
                tmGenera.Enabled = false;
                if (!Conexion())
                {
                    MessageBox.Show("La conexión al servidor ha fallado", "Error", MessageBoxButtons.OK);
                    Inicio();
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
                        varparametros = parametros(varparametros, i);
                        archivonormal(varparametros, i);
                        Validado = validaciones(varparametros, i);
                        if (Validado)
                        {
                            db.HistoricoListas.Add(new HistoricoListas
                            {
                                Fecha_Creacion = DateTime.Now,
                                Tamaño = TamañoLista[i] + " KB",
                                Extension = varparametros.extension,
                                Tipo = i == 0 ? "MultiModal" : "Exclusivo"
                            });
                            varparametros.Equals(null);
                        }
                    }
                    if (Validado)
                    {
                        var listas = db.Parametrizables.ToList();
                        foreach (var item in listas)
                        {
                            item.extension++;
                            if (item.extension > 999)
                            {
                                item.extension = 1;
                            }
                        }
                        db.SaveChanges();
                        tmGenera.Enabled = true;
                    }
                    else
                    {
                        CheckServiceTags();
                    }
                }
            }
            catch (Exception Ex)
            {
                tmGenera.Enabled = true;
                //MessageBox.Show(Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.WriteAllText(@"C:\temporal\Error" + db.Parametrizables.FirstOrDefault().extension + ".txt", Ex.Message);
            }

        }
        protected override void OnStop()
        {
            tmGenera.Stop();

            //EventLog.WriteEntry("El servicio se ha detenido");
        }

        public async void CheckServiceTags()
        {
            await Bot.SendTextMessageAsync(-364639169, "No coincide el número de tags en listas con el número de Tags existentes");
        }

        public variables parametros(variables nuevasvar, int i)
        {
            try
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
                bool exists = System.IO.Directory.Exists(nuevasvar.VOrigen);
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(nuevasvar.VOrigen);
                }
                nuevasvar.VOrigen = nuevasvar.VOrigen + nuevasvar.extension;
                return nuevasvar;
            }
            catch (Exception Ex)
            {
                File.WriteAllText(@"C:\temporal\Error" + db.Parametrizables.FirstOrDefault().extension + ".txt", Ex.Message);
                throw;
            }
            
        }
        public bool validaciones(variables varenca, int i)
        {
            try
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
            catch (Exception Ex)
            {
                File.WriteAllText(@"C:\temporal\Error" + varenca.extension + ".txt", Ex.Message);
                throw;
            }
            
        }


        public bool CountValidation(long ListaCount)
        {
            string consulta = "select count(*) from Tags";

            SqlConnection SQLConn = new SqlConnection("data source=.;initial catalog=GTDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework;");

            SQLConn.Open();

            SqlCommand Command = new SqlCommand(consulta, SQLConn);

            var resultado = Command.ExecuteScalar();

            if (ListaCount == Convert.ToInt32(resultado))
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
            try
            {
                string nuevoorigen;
                if (i == 0)
                {
                    if (!File.Exists(var.VDestino + var.extension))
                    {
                        nuevoorigen = GuardarLSTABINT(var.VOrigen, i);
                        TamañoLista[i] = new FileInfo(nuevoorigen).Length.ToString();
                        File.Copy(nuevoorigen, var.VDestino + var.extension);
                    }
                    else
                    {
                        File.Delete(var.VDestino + var.extension);
                        nuevoorigen = GuardarLSTABINT(var.VOrigen, i);
                        TamañoLista[i] = new FileInfo(nuevoorigen).Length.ToString();
                        File.Copy(nuevoorigen, var.VDestino + var.extension);
                    }
                }
                else
                {
                    foreach (var item in Directory.GetFiles(@"\\10.1.10.111\geaint\MONTOMINIMO", "LSTABINT.*"))
                    {
                        File.Delete(item);
                    }
                    if (!File.Exists(var.VDestino))
                    {
                        nuevoorigen = GuardarLSTABINT(var.VOrigen, i);
                        File.Copy(nuevoorigen, var.VDestino + var.extension);
                    }
                    else
                    {
                        File.Delete(var.VDestino + var.extension);
                        nuevoorigen = GuardarLSTABINT(var.VOrigen, i);
                        File.Copy(nuevoorigen, var.VDestino + var.extension);
                    }
                }
            }
          catch (Exception Ex)
            {
                File.WriteAllText(@"C:\temporal\Error" + var.extension + ".txt", Ex.Message);
                throw;
            }

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
            if (!Directory.Exists(LstabintPath))
            {
                Directory.CreateDirectory(LstabintPath);
            }

            LstabintPath += actualpath.Substring(actualpath.Length - 13, 13);  //13 caracteres = "\LSTABINT.###"

            if (File.Exists(LstabintPath))
            {
                File.Delete(LstabintPath);
            }

            File.Move(actualpath, LstabintPath);
            return LstabintPath;
        }

        public void archivonormal(variables variableslistas, int i)
        {
            try
            {
                string query = "data source=.;initial catalog=GTDB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
                var conexion = new SqlConnection();
                conexion.ConnectionString = query;
                conexion.Open();
                string final = "\\0'";
                string consulta;
                if (i == 0)
                    consulta = "Exec master..xp_Cmdshell 'bcp " + "\"select iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),24-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),24-LEN(NumTag)))+(''01'')+ IIF(StatusTag = 1, ''01'',''00'') + IIF(SaldoTag>9999999,CONVERT(nvarchar,SUBSTRING(CONVERT(nvarchar,CAST(SaldoTag as numeric)),LEN(CAST(SaldoTag as numeric))-7,8)),REPLICATE(''0'',8-LEN(CAST(SaldoTag as numeric)) )+ CONVERT(nvarchar,CAST(SaldoTag as numeric)))+iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),19-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),19-LEN(NumTag)))+ IIF(StatusResidente = 1,''01'',''00'') + REPLICATE(''0'',49) from GTDB.dbo.Tags ORDER BY NumTag ASC;\" queryout \"" + variableslistas.VOrigen + "\"" + " -T -c -t" + final;
                else
                    consulta = "Exec master..xp_Cmdshell 'bcp " + "\"select iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),24-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),24-LEN(NumTag)))+(''01'')+ IIF(SaldoTag>=13500 AND StatusTag = 1, ''01'',''00'') + IIF(SaldoTag>9999999,CONVERT(nvarchar,SUBSTRING(CONVERT(nvarchar,CAST(SaldoTag as numeric)),LEN(CAST(SaldoTag as numeric))-7,8)),REPLICATE(''0'',8-LEN(CAST(SaldoTag as numeric)) )+ CONVERT(nvarchar,CAST(SaldoTag as numeric)))+iif(SUBSTRING(NumTag,1,3) = ''501'',SUBSTRING(NumTag,1,3)+''00''+SUBSTRING(NumTag,4,LEN(NumTag)-3) + REPLICATE(SPACE(1),19-LEN(NumTag)-2), NumTag + REPLICATE(SPACE(1),19-LEN(NumTag)))+ IIF(StatusResidente = 1,''01'',''00'') + REPLICATE(''0'',49) from GTDB.dbo.Tags ORDER BY NumTag ASC;\" queryout \"" + variableslistas.VOrigen + "\"" + " -T -c -t" + final;
                var cmd = new SqlCommand(consulta, conexion);
                cmd.CommandTimeout = 3 * 60;
                cmd.ExecuteNonQuery();
            }
            catch (Exception Ex)
            {
                File.WriteAllText(@"C:\temporal\Error" + variableslistas.extension + ".txt", Ex.Message);
                throw;
            }
           
        }
    }
}
