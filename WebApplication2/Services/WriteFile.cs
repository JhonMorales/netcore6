namespace WebApplication2.Services
{
    //public class WriteFile : IHostedService
    //{
    //    private readonly IWebHostEnvironment env;
    //    private readonly string nombreArchivo = "Arhivo1.txt";
    //    private Timer timer;

    //    public WriteFile(IWebHostEnvironment env)
    //    {
    //        this.env = env;
    //    }

    //    public Task StartAsync(CancellationToken cancellationToken)
    //    {
    //        timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    //        Escribir("Proceso iniciado");
    //        return Task.CompletedTask;
    //    }

    //    public Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        timer.Dispose();
    //        Escribir("Proceso finalizado");
    //        return Task.CompletedTask;
    //    }

    //    private void DoWork(object state)
    //    {
    //        Escribir("Proceso en ejecución: " +DateTime.Now.ToString("dd/MM/yy hh:mm:ss"));
    //    }

    //    private void Escribir(string mensaje)
    //    {
    //        var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
    //        using (StreamWriter write = new StreamWriter(ruta, append: true))
    //        {
    //            write.WriteLine(mensaje);
    //        }
    //    }
    //}
}
