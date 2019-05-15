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

namespace LSTABINT_SERV
{
    public partial class LSTABINTSERVICE : ServiceBase
    {
        private System.Timers.Timer tmGenera = null;
        private GTDBEntities db = new GTDBEntities();
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
            tmGenera.Interval = 180000;
            tmGenera.Elapsed += TmGenera_Elapsed;
            tmGenera.Enabled = true;
            tmGenera.Start();
        }
        public void Inicio()
        {
            //eventLog1.WriteEntry("Termine");

            tmGenera = new System.Timers.Timer();
            tmGenera.Interval = 1800;
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
                //PingReply pingReply = ping.Send("10.1.10.111");
                PingReply pingReply = ping.Send("192.168.0.69");
                if (pingReply.Status == IPStatus.Success)
                {

                    return true;
                }
                else
                    return false;
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Proporcione credenciales al servidor", "Error", MessageBoxButtons.OK);
                return false;
            }
        }
        private void GeneraArchivo()
        {
            try
            {

                if (!Conexion())
                {
                    MessageBox.Show("La conexión al servidor ha fallado", "Error", MessageBoxButtons.OK);
                    Inicio();
                }
                else
                {
                    var TamañoLista = new string[2];
                    foreach (var item in Directory.GetFiles(@"C:\temporal\", "LSTABINT.*"))
                    {
                        File.Delete(item);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        variables varparametros = new variables();
                        varparametros = parametros(varparametros, i);
                        archivonormal(varparametros, i);
                        encabezados(varparametros, i);
                        TamañoLista[i] =  new FileInfo (varparametros.VDestino + varparametros.extension).Length.ToString();
                        db.HistoricoListas.Add(new HistoricoListas
                        {
                            Fecha_Creacion = DateTime.Now,
                            Tamaño = TamañoLista[i] + " KB",
                            Extension = varparametros.extension,
                            Tipo = i == 0 ? "MultiModal" : "Exclusivo"
                        });
                        varparametros.Equals(null);
                    }
                    var listas = db.Parametrizables.ToList();
                    foreach (var item in listas)
                    {

                        item.extension++;
                        if (item.extension > 999 ){
                            item.extension = 1;
                        }
                    }
                    
                    db.SaveChanges();
                    tmGenera.Enabled = true;
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.WriteAllText(@"C:\temporal\Error" + db.Parametrizables.FirstOrDefault().extension + ".txt", Ex.Message);
            }

        }
        protected override void OnStop()
        {
            tmGenera.Stop();

            //EventLog.WriteEntry("El servicio se ha detenido");
        }
        public variables parametros(variables nuevasvar, int i)
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
        public variables encabezados(variables varenca, int i)
        {
            
            var aplicationdate = DateTime.Now.ToString("yyyyMMddHHmm");
            var creationdate = DateTime.Now.ToString("yyyyMMddHHmm");
            string formato = "000000";
            string[] lines = System.IO.File.ReadAllLines(varenca.VOrigen);
            string countlines = lines.LongLength.ToString(formato);
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
            return varenca;
        }

        private void MoverLstabint(variables var, int i)
        {
            try
            {
                if (i == 0)
                {
                    if (!File.Exists(var.VDestino + var.extension))
                    {
                        File.Move(var.VOrigen, var.VDestino + var.extension);
                    }
                    else
                    {
                        File.Delete(var.VDestino + var.extension);
                        File.Move(var.VOrigen, var.VDestino + var.extension);
                    }
                }
                else
                {
                    foreach (var item in Directory.GetFiles(@"\\192.168.0.69\geaint\PARAM\MontoMinimo\", "LSTABINT.*"))
                    {
                        File.Delete(item);
                    }
                    if (File.Exists(var.VDestino) == false)
                    {
                        File.Move(var.VOrigen, var.VDestino + var.extension);
                        File.Delete(var.VOrigen);
                    }
                    else
                    {
                        File.Delete(var.VDestino + var.extension);
                        File.Move(var.VOrigen, var.VDestino + var.extension);
                    }
                }
            }
            catch (Exception Ex)
            {
                File.WriteAllText(@"C:\temporal\Error" + var.extension + ".txt", Ex.Message);
                throw;
            }

        }

        public void archivonormal(variables variableslistas, int i)
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
    }
}
